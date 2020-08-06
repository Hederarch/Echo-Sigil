using System;
using System.Collections;
using System.Collections.Generic;
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
        public animations.Animation[] animations;

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
            animations = new animations.Animation[0];
            walkIndex = 0;
            attackIndex = 0;
            idelIndex = 0;
            fidgetIndex = 0;
        }

        public static Implement SetUnitColors(Implement unit, Color primaryColor, Color secondaryColor)
        {
            unit.primaryColor[0] = primaryColor.r;
            unit.primaryColor[1] = primaryColor.g;
            unit.primaryColor[2] = primaryColor.b;
            unit.secondaryColor[0] = secondaryColor.r;
            unit.secondaryColor[1] = secondaryColor.g;
            unit.secondaryColor[2] = secondaryColor.b;
            return unit;
        }

        public Implement SetUnitColors(Color primaryColor, Color secondaryColor)
        {
            this.primaryColor[0] = primaryColor.r;
            this.primaryColor[1] = primaryColor.g;
            this.primaryColor[2] = primaryColor.b;
            this.secondaryColor[0] = secondaryColor.r;
            this.secondaryColor[1] = secondaryColor.g;
            this.secondaryColor[2] = secondaryColor.b;
            return this;
        }

        internal void SetPrimaryColor(Color obj) => PrimaryColor = obj;

        internal void SetSecondayColor(Color obj) => SecondaryColor = obj;

        public AnimatorController GetAnimationController(Dictionary<Ability, int> abilityDictionary = null)
        {
            if (!CheckIfAllAnimationIndexInArray(abilityDictionary))
            {
                throw new IndexOutOfRangeException();
            }

            AnimatorController animator = new AnimatorController();
            animator.AddLayer("Base");
            animator.AddParameter("Direction", AnimatorControllerParameterType.Int);
            AnimatorStateMachine stateMachine = animator.layers[0].stateMachine;
            stateMachine.AddState(animations[idelIndex].GetState(typeof(SpriteRenderer)),new Vector3(1,0,0));
            

            return animator;
        }

        private bool CheckIfAllAnimationIndexInArray(Dictionary<Ability, int> abilityDictionary = null)
        {
            int length = animations.Length;
            bool upperRequired = length >= walkIndex ||
                        length >= attackIndex ||
                        length >= idelIndex ||
                        length >= fidgetIndex;
            bool lowerRequired = 0 > walkIndex ||
                        0 > attackIndex ||
                        0 > idelIndex ||
                        0 > fidgetIndex;
            bool abilityProblem = false;
            if(abilityDictionary != null)
            {
                foreach (KeyValuePair<Ability,int> iKey in abilityDictionary)
                {
                    int i = iKey.Value;
                    if(length >= i || 0 > i)
                    {
                        abilityProblem = true;
                    }
                }
            }
            bool finalCheck = upperRequired || lowerRequired || abilityProblem;
            return !finalCheck;

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