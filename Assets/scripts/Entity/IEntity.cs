namespace Entity {
    public interface IEntity {
        public int MaxHp { get; set; }
        public int Hp { get; set; }

        public void GetDamage(int pAmount);
        public void GetHeal(int pAmount);
    }
}