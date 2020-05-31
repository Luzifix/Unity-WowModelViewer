using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Interfaces
{
    public abstract class DropdownBase
    {
        public Dictionary<string, Action> Events = new Dictionary<string, Action>();
        
        public GameObject UI;
        public GameObject Settings;
        public GameObject LoadingScreen;

        public DropdownBase(GameObject ui)
        {
            UI = ui;

            LoadingScreen = UI.FindObject("LoadingScreen");
            Settings = UI.FindObject("Settings");
        }

        public void DispatchEvent(string functionName)
        {
            Events.TryGetValue(functionName, out var action);
            action?.Invoke();
        }
    }
}