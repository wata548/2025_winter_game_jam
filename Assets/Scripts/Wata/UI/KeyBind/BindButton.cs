using System;
using System.Collections.Generic;
using System.Linq;
using Game.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class BindButton: MonoBehaviour {
        [SerializeField] private TMP_Text _actionShower;
        [SerializeField] private TMP_Text _keyShower;
        public InputAction Action { get; private set; }
        public KeyCode Key{ get; private set; }

        private static HashSet<KeyCode> _keys = null;

        public void SetUp(InputAction pAction, NewKeyBindWnd pNewKeyBindWnd) {
            Action = pAction;
            Key = InputManager.Instance.GetKeyBinding(pAction);

            _keyShower.text = Key.ToString();
            _actionShower.text = Action.ToString();

            GetComponent<Button>().onClick
                .AddListener(() => pNewKeyBindWnd.TurnOn(this));
        }

        public bool IsChangeable(KeyCode pKey, out string pMessage) {
            if (pKey == Key) {
                pMessage = "It's same key";
                return true;
            }

            if (_keys.Contains(pKey)) {
                pMessage = "This key is already used";
                return false;
            }

            pMessage = "";
            return true;
        }

        public void ChangeKey(KeyCode pKey) {

            InputManager.Instance.SetKeyBinding(Action, pKey);
            Key = pKey;
            _keyShower.text = Key.ToString();
        }
            
        
        private void Awake() {
            if (_keys != null)
                return;

            _keys = new();
            foreach (InputAction action in Enum.GetValues(typeof(InputAction)))
                _keys.Add(InputManager.Instance.GetKeyBinding(action));
        }
    }
}