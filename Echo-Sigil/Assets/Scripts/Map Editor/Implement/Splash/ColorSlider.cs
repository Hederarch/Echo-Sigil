using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class ColorSlider : MonoBehaviour
    {
        public Slider red;
        public Slider green;
        public Slider blue;
        public Image display;

        public Color Color { get => new Color(red.value, green.value, blue.value); set => SetSliders(value); }

        private void SetSliders(Color value)
        {
            red.value = value.r;
            green.value = value.g;
            blue.value = value.b;
        }

        private void Update()
        {
            display.color = Color;
        }
    } 
}
