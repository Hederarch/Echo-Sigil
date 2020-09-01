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
        public static List<AnimationElement> AnimationElements => animationList;
        private static List<AnimationElement> animationList = new List<AnimationElement>();
        private static List<Attachment> attachments = new List<Attachment>();

        public override void Initalize(Implement implement)
        {
            AnimationElement.SetStatics(animationElementObject, addAnimationObject, attachmentObject);
            AnimationElement.UnsubsubscribeAnimation(animationHolderTransform, animationList);
            AnimationElement.PopulateTransformWithAnimations(animationHolderTransform, implement.animations, implement);
            gameObject.SetActive(true);
        }

        public override Implement Save(Implement implement)
        {
            List<IAnimation> animations = new List<IAnimation>();
            foreach(AnimationElement animationElement in animationList)
            {
                animations.Add(animationElement.GetAnimation());
            }
            implement.animations = animations.ToArray();
            return implement;
        }
    }
}
