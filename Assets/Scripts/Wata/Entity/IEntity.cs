namespace Entity {
    public interface IEntity {
        public int MaxHp { get; }
        public int Hp { get; }
        public bool IsDead { get; }

        public void GetDamage(int pAmount);
        public void GetHeal(int pAmount);
    }
}