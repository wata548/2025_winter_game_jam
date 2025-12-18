using UnityEngine;

namespace Wata {
    public class XPosRemainer: MonoBehaviour {
        private void FixedUpdate() {
            var pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }    
    }
}