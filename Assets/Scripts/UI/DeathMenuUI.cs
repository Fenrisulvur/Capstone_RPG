using System.Collections;
using System.Collections.Generic;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenuUI : MonoBehaviour
{
    Button button;

    private void Awake() {
        button = transform.GetComponent<Button>();    
        button.onClick.AddListener(ClickHandler);
    }

    private void ClickHandler()
    {
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        wrapper.Reload();
    }
}
