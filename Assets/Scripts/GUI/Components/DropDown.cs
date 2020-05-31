using System;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Components
{
    public class DropDown : MonoBehaviour
    {
        private DropdownBase mainDropdownDispatcher;

        public GameObject UI;
        
        void Start()
        {
            var dropdownDispatcher = Type.GetType($"GUI.DropdownScripts.{name}Script");
            if (dropdownDispatcher == null)
            {
                Debug.Log($"Cannot find {name}Script");
                return;
            }
            
            mainDropdownDispatcher = (DropdownBase)Activator.CreateInstance(dropdownDispatcher, new object[] { UI });
            
            var allButtons = GetComponentsInChildren<Button>();
            foreach (var button in allButtons)
            {
                button.onClick.AddListener(delegate { ButtonClickedEvent(button.name); });
            }
        }

        private void ButtonClickedEvent(string buttonName)
        {
            mainDropdownDispatcher.DispatchEvent(buttonName);
            gameObject.SetActive(false);
        }
    }
}