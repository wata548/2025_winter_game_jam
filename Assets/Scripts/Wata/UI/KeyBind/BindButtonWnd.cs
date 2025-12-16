using System;
using Game.Input;
using UnityEngine;
using Wata.UI;

namespace UI {
    public class BindButtonWnd: SettingWindowBase {
        
        //==================================================||Fields        
        [SerializeField] private BindButton _bindPrefab;
        [SerializeField] private GameObject _wnd;
        [SerializeField] private NewKeyBindWnd _newKeyWnd;
        
        [Space, Header("Interval")] 
        [SerializeField] private float _horizontalInterval = 800;
        [SerializeField] private float _verticalInterval = 300;
        
       //==================================================||Methods         
        public override void TurnOn(bool pOn) {
            _wnd.SetActive(pOn);
        }
        
       //==================================================||Unity
        private void Awake() {
            var temp = Enum.GetValues(typeof(InputAction));
            var length = temp.Length / 2 + (temp.Length % 2 == 0 ? 0 : 1);
            var idx = 0;
            
            foreach (InputAction action in temp) {

                var newObj = Instantiate(_bindPrefab, _wnd.transform);
                newObj.SetUp(action, _newKeyWnd);
                var pos = newObj.transform.position;
                pos.x += (idx / length) * _horizontalInterval;
                pos.y -= (idx % length) * _verticalInterval;
                newObj.transform.position = pos;
                idx++;
            }
        }
    }
}