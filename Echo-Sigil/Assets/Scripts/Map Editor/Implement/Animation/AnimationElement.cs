using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class AnimationElement : Selectable
    {

        [SerializeField] private InputField nameField;
        public string Name { get => nameField.text; set => nameField.text = value; }

        [SerializeField] private InputField FPSField;
        public int FPS
        {
            get
            {
                int value = int.Parse(FPSField.text);
                return value > 0 ? value : 12;
            }
            set => FPSField.text = value > 0 ? value.ToString() : "12";
        }

        [SerializeField] private InputField numTileField;
        public int NumTile
        {
            get
            {
                int value = int.Parse(numTileField.text);
                return value > 0 ? value : 1;
            }
            set => numTileField.text = value > 0 ? value.ToString() : "1";
        }

        [SerializeField] private Animator previewAnimatior;
        [SerializeField] private Transform subAnimationHolder;

        public AnimationTypeInfo animationTypeInfo;

        [SerializeField] private Transform spritesHolderTransform;
        [SerializeField] private GameObject spriteHodlerObject;
        [SerializeField] private GameObject newSpriteObject;

        Button newSpriteButton;
        List<SpriteHolder> spriteHolders = new List<SpriteHolder>();

        public Transform attachmentHolderTransform;

        public Button DeleteButton;
        public int index;

        public List<AnimationElement> containerList;
        public List<AnimationElement> subAnimationElements;

        public void Initalize(IAnimation animation, bool firstLayer)
        {
            GeneralInitialization(animation,firstLayer);
            TypeSpesficInitialization(animation);

            ResetPreview();
        }

        private void GeneralInitialization(IAnimation animation, bool firstLayer)
        {
            Name = animation.Name;
            FPSField.text = animation.Framerate.ToString();
            index = animation.Index;
            animationTypeInfo.SetFirstLayer(firstLayer, animation.Type == typeof(VaraintAnimation));
        }

        private void TypeSpesficInitialization(IAnimation animation)
        {
            animationTypeInfo.FalsifyBools();

            if (animation.Type == typeof(Animation))
            {
                PopulateSpriteHolder((Animation)animation);
            }
            else if (animation.Type == typeof(DirectionalAnimation))
            {
                animationTypeInfo.ToggleDirectional();
                DirectionalAnimation directionalAnimation = (DirectionalAnimation)animation;
                subAnimationElements = PopulateTransformWithAnimations(subAnimationHolder, directionalAnimation.animations, false, directionalAnimation.animationIndexes);
            }
            else if (animation.Type == typeof(VaraintAnimation))
            {
                animationTypeInfo.ToggleVariant();
                VaraintAnimation varaintAnimation = (VaraintAnimation)animation;
                subAnimationElements = PopulateTransformWithAnimations(subAnimationHolder, varaintAnimation.animations, false);
            }
            else if (animation.Type == typeof(MultiTileAnimation))
            {
                animationTypeInfo.ToggleMultiTile();

                MultiTileAnimation multiTileAnimation = (MultiTileAnimation)animation;
                PopulateSpriteHolder(multiTileAnimation);
                NumTile = multiTileAnimation.tileWidth;
            }
            else
            {
                Debug.LogError("Type was not assigned");
            }
        }

        private void PopulateSpriteHolder(Animation animation)
        {
            DeInitalizeSpriteHolders();
            int i = 0;
            foreach (Sprite sprite in animation.sprites)
            {
                IstantiateSpriteHolder(i, sprite);
                i++;
            }

            newSpriteButton = Instantiate(newSpriteObject, spritesHolderTransform).GetComponent<Button>();
            newSpriteButton.onClick.AddListener(call: AddSprite);
        }

        private void PopulateSpriteHolder(MultiTileAnimation animation)
        {
            DeInitalizeSpriteHolders();
            int i = 0;
            foreach (Sprite sprite in animation.sprites)
            {
                IstantiateSpriteHolder(i, sprite);
                i++;
            }

            newSpriteButton = Instantiate(newSpriteObject, spritesHolderTransform).GetComponent<Button>();
            newSpriteButton.onClick.AddListener(call: AddSprite);
        }

        private void MoveSprite(int index, bool right)
        {
            SpriteHolder moveing = spriteHolders[index];
            int newIndex = right ? index + 1 : index - 1;
            moveing.Index = newIndex;
            moveing.transform.SetSiblingIndex(newIndex);
            spriteHolders.Remove(moveing);
            spriteHolders.Insert(newIndex, moveing);
            ReCaluculateSpriteHolders();
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
            DeInializeSpriteHolder(spriteHolder);
            ReCaluculateSpriteHolders();
        }

        private void AddSprite()
        {
            Sprite[] sprites = SaveSystem.LoadPNG(Vector2.one / 2f);
            foreach (Sprite sprite in sprites)
            {
                IstantiateSpriteHolder(spriteHolders.Count, sprite);
            }
            ReCaluculateSpriteHolders();
        }

        private void ReCaluculateSpriteHolders()
        {
            IAnimation animation = GetAnimation();
            if (animation.Type == typeof(Animation))
            {
                Animation animation1 = (Animation)animation;
                PopulateSpriteHolder(animation1);
            }
            else if (animation.Type == typeof(MultiTileAnimation))
            {
                MultiTileAnimation animation1 = (MultiTileAnimation)animation;
                PopulateSpriteHolder(animation1);
            }
            ResetPreview();
        }

        private void IstantiateSpriteHolder(int index, Sprite sprite)
        {
            SpriteHolder spriteHolder = Instantiate(spriteHodlerObject, spritesHolderTransform).GetComponent<SpriteHolder>();
            spriteHolder.Index = index;
            spriteHolder.image.sprite = sprite;
            spriteHolder.RemoveEvent += RemoveSprite;
            spriteHolder.ChangeEvent += ChangeSprite;
            spriteHolder.MoveEvent += MoveSprite;
            spriteHolders.Add(spriteHolder);
        }

        internal void DeInitalizeSpriteHolders()
        {
            foreach (SpriteHolder spriteHolder in spriteHolders)
            {
                DeInializeSpriteHolder(spriteHolder);
            }
            spriteHolders.Clear();
            if (newSpriteButton != null)
            {
                newSpriteButton.onClick.RemoveAllListeners();
                Destroy(newSpriteButton.gameObject);
                newSpriteButton = null;
            }
        }

        private void DeInializeSpriteHolder(SpriteHolder spriteHolder)
        {
            spriteHolder.RemoveEvent -= RemoveSprite;
            spriteHolder.ChangeEvent -= ChangeSprite;
            spriteHolder.MoveEvent -= MoveSprite;
            Destroy(spriteHolder.gameObject);
        }

        public IAnimation GetAnimation(IAnimation parent = null)
        {
            IAnimation animation = null;
            if (animationTypeInfo.Variant || animationTypeInfo.Directional)
            {
                List<IAnimation> animations = new List<IAnimation>();
                foreach (AnimationElement animationElement in subAnimationElements)
                {
                    IAnimation item = animationElement.GetAnimation(parent);
                    animations.Add(item);
                }

                if (animationTypeInfo.Variant)
                {
                    animation = new VaraintAnimation()
                    {
                        Name = Name,
                        Framerate = int.Parse(FPSField.text),
                        animations = animations.ToArray()
                    };
                }
                else if (animationTypeInfo.Directional)
                {
                    animation = new DirectionalAnimation()
                    {
                        Name = Name,
                        Framerate = int.Parse(FPSField.text),
                        animations = animations.ToArray(),
                        animationIndexes = (DirectionalAnimation.AnimationIndexes)GetAnimationIdexes(subAnimationElements, new DirectionalAnimation.AnimationIndexes())
                    };
                }
            }
            else if (animationTypeInfo.Animation || animationTypeInfo.MultiTile)
            {
                string name = Name;
                if(parent != null)
                {
                    name = parent.Name + name;
                }
                Sprite[] sprites = new Sprite[spriteHolders.Count];
                for (int i = 0; i < spriteHolders.Count; i++)
                {
                    sprites[i] = spriteHolders[i].image.sprite;
                }

                if (animationTypeInfo.Animation)
                {
                    animation = new Animation
                    {
                        Name = name,
                        Framerate = FPS,
                        sprites = sprites
                    };
                }
                else if (animationTypeInfo.MultiTile)
                {
                    animation = new MultiTileAnimation
                    {
                        Name = name,
                        Framerate = FPS,
                        sprites = sprites,
                        tileWidth = NumTile

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
            controller.name = Name + "controller";
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
        public static List<AnimationElement> PopulateTransformWithAnimations(Transform transform, IAnimation[] animations, bool firstLayer = true, IAnimationIndexes indexes = null)
        {
            foreach (Transform toDealte in transform)
            {
                Destroy(toDealte.gameObject);
            }
            List<AnimationElement> animationElements = new List<AnimationElement>();
            GameObject addAnimationObject = Instantiate(staticAddAnimationObject, transform);
            addAnimationObject.GetComponent<Button>().onClick.AddListener(delegate { AddAnimation(transform, ref animationElements, firstLayer, indexes); });

            if (animations != null)
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    AnimationElement animationElement = Instantiate(staticAnimationElementObject, transform).GetComponent<AnimationElement>();
                    addAnimationObject.transform.SetAsLastSibling();
                    animationElements.Add(animationElement);
                    animationElement.containerList = animationElements;

                    animations[i].Index = i;
                    animationElement.Initalize(animations[i], firstLayer);

                    animationElement.DeleteButton.onClick.AddListener(delegate { DestroyAnimation(transform, animationElements, animationElement.index); });

                    if (indexes != null)
                    {
                        PopulateAnimationAttachments(i, animationElement, indexes);
                    }

                }
            }

            return animationElements;

        }
        private static void PopulateAnimationAttachments(int index, AnimationElement animation, IAnimationIndexes indexes)
        {
            foreach (AnimationIndexPair animationIndexPair in indexes)
            {
                if (animationIndexPair == index)
                {
                    AnimationAttachment animationAttachment = Instantiate(staticAttachmentObject, animation.attachmentHolderTransform).GetComponent<AnimationAttachment>();
                    animationAttachment.animationIndex = animationIndexPair;
                    animationAttachment.animationElements = animation.containerList;
                }
            }
        }
        private static AnimationElement AddAnimation(Transform transform, ref List<AnimationElement> animationList, bool firstLayer, IAnimationIndexes indexes = null)
        {
            Animation animation = new Animation(SaveSystem.LoadPNG(Vector2.one / 2f), animationList.Count);
            GameObject gameObject = Instantiate(staticAnimationElementObject, transform);
            gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 2);
            AnimationElement animationElement = gameObject.GetComponent<AnimationElement>();
            animationElement.Initalize(animation, firstLayer);
            animationList.Add(animationElement);
            PopulateAnimationAttachments(animationList.Count -1, animationElement, indexes);
            return animationElement;

        }
        private static void DestroyAnimation(Transform transform, List<AnimationElement> animationList, int index)
        {
            AnimationElement item = animationList[index];
            item.DeleteButton.onClick.RemoveAllListeners();
            animationList.Remove(item);
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
                    a.DeInitalizeSpriteHolders();
                }
                else if (t.TryGetComponent(out Button b))
                {
                    b.onClick.RemoveAllListeners();
                }

                Destroy(t.gameObject);
            }
        }
        public static IAnimationIndexes GetAnimationIdexes(List<AnimationElement> animationList, IAnimationIndexes indexes)
        {
            foreach (AnimationElement animationElement in animationList)
            {
                foreach (Transform attachmentTransform in animationElement.attachmentHolderTransform)
                {
                    AnimationAttachment attachment = attachmentTransform.GetComponent<AnimationAttachment>();
                    AnimationIndexPair animationIndexPair = indexes[attachment.Label];
                    if (animationIndexPair.label != "NULL")
                    {
                        indexes[attachment.Label] = attachment.animationIndex;
                    }

                }
            }
            return indexes;
        }
    }
}
