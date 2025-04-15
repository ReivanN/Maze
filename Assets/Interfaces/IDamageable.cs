public interface IDamageable
{
    void TakeDamage(float damage, TrapType trapType, DamageType damageType);
}

public enum DamageType
{
    Normal = 1,
    Ice = 1 << 1,
    Fire =  1 << 2,
    Poison = 1 << 3
}

