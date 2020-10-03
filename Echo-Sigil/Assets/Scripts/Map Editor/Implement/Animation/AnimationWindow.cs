using MapEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor.Windows
{
    public class AnimationWindow : Window
    {
        public RectTransform animationHolderTransform;
        public GameObject animationElementObject;
        public GameObject addAnimationObject;
        public GameObject attachmentObject;
        public static List<AnimationElement> animationElements = new List<AnimationElement>();

        public override void Initalize(Implement implement, Unit unit = null)
        {
            AnimationElement.SetStatics(animationElementObject, addAnimationObject, attachmentObject);
            AnimationElement.UnsubsubscribeAnimation(animationHolderTransform, animationElements);
            animationElements = AnimationElement.PopulateTransformWithAnimations(animationHolderTransform, implement.animations, true, implement.animationIndexes);
            gameObject.SetActive(true);
        }

        public override Implement Save(Implement implement, Unit unit = null)
        {
            implement.animationIndexes = (Implement.AnimationIndexes)AnimationElement.GetAnimationIdexes(animationElements, implement.animationIndexes);
            List <IAnimation> animations = new List<IAnimation>();
            foreach(AnimationElement animationElement in animationElements)
            {
                animations.Add(animationElement.GetAnimation());
            }
            implement.animations = animations.ToArray();
            
            return implement;
        }
    }
}
