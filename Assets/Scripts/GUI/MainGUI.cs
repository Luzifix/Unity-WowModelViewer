using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace GUI
{
    public class MainGUI : MonoBehaviour
    {
        private GameObject activeDropdown;
        public GameObject ToolBar;
        public GameObject ToolBar2;

        public Canvas MainBackground;

        // Start is called before the first frame update
        void Start()
        {
            var allButtons = ToolBar.GetComponentsInChildren<Button>();
            foreach (var button in allButtons)
            {
                button.onClick.AddListener(delegate { ButtonClicked(button.name); });
            }

            var stateDropdown = GameObject.Find("StateDropdown").GetComponent<Dropdown>();
            stateDropdown.onValueChanged.AddListener(OnStateChanged);
            stateDropdown.value = 0;
        }

        private void ButtonClicked(string buttonName)
        {
            var dropDown = ToolBar.FindObject($"{buttonName}Dropdown");
            if (dropDown == null)
            {
                Debug.Log($"Cannot find dropdown: {buttonName}Dropdown");
                return;
            }
            
            if (activeDropdown != null && activeDropdown != dropDown)
                activeDropdown.SetActive(false);
            
            dropDown.SetActive(!dropDown.activeSelf);
            activeDropdown = dropDown;
        }
        
        private void OnStateChanged(int value)
        {
            var stateDropdown = GameObject.Find("StateDropdown").GetComponent<Dropdown>();
            switch (stateDropdown.value)
            {
                case 0:    // Model Viewer
                    MainBackground.gameObject.SetActive(true);
                    GuiConstants.IsInModelPreview = true;
                    break;
                case 1:    // Map Viewer
                    MainBackground.gameObject.SetActive(false);
                    GuiConstants.IsInModelPreview = false;
                    break;
            }
        }
    }
}