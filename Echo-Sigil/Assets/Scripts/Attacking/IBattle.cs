using System;

public interface IBattle
{
    bool IsTurn { get; set; }
    float HealthPercent { get; }
    float WillPercent { get; }
    bool CanAttack { get; }
    event Action EndEvent;
}
