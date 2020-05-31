using System;
using System.Collections.Generic;
using Casc;
using Interfaces;
using Settings;
using UnityEngine;

namespace GUI.DropdownScripts
{
    public class FileDropdownScript : DropdownBase
    {
        public FileDropdownScript(GameObject ui) : base(ui)
        {
            Events.Add("OpenCasc", OpenCasc);
            Events.Add("Preferences", Preferences);
        }

        private void OpenCasc()
        {
            LoadingScreen.SetActive(true);
            
            CASC.Initialize(SettingsManager<ModelViewerConfig>.Config, LoadingScreen);
        }

        private void Preferences()
        {
            Settings.SetActive(true);
        }
    }
}