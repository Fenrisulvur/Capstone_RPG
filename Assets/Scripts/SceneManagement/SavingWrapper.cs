using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        IEnumerator Start() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
        }

        // void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.L))
        //     {
        //         Load();
        //     }
        //     if (Input.GetKeyDown(KeyCode.S))
        //     {
        //         Save();
        //     }
        // }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            //Call to saving system
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }
}
