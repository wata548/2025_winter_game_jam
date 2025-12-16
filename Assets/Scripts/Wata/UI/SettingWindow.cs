using UnityEngine;

namespace UI {
    
    public class SettingWindow: SettingWindowBase {

        [SerializeField] private GameObject _wnd;
        
        public override void TurnOn(bool pOn) {
            _wnd.SetActive(pOn);
        }
    }
}