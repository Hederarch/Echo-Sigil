using System;

public interface IMovement
{
    bool IsTurn { get; set; }
    bool CanMove { get; }
    event Action EndEvent;

}
