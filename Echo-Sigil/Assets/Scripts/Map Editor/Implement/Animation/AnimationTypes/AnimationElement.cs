using System;
using System.Collections.Generic;
using UnityEditor.Animations;
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
        public Animator previewAnimatior;

        public bool Directional
        {
            get => directionalIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = directionalIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                directionalIcon.colors = colors;
            }
        }
        public Button directionalIcon;
        public DirectionalButton directionalButton;

        public bool Variant
        {
            get => variantIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = variantIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                variantIcon.colors = colors;
            }
        }
        public Button variantIcon;

        public bool MultiTile
        {
            get => multiTileIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = multiTileIcon.colors;
                colors.normalColor = value ? Color.white : Color.black;
                multiTileIcon.colors = colors;
            }
        }
        public Button multiTileIcon;
        public InputField numTileField;

        public Transform spritesHolderTransform;
        public GameObject spriteHodlerObject;
        public GameObject newSpriteObject;
        Button newSpriteButton;
        List<SpriteHolder> spriteHolders = new List<SpriteHolder>();

        public event Action<int> DestroyEvent;
        public int index;

        public Transform attachmentHolderTransform;

        public void Initalize(Animation animation)
        {
            GeneralInitialization(animation);

            ResetPreview();
        }

        private void GeneralInitialization(Animation animation)
        {
            nameField.text = animation.Name;
            FPSField.text = animation.Framerate.ToString();
            Directional = animation.GetType() == typeof(DirectionalAnimation);
            Variant = animation.GetType() == typeof(VaraintAnimation);
            MultiTile = animation.GetType() == typeof(MultiTileAnimation);

            if(!(Directional || Variant || MultiTile))
            {
                PopulateSpriteHolder(animation);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void InvokeDestroy()
        {
            DestroyEvent?.Invoke(index);
        }

        private void PopulateSpriteHolder(Animation animation)
        {
            spriteHolders.Clear();
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
            for (int i = 0; i < spriteHolders.Count; i++)
            {
                spriteHolders[i].Index = i;
            }
            newSpriteButton.transform.SetAsLastSibling();
            ResetPreview();
        }

        private void ChangeSprite(int index, Sprite sprite)
        {
            spriteHolders[index].image.sprite = sprite;
            ResetPreview();
        }

        private void RemoveSprite(int index)
        {
            SpriteHolder spriteHolder = spriteHolders[index];
            spriteHolders.Remove(spriteHolder);
            spriteHolder.RemoveEvent -= RemoveSprite;
            spriteHolder.ChangeEvent -= ChangeSprite;
            spriteHolder.MoveEvent -= MoveSprite;
            Destroy(spriteHolder.gameObject);
            ResetPreview();
        }

        private void AddSprite()
        {
            Sprite[] sprites = SaveSystem.LoadPNG(Vector2.one / 2f);
            foreach (Sprite sprite in sprites)
            {
                IstantiateSprite(spriteHolders.Count, sprite);
            }

            newSpriteButton.transform.SetAsLastSibling();
            ResetPreview();
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
            foreach (SpriteHolder spriteHolder in spriteHolders)
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

        public Animation CurrentSpritesAsAnimation()
        {
            Animation animation = new Animation
            {
                Name = nameField.text,
                Framerate = int.Parse(FPSField.text),
                sprites = new string[spriteHolders.Count]
            };
            for (int i = 0; i < spriteHolders.Count; i++)
            {
                Sprite sprite = spriteHolders[i].image.sprite;
                string filePath = ImplementPath + "/" + animation.Name + "/" + i + ".png";
                if (sprite != null)
                {
                    SaveSystem.SavePNG(filePath, sprite.texture);
                }

                animation.sprites[i] = filePath;
            }
            return animation;
        }

        private void ResetPreview()
        {
            AnimatorController controller = new AnimatorController();
            Animation animations = CurrentSpritesAsAnimation();
            AnimationClip motion = animations.GetAnimationClip(typeof(Image));
            controller.AddLayer("Base");
            controller.AddMotion(motion);
            controller.name = nameField.text + "controller";
            previewAnimatior.runtimeAnimatorController = controller;
        }
    }
}
