using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Wata.UI.Sound {

    public class SoundUI: MonoBehaviour {

        [SerializeField] private GameObject _window;
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _effectSlider;
        [SerializeField] private Slider _bgmSlider;

        public void SetActiveWindow(bool pOn) {
            _window.SetActive(pOn);
        }
        
        private void SetListener() {
            _masterSlider.onValueChanged.AddListener(value => SoundManager.Instance.Master = value );
            _effectSlider.onValueChanged.AddListener(value => SoundManager.Instance.Effect = value );
            _bgmSlider.onValueChanged.AddListener(value => SoundManager.Instance.Bgm = value);
        }
        
        private void Awake() {
            SetListener();
        }
    }
}