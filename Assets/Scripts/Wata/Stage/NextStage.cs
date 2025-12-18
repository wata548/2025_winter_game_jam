using System;
using Stage;
using UnityEngine;

namespace Wata {
    public class NextStage: MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            StageManager.Instance.NextStage();
        }
    }
}