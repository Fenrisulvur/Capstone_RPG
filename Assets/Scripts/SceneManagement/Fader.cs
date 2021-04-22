using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup CanvasGroup;
        Coroutine currentlyActiveFader = null;

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

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentlyActiveFader != null)
            {
                StopCoroutine(currentlyActiveFader);
            }
            currentlyActiveFader = StartCoroutine(FadeRoutine(target, time));
            return currentlyActiveFader;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(CanvasGroup.alpha, target))
            {
                CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        
    }   
}
