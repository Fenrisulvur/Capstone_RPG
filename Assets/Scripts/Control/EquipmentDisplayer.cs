using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

public class EquipmentDisplayer : MonoBehaviour
{
    Equipment equipment;
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;

    private void Awake() {
        equipment = GetComponent<Equipment>();
        equipment.equipmentUpdated += UpdateEquipmentDisplay;
    }

    private void UpdateEquipmentDisplay()
    {
        if (equipment.GetItemInSlot(EquipLocation.Shield)!=null)
        {
            DestroyOldEquipModel(leftHandTransform, "Shield");
            if (equipment.GetItemInSlot(EquipLocation.Shield) is StatsEquipableItem equippedWep && equippedWep.GetDisplayObject() != null )
            {
                AttachItem(equippedWep.GetDisplayObject(), leftHandTransform, "Shield");
            }
        }
        else
        {
            DestroyOldEquipModel(leftHandTransform, "Shield");
        }
    }

    private void AttachItem(GameObject equippedWep, Transform transform, string name)
    {
        GameObject newItem = Instantiate(equippedWep, transform);
        newItem.gameObject.name = name;
    }

    private void DestroyOldEquipModel(Transform location, string name)
    {
        Transform oldItem = location.Find(name);
        if (oldItem == null) return;
        oldItem.name = "DESTROYING";
        Destroy(oldItem.gameObject);
    }
    
}

