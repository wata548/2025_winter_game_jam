using System.Linq;
using UnityEngine;

namespace Physic {
    [RequireComponent(typeof(Rigidbody))]
    public class Movement: MonoBehaviour {
        
        //==================================================||Constants 
        protected const float JUMP_PEEK_HEIGHT = 4.5f;
        protected const float JUMP_PEEK_INTERVAL = 0.3f;
        protected const float JUMP_SCALE = 2 * JUMP_PEEK_HEIGHT / JUMP_PEEK_INTERVAL;
        protected const float GRAVITY_SCALE = -JUMP_SCALE / JUMP_PEEK_INTERVAL;
        protected const float GROUND_CHECK_OFFSET = 0.95f;
        protected const float GROUND_CHECKER_HEIGHT = 0.01f;

        //==================================================||Fields 
        protected Rigidbody _rigid = null;
        private bool _slowFalling = false;
        private float _slowFallingPower = 0.5f;
        
        //==================================================||Properties 
        public bool IsGround { get; private set; } = false;
        public bool SeeRight { get; private set; } = false;
        public Vector3 Velocity => _rigid.linearVelocity;
        
        //==================================================||Methods 
        public void SetActiveSlowFalling(bool pOn, float pPower = 0.3f) {
            _slowFallingPower = pPower;
            _slowFalling = pOn;
        }
        
        public void Move(Vector3 pDir, float pSpeed = 1) =>
            _rigid.linearVelocity += pDir.normalized * pSpeed;
        
        public void Jump(float pScale = 1) {
            var velocity = _rigid.linearVelocity;
            velocity.y += JUMP_SCALE * pScale;
            _rigid.linearVelocity = velocity;
        }

        public void SetHorizonPower(float pPower) {
            var velocity = _rigid.linearVelocity;
            if(pPower != 0)
                SeeRight = pPower > 0;
            velocity.x = pPower;
            _rigid.linearVelocity = velocity;
        }

        public bool IsMovable(Vector3 pDir) {
            var velocity = _rigid.linearVelocity + pDir;
            if (velocity.x == 0)
                return true;
            
            var nextPos = transform.position + velocity * Time.deltaTime;
            var adjust = Mathf.Sign(velocity.x) * transform.localScale.x;
            var existOther = Physics.OverlapBox(nextPos, transform.localScale / 2, Quaternion.identity)
                .Any(collider => collider.gameObject.CompareTag("Enemy") && collider.gameObject != gameObject);
            return !existOther && CheckGround(nextPos + adjust * Vector3.right, velocity.y + GRAVITY_SCALE, out _);      
        }
        
        private bool CheckGround(Vector3 pPos, float pGravity, out float pLength) {
            pLength = 0;
            if (pGravity > 0)
                return false;
            
            var halfScale = transform.localScale/ 2f;
            var center = pPos;
            center.y += halfScale.y;
            halfScale *= GROUND_CHECK_OFFSET;
            halfScale.y = GROUND_CHECKER_HEIGHT / 2;

            var contacts = Physics.BoxCastAll(
                center,
                halfScale,
                Vector3.down,
                Quaternion.identity,
                -pGravity * Time.timeScale * Time.deltaTime + transform.localScale.y,
                LayerMask.GetMask("Ground")
            ).Where(hit => hit.point.y < transform.position.y);
            
            if (!contacts.Any())
                return false;

            pLength = contacts.Min(hit => hit.distance) - transform.localScale.y;
            return true;
        }

        private void GravityProcess() {
            var velocity = _rigid.linearVelocity;

            if(velocity.y <= 0 && _slowFalling)
                velocity.y += GRAVITY_SCALE * Time.deltaTime * _slowFallingPower;
            else 
                velocity.y += GRAVITY_SCALE * Time.deltaTime;
            IsGround = CheckGround(transform.position, velocity.y, out var length);
            
            if (IsGround) {
                velocity.y = 0;
                var pos = transform.position;
                pos.y -= length;
                transform.position = pos;
                _slowFalling = false;
            }
            _rigid.linearVelocity = velocity;
        }
        //==================================================||Unity
        protected virtual void Awake() {
        
            _rigid = GetComponent<Rigidbody>();
            _rigid.useGravity = false;
        }

        protected virtual void Update() {
            GravityProcess();
        }
/*It shows box which checking if player has contacted ground
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