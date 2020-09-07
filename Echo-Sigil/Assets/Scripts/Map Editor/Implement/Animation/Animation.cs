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
            return this;
        }

        public IEnumerator GetEnumerator()
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

    [Serializable]
    public struct DirectionalAnimation : IAnimation, ISerializationCallbackReceiver
    {
        public IAnimation[] animations;

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
        }

        public AnimationClip GetAnimationClip(Type type)
        {
            AnimationClip animationClip = animations[0].GetAnimationClip(type);
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
            animationIndexes.Clamp(this.animations);
            animations[0] = this.animations[animationIndexes["Up"]];
            animations[1] = this.animations[animationIndexes["Down"]];
            animations[2] = this.animations[animationIndexes["Left"]];
            animations[3] = this.animations[animationIndexes["Right"]];
            this.animations = animations;
        }

        public void OnAfterDeserialize()
        {
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
                return this;
            }
        }
        [NonSerialized]
        public AnimationIndexes animationIndexes;

    }

    [Serializable]
    public struct VaraintAnimation : IAnimation
    {
        public IAnimation[] animations;

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
