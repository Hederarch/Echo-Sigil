using System;
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

        public override void Initalize(Implement implement, Unit unit = null)
        {
            Implement.SplashInfo splashInfo = implement.splashInfo;
            nameField.text = splashInfo.name;
            fragmentField.text = splashInfo.fragment;
            powerField.text = splashInfo.power;
            ChangeType(splashInfo.type);
            descriptionField.text = splashInfo.description;
            primaryColor.Color = splashInfo.PrimaryColor;
            secondaryColor.Color = splashInfo.SecondaryColor;
            screenProfile.color = implement.baseSprite == null ? Color.clear : Color.white;
            screenProfile.sprite = implement.baseSprite;
            this.implement = implement;

            //activate
            gameObject.SetActive(true);
        }
        public override Implement Save(Implement implement, Unit unit = null)
        {
            implement.splashInfo = new Implement.SplashInfo(nameField.text, fragmentField.text, powerField.text, powerType, descriptionField.text, primaryColor.Color, secondaryColor.Color);
            return implement;
        }
        public void AddProfile()
        {
            Sprite texture = SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Set Profile", Application.persistentDataPath, "png"), Vector2.one, 1);
            implement.baseSprite = texture;
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
