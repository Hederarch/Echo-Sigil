using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class AnimationElement : Selectable
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

        public Button pulldownButton;
        public Transform subAnimationHolder;
        public RectTransform rectTransform;
        public Vector2 rectHeightMinMax = new Vector2(205,605);
        public VerticalLayoutGroup holder;
        public List<AnimationElement> animationElements = new List<AnimationElement>();
        public bool Extened
        {
            get => rectTransform.offsetMin.y != rectHeightMinMax.x; set
            {
                Vector2 offsetMin = rectTransform.offsetMin;
                offsetMin.y = value ? rectHeightMinMax.y : rectHeightMinMax.x;
                rectTransform.offsetMin = offsetMin;
                holder.spacing += .1f;
                holder.spacing -= .1f;
            }
        }
        public void ToggleExtened()
        {
            Extened = !Extened;
        }

        public bool Directional
        {
            get => directionalIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = directionalIcon.colors;
                Color color = value ? Color.white : Color.black;
                colors.normalColor = color;
                colors.pressedColor = color;
                directionalIcon.colors = colors;
                variantIcon.interactable = !value;
                multiTileIcon.interactable = !value;
            }
        }
        public Button directionalIcon;
        public void ToggleDirectional()
        {
            if (!Variant)
            {
                Directional = !Directional;
                if (Directional)
                {
                    pulldownButton.gameObject.SetActive(true);
                    subAnimationHolder.gameObject.SetActive(true);
                }
                else
                {
                    pulldownButton.gameObject.SetActive(false);
                    subAnimationHolder.gameObject.SetActive(false);
                }
            }
            else
            {
                throw new Exception("Animation cannot be Directional and Varaint");
            }
        }

        public bool Variant
        {
            get => variantIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = variantIcon.colors;
                Color color = value ? Color.white : Color.black;
                colors.normalColor = color;
                colors.pressedColor = color;
                variantIcon.colors = colors;
                directionalIcon.interactable = !value;
                multiTileIcon.interactable = !value;
            }
        }
        public Button variantIcon;
        public void ToggleVariant()
        {
            if (!Directional)
            {
                Variant = !Variant;
                if (Variant)
                {
                    pulldownButton.gameObject.SetActive(true);
                    subAnimationHolder.gameObject.SetActive(true);
                }
                else
                {
                    pulldownButton.gameObject.SetActive(false);
                    subAnimationHolder.gameObject.SetActive(false);
                }
            }
            else
            {
                throw new Exception("Animation cannot be Directional and Varaint");
            }
        }

        public bool MultiTile
        {
            get => multiTileIcon.colors.normalColor == Color.white; set
            {
                ColorBlock colors = multiTileIcon.colors;
                Color color = value ? Color.white : Color.black;
                colors.normalColor = color;
                colors.pressedColor = color;
                multiTileIcon.colors = colors;
                directionalIcon.interactable = !value;
                variantIcon.interactable = !value;
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
                offsetMax.x = -Math.Abs(FPSFieldTransform.rect.width + 10);
                nameFieldTransform.offsetMax = offsetMax;
            }
        }

        public bool Animation => !Directional && !Variant && !MultiTile;

        public Transform spritesHolderTransform;
        public GameObject spriteHodlerObject;
        public GameObject newSpriteObject;
        Button newSpriteButton;
        List<SpriteHolder> spriteHolders = new List<SpriteHolder>();

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
            Directional = true;
            Variant = false;
            ToggleDirectional();
            Variant = true;
            ToggleVariant();
            MultiTile = true;
            ToggleMultiTile();

            if (animation.Type == typeof(Animation))
            {
                PopulateSpriteHolder((Animation)animation);
            }
            else if (animation.Type == typeof(DirectionalAnimation))
            {
                ToggleDirectional();
            }
            else if (animation.Type == typeof(VaraintAnimation))
            {
                ToggleVariant();
            }
            else if (animation.Type == typeof(MultiTileAnimation))
            {
                ToggleMultiTile();

                MultiTileAnimation multiTileAnimation = (MultiTileAnimation)animation;
                PopulateSpriteHolder(multiTileAnimation);
                numTileField.text = multiTileAnimation.tileWidth.ToString();
            }
            else
            {
                Debug.LogError("Type was not assigned");
            }
        }

        public void Destroy()
        {
            DestroyAnimation(transform.parent, containerList, index);
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

        public IAnimation GetAnimation()
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
            IAnimation animation = GetAnimation();
            AnimationClip motion = animation.GetAnimationClip(typeof(Image));
            controller.AddLayer("Base");
            controller.AddMotion(motion);
            controller.name = nameField.text + "controller";
            previewAnimatior.runtimeAnimatorController = controller;
        }

        public static GameObject staticAnimationElementObject;
        public static GameObject staticAddAnimationObject;
        public static GameObject staticAttachmentObject;

        public static void SetStatics(GameObject animationElementObject, GameObject addAnimationObject, GameObject attachmentObject)
        {
            if (animationElementObject != null)
            {
                staticAnimationElementObject = animationElementObject;
            }
            if (addAnimationObject != null)
            {
                staticAddAnimationObject = addAnimationObject;
            }
            if (attachmentObject != null)
            {
                staticAttachmentObject = attachmentObject;
            }
        }

        public List<AnimationElement> containerList;
        public static List<AnimationElement> PopulateTransformWithAnimations(Transform transform, IAnimation[] animations, Implement implement, Dictionary<Ability, int> abilites = null)
        {
            List<AnimationElement> animationElements = new List<AnimationElement>();
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationElement animationElement = Instantiate(staticAnimationElementObject, transform).GetComponent<AnimationElement>();
                animationElements.Add(animationElement);
                animations[i].Index = i;
                animationElement.holder = transform.GetComponent<VerticalLayoutGroup>();
                animationElement.Initalize(animations[i]);
                PopulateAnimationAttachments(i, animationElement, implement, abilites);
                animationElement.containerList = animationElements;
            }
            
            Instantiate(staticAddAnimationObject, transform).GetComponent<Button>().onClick.AddListener(delegate { AddAnimation(transform, animationElements, implement.ImplementPath); });
            return animationElements;

        }
        private static void PopulateAnimationAttachments(int index, AnimationElement animation, Implement implement, Dictionary<Ability, int> abilites = null)
        {
            if (implement.idelIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Idel", index);
            }
            if (implement.attackIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Attack", index);
            }
            if (implement.fidgetIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Fidget", index);
            }
            if (implement.walkIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Walk", index).Directional = true;
            }
            if (abilites != null && false)
            {
                foreach (KeyValuePair<Ability, int> AKey in abilites)
                {
                    if (AKey.Value == index)
                    {
                        InstantiateAnimationAttachment(animation, AKey.Key.name, index);
                        return;
                    }
                }
            }
        }

        private static Attachment InstantiateAnimationAttachment(AnimationElement animation, string name, int index)
        {
            Attachment attachment = Instantiate(staticAttachmentObject, animation.attachmentHolderTransform).GetComponent<Attachment>();
            attachment.Name = name;
            attachment.index = index;
            return attachment;
        }

        private static AnimationElement AddAnimation(Transform transform, List<AnimationElement> animationList, string implementPath)
        {
            Animation animation = new Animation(SaveSystem.LoadPNG(Vector2.one / 2f), animationList.Count, implementPath);
            GameObject gameObject = Instantiate(staticAnimationElementObject, transform);
            gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 2);
            AnimationElement animationElement = gameObject.GetComponent<AnimationElement>();
            animationElement.holder = transform.GetComponent<VerticalLayoutGroup>();
            animationElement.Initalize(animation);
            animationList.Add(animationElement);
            return animationElement;

        }
        private static void DestroyAnimation(Transform transform, List<AnimationElement> animationList, int index)
        {
            animationList.Remove(animationList[index]);
            Destroy(transform.GetChild(index).gameObject);
        }
        public static IAnimation[] SaveAnimations(List<AnimationElement> animationList)
        {
            List<IAnimation> animations = new List<IAnimation>();
            foreach (AnimationElement a in animationList)
            {
                animations.Add(a.GetAnimation());
            }
            return animations.ToArray();
        }
        public static void UnsubsubscribeAnimation(Transform transform, List<AnimationElement> animationList)
        {
            animationList.Clear();
            foreach (Transform t in transform)
            {
                if (t.TryGetComponent(out AnimationElement a))
                {
                    a.DeInitalize();
                }
                else if (t.TryGetComponent(out Button b))
                {
                    b.onClick.RemoveAllListeners();
                }

                Destroy(t.gameObject);
            }
        }
        public static Implement GetAnimationIdexes(List<AnimationElement> animationList, Implement implement)
        {
            foreach (AnimationElement animationElement in animationList)
            {
                foreach (Transform attachmentTransform in animationElement.attachmentHolderTransform)
                {
                    Attachment attachment = attachmentTransform.GetComponent<Attachment>();
                    if (attachment.Name == "Idel")
                    {
                        implement.idelIndex = attachment.index;
                    }
                    else if (attachment.Name == "Attack")
                    {
                        implement.attackIndex = attachment.index;
                    }
                    else if (attachment.Name == "Fidget")
                    {
                        implement.fidgetIndex = attachment.index;
                    }
                    else if (attachment.Name == "Walk")
                    {
                        implement.walkIndex = attachment.index;
                    }

                }
            }
            return implement;
        }
        public static Dictionary<Ability, int> GetAnimationIdexes(List<AnimationElement> animationList, Dictionary<Ability, int> abilites)
        {
            if (abilites != null)
            {
                foreach (AnimationElement animationElement in animationList)
                {
                    foreach (Transform attachmentTransform in animationElement.attachmentHolderTransform)
                    {
                        Attachment attachment = attachmentTransform.GetComponent<Attachment>();

                        foreach (KeyValuePair<Ability, int> AKey in abilites)
                        {
                            if (attachment.Name == AKey.Key.name)
                            {
                                abilites[AKey.Key] = attachment.index;
                                break;
                            }
                        }
                    }
                }
            }
            return abilites;
        }
    }
}
