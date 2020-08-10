using System;
using UnityEngine;
using UnityEngine.UI;

public class DirectionalButton : MonoBehaviour
{
    public Button[] buttons = new Button[4];
    public event Action<TacticsMovementCamera.Direction> OnClickEvent;

    public void OnClick(int index)
    {
        OnClickEvent?.Invoke((TacticsMovementCamera.Direction)index);
    }
}
