using System;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        public Text StatusText;
        public Slider Progressbar;

        public void SetStatus(string text, uint value)
        {
            StatusText.text = text;
            Progressbar.value = value;
        }
    }
}