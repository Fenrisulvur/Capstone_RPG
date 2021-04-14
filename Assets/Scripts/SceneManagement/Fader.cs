using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup CanvasGroup;
        
        private void Start() {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        IEnumerator FadeOutIn()
        {
            yield return FadeOut(3f);
            print("Faded out");
            yield return FadeIn(3f);
            print("Faded in");
        }

        public IEnumerator FadeOut(float time)
        {
            while (CanvasGroup.alpha < 1)
            {
                CanvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (CanvasGroup.alpha > 0)
            {
                CanvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }   
}
