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

        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
            Debug.Log("Manual load");
        }

        public void Reload()
        {
            StartCoroutine(LoadLastScene());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
            Debug.Log("Manual save");
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
            Debug.Log("Manual delete");
        }
    }
}
