using System;
using System.Linq;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Physic {
    [RequireComponent(typeof(Rigidbody))]
    public class Movement: MonoBehaviour {
        
        //==================================================||Constants 
        private const float JUMP_PEEK_HEIGHT = 4f;
        private const float JUMP_PEEK_INTERVAL = 0.3f;
        private const float JUMP_SCALE = 2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL;
        private const float GRAVITY_SCALE = -JUMP_SCALE / JUMP_PEEK_INTERVAL;
        private const float GROUND_CHECK_OFFSET = 0.95f;
        private const float GROUND_CHECKER_HEIGHT = 0.01f;

        //==================================================||Fields 
        private Rigidbody _rigid = null;
        private bool _isGround = false;
        private bool _slowFalling = false;
        private float _slowFallingPower = 0.5f;
        
        //==================================================||Methods 
        public void SetActiveSlowFalling(bool pOn, float pPower = 0.3f) {
            _slowFallingPower = pPower;
            _slowFalling = pOn;
        }
        
        public void Jump() {
            var velocity = _rigid.linearVelocity;
            velocity.y += JUMP_SCALE;
            _rigid.linearVelocity = velocity;
        }
        
        private bool IsGround(out float pLength) {
            pLength = 0;
            
            var velocity = _rigid.linearVelocity.y;
            if(_isGround && velocity == 0)
                return true;
            
            var halfScale = transform.localScale/ 2f;
            var center = transform.position;
            center.y += halfScale.y;
            halfScale *= GROUND_CHECK_OFFSET;
            halfScale.y = GROUND_CHECKER_HEIGHT / 2;

            var contacts = Physics.BoxCastAll(
                center,
                halfScale,
                Vector3.down,
                Quaternion.identity,
                -velocity * Time.timeScale * Time.deltaTime + transform.localScale.y,
                LayerMask.GetMask("Ground")
            );
            if (contacts.Length == 0)
                return false;
            
            pLength = contacts.Min(hit => hit.distance) - transform.localScale.y;
            return true;
        }

        private void GravityProcess() {
            var velocity = _rigid.linearVelocity;

            _isGround = IsGround(out var length);
            if(velocity.y <= 0 && _slowFalling)
                velocity.y += GRAVITY_SCALE * Time.deltaTime * _slowFallingPower;
            else 
                velocity.y += GRAVITY_SCALE * Time.deltaTime;
            
            if (_isGround) {
                velocity.y = 0;
                var pos = transform.position;
                pos.y -= length;
                transform.position = pos;
                _slowFalling = false;
            }
            _rigid.linearVelocity = velocity;
        }
        //==================================================||Unity
        private void Awake() {
        
            _rigid = GetComponent<Rigidbody>();
            _rigid.useGravity = false;
        }

        private void Update() {
            GravityProcess();
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (_isGround)
                    Jump();
                else
                    SetActiveSlowFalling(true);
            }
        }
/*
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            var scale = transform.localScale;
            var center = transform.position;
            center.y -= scale.y / 2;
            scale *= GROUND_CHECK_OFFSET;
            scale.y = _rigid != null ? _rigid.linearVelocity.y * Time.deltaTime : 0;
            center.y += scale.y / 2;

            Gizmos.color = Color.red;
            Gizmos.DrawCube(center, scale);
        }
#endif*/
    }

}