using UnityEngine;

namespace Game.Player.Combat
{
    public enum AttackType
    {
        Normal,
        Aerial
    }

    [System.Serializable]
    public class AttackConfig
    {
        public float m_cooldown;
        public float m_range;
        public float m_radius;
        public int m_baseDamage;
    }
}
