using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using MapEditor.Animations;
using System.Collections;

namespace MapEditor
{
    [Serializable]
    public class Implement
    {
        public int index;

        [Serializable]
        public struct SplashInfo
        {
            public string name;
            public string fragment;
            public string power;
            public int type;
            public string description;
            public float[] primaryColor;
            public float[] secondaryColor;

            public SplashInfo(string name, string fragment, string power, int type, string description, float[] primaryColor, float[] secondaryColor)
            {
                this.name = name ?? "";
                this.fragment = fragment ?? "";
                this.power = power ?? "";
                this.type = type;
                this.description = description ?? "";
                this.primaryColor = primaryColor ?? new float[] { 0, 0, 0 };
                this.secondaryColor = secondaryColor ?? new float[] { 1, 1, 1 };
            }

            public SplashInfo(string name, string fragment, string power, int type, string description, Color primaryColor, Color secondaryColor)
            {
                this.name = name ?? "";
                this.fragment = fragment ?? "";
                this.power = power ?? "";
                this.type = type;
                this.description = description ?? "";
                this.primaryColor = new float[] { 0, 0, 0 };
                this.secondaryColor = new float[] { 1, 1, 1 };
                PrimaryColor = primaryColor;
                SecondaryColor = secondaryColor;
            }
            public Color PrimaryColor { get => new Color(primaryColor[0], primaryColor[1], primaryColor[2]); set => SetUnitColors(value, SecondaryColor); }
            public Color SecondaryColor { get => new Color(secondaryColor[0], secondaryColor[1], secondaryColor[2]); set => SetUnitColors(PrimaryColor, value); }

            public void SetUnitColors(Color primaryColor, Color secondaryColor)
            {
                this.primaryColor[0] = primaryColor.r;
                this.primaryColor[1] = primaryColor.g;
                this.primaryColor[2] = primaryColor.b;
                this.secondaryColor[0] = secondaryColor.r;
                this.secondaryColor[1] = secondaryColor.g;
                this.secondaryColor[2] = secondaryColor.b;
            }

        }
        public SplashInfo splashInfo;
        
        [NonSerialized]
        public IAnimation[] animations;

        [Serializable]
        public class AnimationIndexes : IAnimationIndexes
        {
            public AnimationIndexPair walkIndex = new AnimationIndexPair(0, "Walk", true);
            public AnimationIndexPair attackIndex = new AnimationIndexPair(0, "Attack");
            public AnimationIndexPair idelIndex = new AnimationIndexPair(0, "Idel");
            public AnimationIndexPair fidgetIndex = new AnimationIndexPair(0, "Fidget");
            private int index = -1;

            public AnimationIndexPair this[string s]
            {
                get
                {
                    switch (s)
                    {
                        case "Walk":
                            return walkIndex;
                        case "Attack":
                            return attackIndex;
                        case "Idel":
                            return idelIndex;
                        case "Fidget":
                            return fidgetIndex;
                    }
                    return new AnimationIndexPair(0, "NULL");
                }
                set
                {
                    switch (s)
                    {
                        case "Walk":
                            walkIndex = value;
                            break;
                        case "Attack":
                            attackIndex = value;
                            break;
                        case "Idel":
                            idelIndex = value;
                            break;
                        case "Fidget":
                            fidgetIndex = value;
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
                            return walkIndex;
                        case 1:
                            return attackIndex;
                        case 2:
                            return idelIndex;
                        case 3:
                            return fidgetIndex;
                    }
                    return new AnimationIndexPair(0, "NULL");
                }
            }

            object IEnumerator.Current => Current;

            public bool Clamp(IAnimation[] animations)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!walkIndex.Clamp(animations))
                    {
                        return false;
                    }
                    if (!attackIndex.Clamp(animations))
                    {
                        return false;
                    }
                    if (!idelIndex.Clamp(animations))
                    {
                        return false;
                    }
                    if (!fidgetIndex.Clamp(animations))
                    {
                        return false;
                    }
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
        public AnimationIndexes animationIndexes;

        [NonSerialized]
        public int modPathIndex;
        [NonSerialized]
        public Sprite baseSprite;

        public Implement(int _modPathIndex,int _index)
        {
            splashInfo = new SplashInfo("", "", "", -1, "", Color.black, Color.white);
            index = _index;
            modPathIndex = _modPathIndex;
            animations = new IAnimation[0];
            animationIndexes = new AnimationIndexes();
            baseSprite = null;
        }

        public AnimatorController GetAnimationController()
        {
            AnimatorController animator = new AnimatorController();
            animator.AddLayer("Base");
            animator.AddParameter("Direction", AnimatorControllerParameterType.Int);
            AnimatorStateMachine stateMachine = animator.layers[0].stateMachine;
            if (!animationIndexes.Clamp(animations))
            {
                stateMachine.AddState(GetAnimatorStateOfBaseSprite(), Vector3.zero);
            }
            else
            {
                stateMachine.AddState(animations[animationIndexes.idelIndex].GetAnimatorState(typeof(SpriteRenderer)), new Vector3(1, 0, 0));
            }
            return animator;
        }

        internal bool NullCheck()
        {
            if(animations != null)
            {
                foreach (IAnimation animation in animations)
                {
                    if(animation != null)
                    {
                        if (!animation.NullCheck())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return animationIndexes != null;
        }

        private AnimatorState GetAnimatorStateOfBaseSprite()
        {
            AnimationClip clip = new AnimationClip
            {
                name = "Base Sprite",
            };

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[1];
            spriteKeyFrames[0] = new ObjectReferenceKeyframe
            {
                time = 0,
                value = baseSprite
            };

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            AnimatorState animatorState = new AnimatorState
            {
                motion = clip,
                name = clip.name
            };

            return animatorState;
        }

    }
}