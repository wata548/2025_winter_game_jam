using System;
using Stage;
using UnityEngine;

namespace Wata.Stage {
    public class DisappearBlock: MonoBehaviour {
        public static bool IsOn { get; set; } = false;
        
        private void Update() {

            if (!IsOn)
                return;
            if (!StageManager.Instance.ExistEnemy)
                Destroy(gameObject);
        }
    }
}