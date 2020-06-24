using System;

public interface IBattle
{
    void SetIsTurn();
    event Action EndEvent;
    float GetHealthPercent();
    float GetWillPercent();
    bool GetCanAttack();
}
