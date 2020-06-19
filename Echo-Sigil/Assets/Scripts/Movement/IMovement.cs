using System;

public interface IMovement
{
    void SetIsTurn();
    event Action EndEvent;
    bool GetCanMove();
}
