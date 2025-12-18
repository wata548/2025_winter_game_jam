using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kurohee {
    public class Title: MonoBehaviour {
        public void Load(string scene) => FadeController.Instance.Load(scene);
        
        public void StartGame() {
            FadeController.Instance.Load("Game");
        }

        public void Setting() {
            SettingManager.Instance.TurnOn();
        }

        public void Quit() {
            Application.Quit();
        }
    }
}