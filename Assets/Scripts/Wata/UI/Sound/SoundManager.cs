using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sound {
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager: MonoBehaviour {

       //==================================================||Fields 
        private AudioSource _bgmPlayer = null;
        private Dictionary<string, AudioClip> _audios = null;
        
       //==================================================||Properties 
        public static SoundManager Instance { get; private set; } = null;
        public float Master { get; set; } = 1;
        public float Bgm { get; set; } = 1;
        public float Effect { get; set; } = 1;
 
        //==================================================||Methods 
        public void PlayEffect(Vector3 pPos, string pClip) {
            AudioSource.PlayClipAtPoint(_audios[pClip], pPos, Effect * Master);
        }

        public void PlayBackGround(string pClip) {
            _bgmPlayer.PlayOneShot(_audios[pClip], Bgm * Master);
        }

        private void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
               
            _audios = Resources.LoadAll<AudioClip>("Sounds")
                .ToDictionary(clip => clip.name, clip => clip);
            _bgmPlayer = GetComponent<AudioSource>();
            PlayBackGround("MainBGM");
        }
    }
}