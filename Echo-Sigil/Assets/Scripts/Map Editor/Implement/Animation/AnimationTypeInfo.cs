using System;
using UnityEngine.UI;
using UnityEngine;

namespace MapEditor.Animations
{
    public class AnimationTypeInfo : MonoBehaviour
    {
        public Color trueColor = Color.white;
        public Color falseColor = Color.black;

        public bool FirstLayer { get; private set; }
        public void SetFirstLayer(bool value,bool isVariant)
        {
            if (!value)
            {
                directionalIcon.interactable = false;
                if(isVariant)
                {
                    variantIcon.interactable = false;
                }
            }
            FirstLayer = value;
        }

        [SerializeField] private Button directionalIcon;
        public bool Directional
        {
            get => directionalIcon.colors.normalColor == trueColor; private set => directionalIcon.colors = BoolColor(value);
        }
        public void ToggleDirectional()
        {
            if (!Variant)
            {
                Directional = !Directional;
                if (Directional)
                {
                    pulldownButton.gameObject.SetActive(true);
                    layoutElement.minHeight = rectHeightMinMax.y;
                }
                else
                {
                    pulldownButton.gameObject.SetActive(false);
                    layoutElement.minHeight = rectHeightMinMax.x;
                }
            }
            else
            {
                throw new Exception("Animation cannot be Directional and Varaint");
            }
        }

        [SerializeField] private Button variantIcon;
        public bool Variant
        {
            get => variantIcon.colors.normalColor == trueColor; private set => variantIcon.colors = BoolColor(value);
        }
        public void ToggleVariant()
        {
            if (!Directional)
            {
                Variant = !Variant;
                if (Variant)
                {
                    pulldownButton.gameObject.SetActive(true);
                    layoutElement.minHeight = rectHeightMinMax.y;
                }
                else
                {
                    pulldownButton.gameObject.SetActive(false);
                    layoutElement.minHeight = rectHeightMinMax.x;
                }
            }
            else
            {
                throw new Exception("Animation cannot be Directional and Varaint");
            }
        }

        [SerializeField] private Button multiTileIcon;
        public bool MultiTile
        {
            get => multiTileIcon.colors.normalColor == trueColor; private set => multiTileIcon.colors = BoolColor(value);
        }
        public void ToggleMultiTile()
        {
            MultiTile = !MultiTile;
            if (MultiTile)
            {
                numTileFieldTransform.gameObject.SetActive(true);
                Vector2 offsetMax = nameFieldTransform.offsetMax;
                offsetMax.x = -Math.Abs(FPSFieldTransform.rect.width + numTileFieldTransform.rect.width + 15);
                nameFieldTransform.offsetMax = offsetMax;
            }
            else
            {
                numTileFieldTransform.gameObject.SetActive(false);
                Vector2 offsetMax = nameFieldTransform.offsetMax;
                offsetMax.x = -Math.Abs(FPSFieldTransform.rect.width + 10);
                nameFieldTransform.offsetMax = offsetMax;
            }
        }
        [SerializeField] private RectTransform numTileFieldTransform;
        [SerializeField] private RectTransform FPSFieldTransform;
        [SerializeField] private RectTransform nameFieldTransform;

        public bool Animation => !Directional && !Variant && !MultiTile;

        private ColorBlock BoolColor(bool value)
        {
            ColorBlock colors = multiTileIcon.colors;
            Color color = value ? trueColor : falseColor;
            colors.normalColor = color;
            colors.pressedColor = color;
            return colors;
        }

        public void FalsifyBools()
        {
            layoutElement.minHeight = rectHeightMinMax.x;
            ColorBlock colorBlock = BoolColor(false);
            directionalIcon.colors = colorBlock;
            variantIcon.colors = colorBlock;
            multiTileIcon.colors = colorBlock;
            pulldownButton.gameObject.SetActive(false);
            numTileFieldTransform.gameObject.SetActive(false);
            Vector2 offsetMax = nameFieldTransform.offsetMax;
            offsetMax.x = -Math.Abs(FPSFieldTransform.rect.width + 10);
            nameFieldTransform.offsetMax = offsetMax;
        }

        [SerializeField] private Vector3 rectHeightMinMax = new Vector3(200, 205, 605);
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private Button pulldownButton;
        public bool Extened
        {
            get => layoutElement.minHeight <= rectHeightMinMax.y; set => layoutElement.minHeight = value ? rectHeightMinMax.y : rectHeightMinMax.z;
        }
        public void ToggleExtened()
        {
            Extened = !Extened;
        }
    }
}
