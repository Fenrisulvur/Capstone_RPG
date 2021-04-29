using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "dev-save";

        private void Awake() 
        {
            StartCoroutine(LoadLastScene());    
        }

        IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            //Call to saving system
            StartCoroutine( GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile) );
        }

        public void Delete()
        {
            //Call to saving system
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
