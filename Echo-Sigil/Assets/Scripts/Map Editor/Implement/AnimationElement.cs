using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace mapEditor.animations
{
    public class AnimationElement : MonoBehaviour
    {
        /// <summary>
        /// Path to the implements animation folder. 
        /// </summary>
        public static string ImplementPath;
        public InputField nameField;
        public InputField FPSField;
        public UnityEngine.Animation previewAnimatior;
        bool Directional
        {
            get => directionalIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = directionalIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                directionalIcon.colors = colors;
            }
        }
        public Button directionalIcon;
        bool Variant
        {
            get => variantIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = variantIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                variantIcon.colors = colors;
            }
        }
        public Button variantIcon;
        bool MultiTile
        {
            get => multiTileIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = multiTileIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                multiTileIcon.colors = colors;
            }
        }
        public Button multiTileIcon;

        public Transform spritesHolderTransform;
        public GameObject spriteHodlerObject;
        public GameObject newSpriteObject;
        Button newSpriteButton;
        List<SpriteHolder> spriteHolders = new List<SpriteHolder>();

        public event Action<int> DestroyEvent;
        public int index;

        public void Initalize(Animation animation)
        {
            nameField.text = animation.name;
            FPSField.text = animation.framerate.ToString();
            Directional = animation.GetType() == typeof(DirectionalAnimation);
            Variant = animation.GetType() == typeof(VaraintAnimation);
            MultiTile = animation.GetType() == typeof(MultiTileAnimation);

            spriteHolders.Clear();
            PopulateSpriteHolder(animation);

            previewAnimatior.clip = animation.GetAnimationClip();
        }

        public void InvokeDestroy()
        {
            DestroyEvent?.Invoke(index);
        }

        private void PopulateSpriteHolder(Animation animation)
        {
            int i = 0;
            foreach (Sprite sprite in animation)
            {
                IstantiateSprite(i, sprite);
                i++;
            }

            newSpriteButton = Instantiate(newSpriteObject, spritesHolderTransform).GetComponent<Button>();
            newSpriteButton.onClick.AddListener(call: AddSprite);
        }

        private void MoveSprite(int index, bool right)
        {
            SpriteHolder moveing = spriteHolders[index];
            int newIndex = right ? index - 1 : index + 1;
            moveing.Index = newIndex;
            moveing.transform.SetSiblingIndex(newIndex);
            spriteHolders.Remove(moveing);
            spriteHolders.Insert(newIndex, moveing);
            for(int i = 0; i < spriteHolders.Count; i++)
            {
                spriteHolders[i].Index = i;
            }
            newSpriteButton.transform.SetAsLastSibling();
            previewAnimatior.clip = Save().GetAnimationClip();
        }

        private void ChangeSprite(int index, Sprite sprite)
        {
            spriteHolders[index].image.sprite = sprite;
            previewAnimatior.clip = Save().GetAnimationClip();
        }

        private void RemoveSprite(int index)
        {
            SpriteHolder spriteHolder = spriteHolders[index];
            spriteHolders.Remove(spriteHolder);
            spriteHolder.RemoveEvent -= RemoveSprite;
            spriteHolder.ChangeEvent -= ChangeSprite;
            spriteHolder.MoveEvent -= MoveSprite;
            Destroy(spriteHolder.gameObject);
            previewAnimatior.clip = Save().GetAnimationClip();
        }

        private void AddSprite()
        {
            IstantiateSprite(spriteHolders.Count, SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Add Sprite",Directory.GetCurrentDirectory(),"png"),Vector2.one/2f));
            newSpriteButton.transform.SetAsLastSibling();
            previewAnimatior.clip = Save().GetAnimationClip();
        }

        private void IstantiateSprite(int index, Sprite sprite)
        {
            SpriteHolder spriteHolder = Instantiate(spriteHodlerObject, spritesHolderTransform).GetComponent<SpriteHolder>();
            spriteHolder.Index = index;
            spriteHolder.image.sprite = sprite;
            spriteHolder.RemoveEvent += RemoveSprite;
            spriteHolder.ChangeEvent += ChangeSprite;
            spriteHolder.MoveEvent += MoveSprite;
            spriteHolders.Add(spriteHolder);
        }

        internal void DeInitalize()
        {
            DestroyEvent = null;
            foreach(SpriteHolder spriteHolder in spriteHolders)
            {
                spriteHolder.RemoveEvent -= RemoveSprite;
                spriteHolder.ChangeEvent -= ChangeSprite;
                spriteHolder.MoveEvent -= MoveSprite;
                Destroy(spriteHolder.gameObject);
            }
            Transform buttonTransform = spritesHolderTransform.GetChild(spritesHolderTransform.childCount - 1);
            buttonTransform.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(buttonTransform.gameObject);
        }

        public Animation Save()
        {
            Animation sprites = new Animation
            {
                name = nameField.text,
                framerate = int.Parse(FPSField.text),
                sprites = new string[spriteHolders.Count]
            };
            for (int i = 0; i < spriteHolders.Count; i++)
            {
                Sprite sprite = spriteHolders[i].image.sprite;
                string filePath = ImplementPath + "/" + sprites.name + "/" + i + ".png";
                if (sprite != null)
                {
                    SaveSystem.SavePNG(filePath, sprite.texture);
                }

                sprites.sprites[i] = filePath;
            }
            return sprites;
        }
    }
}
