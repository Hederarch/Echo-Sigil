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
        public RectTransform nameFieldTransform;
        public InputField FPSField;
        public RectTransform FPSFieldTransform;
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
        public RectTransform numTileFieldTransform;
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
                offsetMax.x = -Math.Abs (FPSFieldTransform.rect.width + 10);
                nameFieldTransform.offsetMax = offsetMax;
            }
        }

        public bool Animation => !Directional && !Variant && !MultiTile;

        public Transform spritesHolderTransform;
        public GameObject spriteHodlerObject;
        public GameObject newSpriteObject;
        Button newSpriteButton;
        List<SpriteHolder> spriteHolders = new List<SpriteHolder>();

        public event Action<int> DestroyEvent;
        public int index;

        public Transform attachmentHolderTransform;

        public void Initalize(IAnimation animation)
        {
            GeneralInitialization(animation);
            TypeSpesficInitialization(animation);

            ResetPreview();
        }

        private void GeneralInitialization(IAnimation animation)
        {
            nameField.text = animation.Name;
            FPSField.text = animation.Framerate.ToString();
            index = animation.Index;
        }

        private void TypeSpesficInitialization(IAnimation animation)
        {
            //Falseify all bools
            Directional = false;
            Variant = false;
            MultiTile = false;

            //unset MultiTile
            numTileFieldTransform.gameObject.SetActive(false);
            Vector2 offsetMax = nameFieldTransform.offsetMax;
            offsetMax.x = -Math.Abs(FPSFieldTransform.rect.width + 10);
            nameFieldTransform.offsetMax = offsetMax;

            if (animation.Type == typeof(Animation))
            {
                PopulateSpriteHolder((Animation)animation);
            }
            else if (animation.Type == typeof(DirectionalAnimation))
            {
                Directional = true;
            }
            else if (animation.Type == typeof(VaraintAnimation))
            {
                Variant = true;
            }
            else if (animation.Type == typeof(MultiTileAnimation))
            {
                MultiTile = true;
                MultiTileAnimation multiTileAnimation = (MultiTileAnimation)animation;
                PopulateSpriteHolder(multiTileAnimation);
                numTileFieldTransform.gameObject.SetActive(true);
                offsetMax.x = -Math.Abs(numTileFieldTransform.rect.width + FPSFieldTransform.rect.width + 15);
                nameFieldTransform.offsetMax = offsetMax;
                numTileField.text = multiTileAnimation.tileWidth.ToString();
            }
            else
            {
                Debug.LogError("Type was not assigned");
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

        private void PopulateSpriteHolder(MultiTileAnimation animation)
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

        public IAnimation CurrentSpritesAsAnimation()
        {
            IAnimation animation = null;
            if (Variant || Directional)
            {
                throw new NotImplementedException();
            }
            else if (Animation || MultiTile)
            {
                string[] sprites = new string[spriteHolders.Count];
                for (int i = 0; i < spriteHolders.Count; i++)
                {
                    Sprite sprite = spriteHolders[i].image.sprite;
                    string filePath = ImplementPath + "/" + nameField.text + "/" + i + ".png";
                    if (sprite != null)
                    {
                        SaveSystem.SavePNG(filePath, sprite.texture);
                    }

                    sprites[i] = filePath;
                }

                if (Animation)
                {
                    animation = new Animation
                    {
                        Name = nameField.text,
                        Framerate = int.Parse(FPSField.text),
                        sprites = sprites
                    };
                }
                else if (MultiTile)
                {
                    animation = new MultiTileAnimation
                    {
                        Name = nameField.text,
                        Framerate = int.Parse(FPSField.text),
                        sprites = sprites,
                        tileWidth = int.Parse(numTileField.text)
                        
                    };
                }
                
            } 

            return animation;
        }

        private void ResetPreview()
        {
            AnimatorController controller = new AnimatorController();
            IAnimation animation = CurrentSpritesAsAnimation();
            AnimationClip motion = animation.GetAnimationClip(typeof(Image));
            controller.AddLayer("Base");
            controller.AddMotion(motion);
            controller.name = nameField.text + "controller";
            previewAnimatior.runtimeAnimatorController = controller;
        }
    }
}
