using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI {

    public enum SettingType {
        Setting,
        Sound,
        KeyBind
    }

    [Serializable]
    public struct SettingInfo {
        
        [field: SerializeField] public SettingType Type { get; private set; }
        [field: SerializeField] public SettingWindowBase Window { get; private set; }
    } 
    
    public class SettingManager: MonoBehaviour {

        //==================================================||Fields 
        [SerializeField] private GameObject _wnd;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _closeText;
        [SerializeField] private List<SettingInfo> _infos;
        private Dictionary<SettingType, SettingWindowBase> _matchSetting = null;
        
        private SettingType _curType = default;

        //==================================================||Methods 
        public void ChangeSettingType(string pType) =>
            ChangeSettingType(Enum.Parse<SettingType>(pType));
        
        public void ChangeSettingType(SettingType pType) {
            
            _matchSetting[_curType].TurnOn(false);
            _curType = pType;
            _matchSetting[_curType].TurnOn(true);
            _closeText.text = _curType == SettingType.Setting ? "Close" : "Back";
        }

        public void CloseButton() {
            if (_curType != SettingType.Setting) {
                ChangeSettingType(SettingType.Setting);
                return;
            }

            _wnd.SetActive(false);
        }
        
        //==================================================||Unity 
        private void Start() {
            _matchSetting ??= _infos
                .ToDictionary(info => info.Type, info => info.Window);

            ChangeSettingType(SettingType.Setting);
        }
    }
}