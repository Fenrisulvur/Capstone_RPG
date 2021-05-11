using System;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class AlertPopup : MonoBehaviour {

        [SerializeField] TextMeshProUGUI text = null;
        float fadeDelay = 2;
        float timeSinceLastMsg = Mathf.Infinity;
        int opacity = 0;

        public void Send(string msg)
        {   
            timeSinceLastMsg = 0;
            opacity = 255;
            text.color = new Color32(255, 0, 0, (byte)opacity);
            text.SetText(msg);
        }
        private void LateUpdate() {
            timeSinceLastMsg += Time.deltaTime;
            if (timeSinceLastMsg >= fadeDelay)
                opacity = opacity - 10;
                text.color = new Color32(255, 0, 0, (Byte)Math.Max(opacity,0));
        }
    }
}