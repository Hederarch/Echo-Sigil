using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace mapEditor
{
    [Serializable]
    public struct Implement
    {
        public string name;
        public int index;
        public string fragment;
        public string power;
        public int type;
        public string description;
        public float[] primaryColor;
        public float[] secondaryColor;
        public animations.IAnimation[] animations;

        public int walkIndex;
        public int attackIndex;
        public int idelIndex;
        public int fidgetIndex;

        public Color PrimaryColor { get => new Color(primaryColor[0], primaryColor[1], primaryColor[2]); set => SetUnitColors(value, SecondaryColor); }
        public Color SecondaryColor { get => new Color(secondaryColor[0], secondaryColor[1], secondaryColor[2]); set => SetUnitColors(PrimaryColor, value); }
        public Sprite GetBaseSprite(string modPath) => SaveSystem.LoadPNG(modPath + "/" + name + "/Base.png", Vector2.one / 2f, 1);
        public void SetBaseSprite(string modPath, Sprite sprite)
        {
            if (sprite != null)
            {
                SaveSystem.SavePNG(modPath + "/" + name + "/Base.png", sprite.texture);
            }
        }

        public Implement(string name, int index)
        {
            primaryColor = new float[3];
            secondaryColor = new float[3];
            this.index = index;
            this.name = name;
            fragment = "";
            power = "";
            type = -1;
            description = "";
            animations = new animations.IAnimation[0];
            walkIndex = 0;
            attackIndex = 0;
            idelIndex = 0;
            fidgetIndex = 0;
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

        public AnimatorController GetAnimationController(string modPath, Dictionary<Ability, int> abilityDictionary = null)
        {
            AnimatorController animator = new AnimatorController();
            animator.AddLayer("Base");
            animator.AddParameter("Direction", AnimatorControllerParameterType.Int);
            AnimatorStateMachine stateMachine = animator.layers[0].stateMachine;
            if (!ClampAnimationIndex(abilityDictionary))
            {
                stateMachine.AddState(GetAnimatorStateOfBaseSprite(modPath),Vector3.zero);
            }
            else
            {
                stateMachine.AddState(animations[idelIndex].GetAnimatorState(typeof(SpriteRenderer)), new Vector3(1, 0, 0));
            }
            return animator;
        }

        private bool ClampAnimationIndex(Dictionary<Ability, int> abilityDictionary = null)
        {
            int length = animations.Length;
            if (length > 0)
            {
                //Upper Bounds
                walkIndex = length <= walkIndex ? length - 1 : walkIndex;
                attackIndex = length <= attackIndex ? length - 1 : attackIndex;
                idelIndex = length <= idelIndex ? length - 1 : idelIndex;
                fidgetIndex = length <= fidgetIndex ? length - 1 : fidgetIndex;

                //lowerbounds
                walkIndex = 0 > walkIndex ? 0 : walkIndex;
                attackIndex = 0 > attackIndex ? 0 : attackIndex;
                idelIndex = 0 > idelIndex ? 0 : idelIndex;
                fidgetIndex = 0 > fidgetIndex ? 0 : fidgetIndex;

                if (abilityDictionary != null)
                {
                    foreach (KeyValuePair<Ability, int> iKey in abilityDictionary)
                    {
                        int i = iKey.Value;
                        if (length <= i)
                        {
                            abilityDictionary[iKey.Key] = length - 1;
                        }
                        else if (0 > i)
                        {
                            abilityDictionary[iKey.Key] = 0;
                        }
                    }
                }
                return true;
            }
            else
            {
                Debug.LogError("No amimations for " + name + " found on file");
                return false;
            }
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
                value = GetBaseSprite(modPath)
            };

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            AnimatorState animatorState = new AnimatorState();
            animatorState.motion = clip;
            animatorState.name = clip.name;

            return animatorState;
        }
    }
    [Serializable]
    public class ImplementList
    {
        public string modPath;
        public string modName;
        public Implement[] implements;
        public Sprite BaseSprite(int index) => implements[index].GetBaseSprite(modPath);

        public static implicit operator Implement[](ImplementList i) => i.implements;

        public ImplementList(int Length, string modPath = null, string modName = "Defualt")
        {
            this.modPath = SaveSystem.SetDefualtModPath(modPath);
            this.modName = modName;
            implements = new Implement[Length];
        }

        public ImplementList(Implement[] implements, string modPath = null, string modName = "Defualt")
        {
            this.modPath = SaveSystem.SetDefualtModPath(modPath);
            this.modName = modName;
            this.implements = implements;
        }

        public string ImplementPath(int index) => modPath + "/" + implements[index].name;
    }
}