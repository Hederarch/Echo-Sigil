using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MapEditor.Animations
{
    public interface IAnimation : IComparable<IAnimation>
    {
        string Name { get; set; }
        int Framerate { get; set; }

        int Index { get; set; }
        Type Type { get; }

        AnimationClip GetAnimationClip(Type type);

        AnimatorState GetAnimatorState(Type type);

        AnimatorStateMachine GetAnimatorStateMachine(Type type);

    }

    [Serializable]
    public struct Animation : IAnimation, IEnumerable<Sprite>, IEnumerator<Sprite>
    {
        public string name;
        public string Name { get => name; set => name = value; }
        public int framerate;
        public int Framerate { get => framerate; set => framerate = value; }

        public string[] sprites;
        int curIndex;

        public Sprite Current
        {
            get
            {
                try
                {
                    return SaveSystem.LoadPNG(sprites[curIndex], Vector2.one / 2f);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public int index;
        public int Index { get => index; set => index = value; }

        public Type Type => typeof(Animation);

        public Animation(Sprite[] sprites, int index, string implementPath)
        {
            name = "New";
            framerate = 12;
            this.sprites = new string[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                string filePath = implementPath + "/" + name + "/" + i + ".png";
                if (sprites[i] != null)
                {
                    SaveSystem.SavePNG(filePath, sprites[i].texture);
                }

                this.sprites[i] = filePath;
            }
            this.index = index;
            curIndex = -1;
        }

        public AnimationClip GetAnimationClip(Type type)
        {
            AnimationClip clip = new AnimationClip
            {
                name = name,
                frameRate = framerate > 0 ? framerate : 12,
            };

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = type,
                path = "",
                propertyName = "m_Sprite"
            };

            int spritesLength = sprites != null ? sprites.Length : 0;
            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[spritesLength];
            for (int i = 0; i < spritesLength; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = (float)i / (float)framerate,
                    value = SaveSystem.LoadPNG(sprites[i], new Vector2(.5f, 0))
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            if (!clip.isLooping)
            {
                Debug.LogError("Animation " + name + " not set to loop");
            }

            return clip;
        }

        public AnimatorStateMachine GetAnimatorStateMachine(Type type)
        {
            AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
            AnimatorState state = GetAnimatorState(type);
            state.AddExitTransition();
            animatorStateMachine.AddState(state, Vector3.one);
            animatorStateMachine.name = name;

            return animatorStateMachine;
        }

        public AnimatorState GetAnimatorState(Type type)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.motion = GetAnimationClip(type);
            animatorState.name = name;

            return animatorState;
        }

        IEnumerator<Sprite> IEnumerable<Sprite>.GetEnumerator()
        {
            Reset();
            return this;
        }

        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }

        public bool MoveNext()
        {
            curIndex++;
            return curIndex < sprites.Length;
        }

        public void Reset()
        {
            curIndex = -1;
        }

        public void Dispose()
        {

        }

        public int CompareTo(IAnimation other)
        {
            return Index.CompareTo(other.Index);
        }

        public static void SerilizeAnimationArrays(IAnimation[] animations, ref Animation[] saveAnimations, ref DirectionalAnimation[] saveDirectionalAnimations, ref VaraintAnimation[] saveVaraintAnimations, ref MultiTileAnimation[] saveMultiTileAnimations)
        {
            saveAnimations = new Animations.Animation[0];
            saveDirectionalAnimations = new DirectionalAnimation[0];
            saveVaraintAnimations = new VaraintAnimation[0];
            saveMultiTileAnimations = new MultiTileAnimation[0];

            for (int i = 0; i < animations.Length; i++)
            {
                IAnimation animation = animations[i];
                animation.Index = i;

                if (animation.Type == typeof(Animation))
                {
                    Animations.Animation[] animationArray = new Animation[saveAnimations.Length + 1];
                    saveAnimations.CopyTo(animationArray, 0);
                    animationArray[saveAnimations.Length] = (Animation)animation;
                    saveAnimations = animationArray;
                }
                else if (animation.Type == typeof(DirectionalAnimation))
                {
                    DirectionalAnimation[] directionalAnimationArray = new DirectionalAnimation[saveDirectionalAnimations.Length + 1];
                    saveDirectionalAnimations.CopyTo(directionalAnimationArray, 0);
                    directionalAnimationArray[saveDirectionalAnimations.Length] = (DirectionalAnimation)animation;
                    saveDirectionalAnimations = directionalAnimationArray;
                }
                else if (animation.Type == typeof(VaraintAnimation))
                {
                    VaraintAnimation[] variantAnimationArray = new VaraintAnimation[saveVaraintAnimations.Length + 1];
                    saveAnimations.CopyTo(variantAnimationArray, 0);
                    variantAnimationArray[saveVaraintAnimations.Length] = (VaraintAnimation)animation;
                    saveVaraintAnimations = variantAnimationArray;
                }
                else if (animation.Type == typeof(MultiTileAnimation))
                {
                    MultiTileAnimation[] multiTileAnimationArray = new MultiTileAnimation[saveMultiTileAnimations.Length + 1];
                    saveMultiTileAnimations.CopyTo(multiTileAnimationArray, 0);
                    multiTileAnimationArray[saveMultiTileAnimations.Length] = (MultiTileAnimation)animation;
                    saveMultiTileAnimations = multiTileAnimationArray;
                }
                else
                {
                    Debug.LogError("Type was not assigned");
                }
            }
        }

        public static IAnimation[] DeserializeAnimationArray(Animation[] saveAnimations, DirectionalAnimation[] saveDirectionalAnimations, VaraintAnimation[] saveVaraintAnimations, MultiTileAnimation[] saveMultiTileAnimations)
        {
            List<IAnimation> listOfAnimations = new List<IAnimation>();
            if (saveAnimations != null)
            {
                foreach (Animations.Animation animation in saveAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveDirectionalAnimations != null)
            {
                foreach (DirectionalAnimation animation in saveDirectionalAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveVaraintAnimations != null)
            {
                foreach (VaraintAnimation animation in saveVaraintAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveMultiTileAnimations != null)
            {
                foreach (MultiTileAnimation animation in saveMultiTileAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            listOfAnimations.Sort();
            return listOfAnimations.ToArray();
        }

    }

    [Serializable]
    public struct DirectionalAnimation : IAnimation, ISerializationCallbackReceiver
    {
        public IAnimation[] animations;

        [SerializeField]
        Animation[] saveAnimations;
        [SerializeField]
        VaraintAnimation[] saveVaraintAnimations;
        [SerializeField]
        MultiTileAnimation[] saveMultiTileAnimations;

        public string name;
        public string Name { get => name; set => name = value; }
        public int framerate;
        public int Framerate { get => framerate; set => framerate = value; }

        public int index;
        public int Index { get => index; set => index = value; }

        public Type Type => typeof(DirectionalAnimation);

        public DirectionalAnimation(IAnimation[] animations, int index)
        {
            name = "New";
            framerate = 12;
            this.animations = animations;
            this.index = index;
            animationIndexes = new AnimationIndexes();
            saveAnimations = new Animation[0];
            saveMultiTileAnimations = new MultiTileAnimation[0];
            saveVaraintAnimations = new VaraintAnimation[0];
        }

        public AnimationClip GetAnimationClip(Type type)
        {
            AnimationClip animationClip;
            if (animations != null && animations.Length > 0)
            {
                animationClip = animations[0].GetAnimationClip(type);
            }
            else
            {
                animationClip = new Animation().GetAnimationClip(type);
            }

            animationClip.name = name;
            animationClip.frameRate = framerate;
            return animationClip;
        }

        public AnimatorStateMachine GetAnimatorStateMachine(Type type)
        {
            AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
            for (int i = 0; i < 4; i++)
            {
                AnimatorState animatorState = new AnimatorState();
                animatorState.motion = animations[i].GetAnimationClip(type);
                animatorStateMachine.AddState(animatorState, new Vector3(1, i, 0));
                animatorState.AddExitTransition();
            }
            return animatorStateMachine;
        }

        public AnimatorState GetAnimatorState(Type type)
        {
            throw new Exception("Directional Animation is not collapable");
        }

        public int CompareTo(IAnimation other)
        {
            return Index.CompareTo(other.Index);
        }

        public void OnBeforeSerialize()
        {
            IAnimation[] animations = new IAnimation[4];
            if (animationIndexes.Clamp(this.animations))
            {
                animations[0] = this.animations[animationIndexes["Up"]];
                animations[1] = this.animations[animationIndexes["Down"]];
                animations[2] = this.animations[animationIndexes["Left"]];
                animations[3] = this.animations[animationIndexes["Right"]];
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    animations[i] = new Animation();
                }
            }

            saveAnimations = new Animation[0];
            saveVaraintAnimations = new VaraintAnimation[0];
            saveMultiTileAnimations = new MultiTileAnimation[0];

            for (int i = 0; i < animations.Length; i++)
            {
                IAnimation animation = animations[i];
                animation.Index = i;

                if (animation.Type == typeof(Animation))
                {
                    Animation[] animationArray = new Animation[saveAnimations.Length + 1];
                    saveAnimations.CopyTo(animationArray, 0);
                    animationArray[saveAnimations.Length] = (Animation)animation;
                    saveAnimations = animationArray;
                }
                else if (animation.Type == typeof(VaraintAnimation))
                {
                    VaraintAnimation[] variantAnimationArray = new VaraintAnimation[saveVaraintAnimations.Length + 1];
                    saveAnimations.CopyTo(variantAnimationArray, 0);
                    variantAnimationArray[saveVaraintAnimations.Length] = (VaraintAnimation)animation;
                    saveVaraintAnimations = variantAnimationArray;
                }
                else if (animation.Type == typeof(MultiTileAnimation))
                {
                    MultiTileAnimation[] multiTileAnimationArray = new MultiTileAnimation[saveMultiTileAnimations.Length + 1];
                    saveMultiTileAnimations.CopyTo(multiTileAnimationArray, 0);
                    multiTileAnimationArray[saveMultiTileAnimations.Length] = (MultiTileAnimation)animation;
                    saveMultiTileAnimations = multiTileAnimationArray;
                }
                else
                {
                    Debug.LogError("Type was not assigned");
                }
            }
        }

        public void OnAfterDeserialize()
        {
            List<IAnimation> listOfAnimations = new List<IAnimation>();
            if (saveAnimations != null)
            {
                foreach (Animation animation in saveAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveVaraintAnimations != null)
            {
                foreach (VaraintAnimation animation in saveVaraintAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveMultiTileAnimations != null)
            {
                foreach (MultiTileAnimation animation in saveMultiTileAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            listOfAnimations.Sort();
            animations = listOfAnimations.ToArray();

            animationIndexes = new AnimationIndexes();
        }

        public class AnimationIndexes : IAnimationIndexes
        {
            public AnimationIndexPair up = new AnimationIndexPair(0, "Up");
            public AnimationIndexPair down = new AnimationIndexPair(1, "Down");
            public AnimationIndexPair left = new AnimationIndexPair(2, "Left");
            public AnimationIndexPair right = new AnimationIndexPair(3, "Right");
            private int index = -1;

            public AnimationIndexPair this[string s]
            {
                get
                {
                    switch (s)
                    {
                        case "Up":
                            return up;
                        case "Down":
                            return down;
                        case "Left":
                            return left;
                        case "Right":
                            return right;
                    }
                    return new AnimationIndexPair(0, "NULL");
                }
                set
                {
                    switch (s)
                    {
                        case "Up":
                            up = value;
                            break;
                        case "Down":
                            down = value;
                            break;
                        case "Left":
                            left = value;
                            break;
                        case "Right":
                            right = value;
                            break;
                    }
                }
            }

            public AnimationIndexPair Current
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return up;
                        case 1:
                            return down;
                        case 2:
                            return left;
                        case 3:
                            return right;
                    }
                    throw new IndexOutOfRangeException();
                }
            }

            object IEnumerator.Current => Current;

            public bool Clamp(IAnimation[] animations)
            {
                if (!up.Clamp(animations))
                {
                    return false;
                }
                if (!down.Clamp(animations))
                {
                    return false;
                }
                if (!left.Clamp(animations))
                {
                    return false;
                }
                if (!right.Clamp(animations))
                {
                    return false;
                }
                return true;
            }

            public void Dispose()
            {

            }

            public IEnumerator<AnimationIndexPair> GetEnumerator()
            {
                Reset();
                return this;
            }

            public bool MoveNext()
            {
                index++;
                return index < 4;
            }

            public void Reset()
            {
                index = -1;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                Reset();
                return this;
            }
        }
        [NonSerialized]
        public AnimationIndexes animationIndexes;

    }

    [Serializable]
    public struct VaraintAnimation : IAnimation, ISerializationCallbackReceiver
    {
        public IAnimation[] animations;

        [SerializeField]
        Animation[] saveAnimations;
        [SerializeField]
        MultiTileAnimation[] saveMultiTileAnimations;

        public string name;
        public string Name { get => name; set => name = value; }
        public int framerate;
        public int Framerate { get => framerate; set => framerate = value; }

        public int index;
        public int Index { get => index; set => index = value; }

        public Type Type => typeof(VaraintAnimation);

        public VaraintAnimation(IAnimation[] animations, int index)
        {
            name = "New";
            framerate = 12;
            this.animations = animations;
            this.index = index;
            saveAnimations = new Animation[0];
            saveMultiTileAnimations = new MultiTileAnimation[0];
        }

        public AnimationClip GetAnimationClip(Type type)
        {
            int randomInt = UnityEngine.Random.Range(0, animations.Length - 1);
            Debug.LogError("Variant Animation is being collaped to index " + randomInt);

            AnimationClip animationClip = animations[randomInt].GetAnimationClip(type);
            animationClip.name = name;
            animationClip.frameRate = framerate;
            return animationClip;
        }

        public AnimatorStateMachine GetAnimatorStateMachine(Type type)
        {
            AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
            for (int i = 0; i < 4; i++)
            {
                AnimatorState animatorState = new AnimatorState();
                animatorState.motion = animations[i].GetAnimationClip(type);
                animatorStateMachine.AddState(animatorState, new Vector3(1, i, 0));
                animatorState.AddExitTransition();
            }
            return animatorStateMachine;
        }

        public AnimatorState GetAnimatorState(Type type)
        {
            int randomInt = UnityEngine.Random.Range(0, animations.Length - 1);
            Debug.LogError("Variant Animation is being collaped to index " + randomInt);

            AnimatorState animatorState = new AnimatorState();
            animatorState.motion = animations[randomInt].GetAnimationClip(type);
            animatorState.name = name;

            return animatorState;
        }

        public int CompareTo(IAnimation other)
        {
            return Index.CompareTo(other.Index);
        }

        public void OnBeforeSerialize()
        {
            saveAnimations = new Animation[0];
            saveMultiTileAnimations = new MultiTileAnimation[0];

            for (int i = 0; i < animations.Length; i++)
            {
                IAnimation animation = animations[i];
                animation.Index = i;

                if (animation.Type == typeof(Animations.Animation))
                {
                    Animations.Animation[] animationArray = new Animations.Animation[saveAnimations.Length + 1];
                    saveAnimations.CopyTo(animationArray, 0);
                    animationArray[saveAnimations.Length] = (Animations.Animation)animation;
                    saveAnimations = animationArray;
                }
                else if (animation.Type == typeof(MultiTileAnimation))
                {
                    MultiTileAnimation[] multiTileAnimationArray = new MultiTileAnimation[saveMultiTileAnimations.Length + 1];
                    saveMultiTileAnimations.CopyTo(multiTileAnimationArray, 0);
                    multiTileAnimationArray[saveMultiTileAnimations.Length] = (MultiTileAnimation)animation;
                    saveMultiTileAnimations = multiTileAnimationArray;
                }
                else
                {
                    Debug.LogError("Valid Type was not assigned");
                }
            }
        }

        public void OnAfterDeserialize()
        {
            List<IAnimation> listOfAnimations = new List<IAnimation>();
            if (saveAnimations != null)
            {
                foreach (Animations.Animation animation in saveAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            if (saveMultiTileAnimations != null)
            {
                foreach (MultiTileAnimation animation in saveMultiTileAnimations)
                {
                    listOfAnimations.Add(animation);
                }
            }
            listOfAnimations.Sort();
            animations = listOfAnimations.ToArray();
        }

    }

    [Serializable]
    public struct MultiTileAnimation : IAnimation, IEnumerable<Sprite>, IEnumerator<Sprite>
    {
        public int tileWidth;

        public string name;
        public string Name { get => name; set => name = value; }
        public int framerate;
        public int Framerate { get => framerate; set => framerate = value; }
        public string[] sprites;
        int curIndex;

        public int index;
        public int Index { get => index; set => index = value; }

        public Type Type => typeof(MultiTileAnimation);
        public Sprite Current
        {
            get
            {
                try
                {
                    return SaveSystem.LoadPNG(sprites[curIndex], Vector2.one / 2f, tileWidth);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
        object IEnumerator.Current => Current;

        public MultiTileAnimation(Sprite[] sprites, int index, string implementPath, int tileWidth = 1)
        {
            name = "New";
            framerate = 12;
            this.sprites = new string[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                string filePath = implementPath + "/" + name + "/" + i + ".png";
                if (sprites[i] != null)
                {
                    SaveSystem.SavePNG(filePath, sprites[i].texture);
                }

                this.sprites[i] = filePath;
            }
            this.index = index;
            curIndex = -1;
            this.tileWidth = tileWidth;
        }

        public AnimationClip GetAnimationClip(Type type)
        {
            AnimationClip clip = new AnimationClip
            {
                name = name,
                frameRate = framerate,
            };

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = type,
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = (float)i / (float)framerate,
                    value = SaveSystem.LoadPNG(sprites[i], new Vector2(.5f, 0), tileWidth)
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            if (!clip.isLooping)
            {
                Debug.LogError("Animation " + name + " not set to loop");
            }

            return clip;
        }

        public AnimatorState GetAnimatorState(Type type)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.motion = GetAnimationClip(type);
            animatorState.name = name;

            return animatorState;
        }

        public AnimatorStateMachine GetAnimatorStateMachine(Type type)
        {
            AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
            AnimatorState state = GetAnimatorState(type);
            state.AddExitTransition();
            animatorStateMachine.AddState(state, Vector3.one);
            animatorStateMachine.name = name;

            return animatorStateMachine;
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            curIndex++;
            return curIndex < sprites.Length;
        }

        public void Reset()
        {
            curIndex = -1;
        }

        public void Dispose()
        {

        }

        public int CompareTo(IAnimation other)
        {
            return Index.CompareTo(other.Index);
        }
    }
}
