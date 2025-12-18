namespace Entity.Enemy.Behaviour.Glub {
    public class Heading: AnimationBehaviour {
        private string _afterAnimation;
        
        protected override void OnAfter() {
            _animator.Play(_afterAnimation, 0, 0);
        }
    }
}