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
    public struct Implement : ISerializationCallbackReceiver
    {
        public string name;
        public int index;
        public string fragment;
        public string power;
        public int type;
        public string description;
        public float[] primaryColor;
        public float[] secondaryColor;

        public IAnimation[] animations;
        //only to be used for saveing
        public Animations.Animation[] saveAnimations;
        public DirectionalAnimation[] saveDirectionalAnimations;
        public VaraintAnimation[] saveVaraintAnimations;
        public MultiTileAnimation[] saveMultiTileAnimations;

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
        public AnimationIndexes animationIndexes;

        [NonSerialized]
        public ImplementList implementList;
        public string ImplementPath => implementList.modPath + "/" + name;
        public Sprite BaseSprite { get => SaveSystem.LoadPNG(ImplementPath + "/Base.png", Vector2.one / 2f, 1); set => SaveSystem.SavePNG(ImplementPath + "/Base.png", value.texture); }

        public Color PrimaryColor { get => new Color(primaryColor[0], primaryColor[1], primaryColor[2]); set => SetUnitColors(value, SecondaryColor); }
        public Color SecondaryColor { get => new Color(secondaryColor[0], secondaryColor[1], secondaryColor[2]); set => SetUnitColors(PrimaryColor, value); }

        public Implement(string name, ImplementList implementList)
        {
            primaryColor = new float[3];
            secondaryColor = new float[3];
            this.name = name;
            fragment = "";
            power = "";
            type = -1;
            description = "";
            animations = new IAnimation[0];
            animationIndexes = new AnimationIndexes();
            saveAnimations = null;
            saveDirectionalAnimations = null;
            saveMultiTileAnimations = null;
            saveVaraintAnimations = null;
            this.implementList = implementList;
            int length = 0;
            if (implementList.Implements != null)
            {
                length = implementList.Implements.Length;
            }
            Implement[] implements = new Implement[length + 1];
            implementList.Implements.CopyTo(implements, 0);
            index = length;
            implements[length] = this;
            implementList.implements = implements;
        }

        public void SetUnitColors(Color primaryColor, Color secondaryColor)
        {
            this.primaryColor[0] = primaryColor.r;
            this.primaryColor[1] = primaryColor.g;
            this.primaryColor[2] = primaryColor.b;
            this.secondaryColor[0] = secondaryColor.r;
            this.secondaryColor[1] = secondaryColor.g;
            this.secondaryColor[2] = secondaryColor.b;
        }

        public AnimatorController GetAnimationController(string modPath)
        {
            AnimatorController animator = new AnimatorController();
            animator.AddLayer("Base");
            animator.AddParameter("Direction", AnimatorControllerParameterType.Int);
            AnimatorStateMachine stateMachine = animator.layers[0].stateMachine;
            if (!animationIndexes.Clamp(animations))
            {
                stateMachine.AddState(GetAnimatorStateOfBaseSprite(modPath), Vector3.zero);
            }
            else
            {
                stateMachine.AddState(animations[animationIndexes.idelIndex].GetAnimatorState(typeof(SpriteRenderer)), new Vector3(1, 0, 0));
            }
            return animator;
        }

        private AnimatorState GetAnimatorStateOfBaseSprite(string modPath)
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
                value = BaseSprite
            };

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            AnimatorState animatorState = new AnimatorState
            {
                motion = clip,
                name = clip.name
            };

            return animatorState;
        }

        public void OnBeforeSerialize()
        {
            saveAnimations = new Animations.Animation[0];
            saveDirectionalAnimations = new DirectionalAnimation[0];
            saveVaraintAnimations = new VaraintAnimation[0];
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
            animations = listOfAnimations.ToArray();
        }
    }
    [Serializable]
    public class ImplementList
    {
        public string modPath;
        public string modName;
        public Implement[] implements;
        public Implement[] Implements => linkImplents();

        public static implicit operator Implement[](ImplementList i) => i.implements;

        public Implement[] linkImplents()
        {
            for (int i = 0; i < implements.Length; i++)
            {
                implements[i].implementList = this;
            }
            return implements;
        }

        public ImplementList(int Length, string modPath = null, string modName = "Defualt")
        {
            this.modPath = SaveSystem.SetDefualtModPath(modPath);
            this.modName = modName;
            implements = new Implement[Length];
            linkImplents();
        }

        public ImplementList(Implement[] implements, string modPath = null, string modName = "Defualt")
        {
            this.modPath = SaveSystem.SetDefualtModPath(modPath);
            this.modName = modName;
            this.implements = implements;
            linkImplents();
        }

        public string ImplementPath(int index) => modPath + "/" + Implements[index].name;

        public Implement this[int index] { get => implements[index]; set => implements[index] = value; }
    }
}