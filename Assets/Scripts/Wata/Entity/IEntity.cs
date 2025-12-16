namespace Entity {
    public interface IEntity {
        public int MaxHp { get; }
        public int Hp { get; }
        public bool IsDead { get; }
        public int PoisonStack { get; }

        public void GetDamage(int pAmount);
        public void GetHeal(int pAmount);
        public void AddPoisonStack(int pAmount);
        public void RemovePoisonStack(int pAmount);
    }
}