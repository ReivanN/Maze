public interface IDamageable
{
    void TakeDamage(float damage, TrapType trapType, DamageType damageType);
}

public enum DamageType
{
    Normal,
    Ice,
    Fire,
    Poison
}

