using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Windows
{
    public class SplashWindow : Window
    {
        public ColorSlider primaryColor;
        public ColorSlider secondaryColor;
        public Image primaryColorImage;
        public Image secondayColorImage;
        public Image screenProfile;
        public InputField nameField;
        public InputField fragmentField;
        public InputField powerField;
        public int powerType;
        public Button[] typeButtons;
        public InputField descriptionField;

        private Implement implement;

        public override void Initalize(Implement implement)
        {
            nameField.text = implement.name;
            fragmentField.text = implement.fragment;
            powerField.text = implement.power;
            ChangeType(implement.type);
            descriptionField.text = implement.description;
            primaryColor.Color = implement.PrimaryColor;
            secondaryColor.Color = implement.SecondaryColor;
            screenProfile.color = implement.BaseSprite == null ? Color.clear : Color.white;
            screenProfile.sprite = implement.BaseSprite;
            this.implement = implement;

            //activate
            gameObject.SetActive(true);
        }
        public override Implement Save(Implement implement)
        {
            implement.name = nameField.text;
            implement.fragment = fragmentField.text;
            implement.power = powerField.text;
            implement.type = powerType;
            implement.description = descriptionField.text;
            implement.PrimaryColor = primaryColor.Color;
            implement.SecondaryColor = secondaryColor.Color;
            return implement;
        }
        public void AddProfile()
        {
            Sprite texture = SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Set Profile", Application.persistentDataPath, "png"), Vector2.one, 1);
            SaveSystem.SavePNG(implement.implementList.ImplementPath(implement.index) + "/Base.png", texture.texture);
            screenProfile.color = Color.white;
            screenProfile.sprite = texture;
        }
        public void SplashPreview()
        {
            throw new NotImplementedException();
        }
        public void ChangeType(int i)
        {
            foreach (Button butt in typeButtons)
            {
                ColorBlock colorBlock = butt.colors;
                colorBlock.normalColor = Color.black;
                butt.colors = colorBlock;
            }
            if (typeButtons.Length > i && i >= 0)
            {
                ColorBlock altcolorBlock = typeButtons[i].colors;
                altcolorBlock.normalColor = Color.white;
                typeButtons[i].colors = altcolorBlock;
            }
            powerType = i;
        }
        private void Update()
        {
            primaryColorImage.color = primaryColor.Color;
            secondayColorImage.color = secondaryColor.Color;
        }
    } 
}
