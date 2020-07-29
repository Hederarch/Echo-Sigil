using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace mapEditor
{
    public class Animation : MonoBehaviour
    {
        public InputField nameField;
        public InputField FPSField;
        public Image previewImage;
        bool Directional { get => directionalIcon.color == Color.white; set => directionalIcon.color = value ? Color.white : Color.black; }
        public Image directionalIcon;
        bool Variant { get => variantIcon.color == Color.white; set => variantIcon.color = value ? Color.white : Color.black; }
        public Image variantIcon;
        bool MultiTile { get => multiTileIcon.color == Color.white; set => multiTileIcon.color = value ? Color.white : Color.black; }
        public Image multiTileIcon;

        public Transform spritesHolderTrasnform;
        public GameObject spriteHodlerObject;
        public GameObject newSpriteObject;

        public void Initalize(Implement.Animation animation)
        {
            nameField.text = animation.name;
            Directional = animation.GetType() == typeof(Implement.DirectionalAnimation);
            Variant = animation.GetType() == typeof(Implement.VaraintAnimation);
            MultiTile = animation.GetType() == typeof(Implement.MultiTileAnimation);
            UnityEngine.Animation previewAnimator = previewImage.gameObject.AddComponent<UnityEngine.Animation>();
            previewAnimator.clip = animation.GetAnimationClip();
            PopulateSpriteHolder(animation);
        }

        private void PopulateSpriteHolder(Implement.Animation animation)
        {
            
        }
    } 
}
