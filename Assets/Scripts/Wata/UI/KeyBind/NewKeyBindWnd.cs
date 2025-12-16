using System;
using System.Linq;
using Game.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class NewKeyBindWnd: MonoBehaviour {

        //==================================================||Fields 
        [SerializeField] private GameObject _wnd;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _context;
        [SerializeField] private TMP_Text _bugInfo;
        [SerializeField] private Button _applyButton;

        private KeyCode _key;
        private BindButton _bindButton;

        //==================================================||Methods 
        public void TurnOn(BindButton pButton) {
            _wnd.SetActive(true);
            _bindButton = pButton;

            _key = pButton.Key;
            _title.text = pButton.Action.ToString();
            _context.text = _key.ToString();
        }

        public void TurnOff() {
            _wnd.SetActive(false);
            _bindButton = null;
        }

        public void ChangeKey() {
            _bindButton.ChangeKey(_key);
            _wnd.SetActive(false);
        }
        
        //==================================================||Unity 

        private void Update() {
            if (_bindButton == null)
                return;
            
            var key = ((KeyCode[])Enum.GetValues(typeof(KeyCode)))
                .FirstOrDefault(key => key != KeyCode.Mouse0 && Input.GetKeyDown(key));
            if (key == KeyCode.None)
                return;

            _key = key;
            _applyButton.interactable = _bindButton.IsChangeable(key, out var message);
            _context.text = key.ToString();
            _bugInfo.text = message;
        }
    }
}