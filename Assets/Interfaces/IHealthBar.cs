public interface IHealthBar
{
    void UpdateHealthBar(float currentHP, float maxHP, int levelEnemy);
    void ShowHealthBar();
    void HideHealthBar();
}
