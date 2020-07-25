using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorSlider : MonoBehaviour
{
    public Slider red;
    public Slider green;
    public Slider blue;
    public Image display;
    private Color prevColor;

    public Color Color { get => new Color(red.value, green.value, blue.value); set => SetSliders(value); }
    public event Action<Color> ColorChangedEvent;

    private void SetSliders(Color value)
    {
        red.value = value.r;
        green.value = value.g;
        blue.value = value.b;
    }

    private void Update()
    {
        Color color = Color;
        display.color = color;
        if(color != prevColor)
        {
            ColorChangedEvent?.Invoke(color);
        }
        prevColor = color;
    }
}
