public interface IBattle
{
    bool IsTurn { get; set; }
    float HealthPercent { get; }
    bool CanAttack { get; }
}
