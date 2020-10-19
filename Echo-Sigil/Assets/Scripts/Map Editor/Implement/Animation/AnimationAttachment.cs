using MapEditor.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class AnimationAttachment : Selectable, IBeginDragHandler , IDragHandler, IEndDragHandler
    {
        [SerializeField] private static float attachmentSizeIncrese = 2f;
        [SerializeField] private static float attachedSizeIncrese = 1.1f;
        [SerializeField] private Text text;
        public string Label { get => text.text; set => text.text = value; }
        public AnimationIndexPair animationIndex
        {
            get => new AnimationIndexPair(index, Label, Directional); set
            {
                index = value.index;
                Label = value.label;
                Directional = value.directional;
            }
        }
        public int index;
        public bool Directional
        {
            get => directionalIcon.color == Color.white; set => directionalIcon.color = value ? Color.white : Color.clear;
        }
        [SerializeField] private Image directionalIcon;
        Transform prevTransform;
        [SerializeField] private LayoutElement layoutElement;
        private bool isDragging;

        public AnimationElement animationElement
        {
            get
            {
                if (!isDragging)
                {
                    AnimationElement output = transform.parent.parent.parent.GetComponent<AnimationElement>();
                    if(output != null)
                    {
                        return output;
                    }
                    else
                    {
                        Debug.LogError("Animation Attachment " + Label + "could not find Animation Element. Returning top level");
                        if(AnimationWindow.animationElements != null && AnimationWindow.animationElements.Count > 0)
                        {
                            return AnimationWindow.animationElements[0];
                        }
                        else
                        {
                            Debug.LogError("No animation exist! AAAAHHHHH!");
                            return null;
                        }
                        
                    }
                }
                else
                {
                    return prevAnimationElement;
                }
            }
        }
        private AnimationElement prevAnimationElement;

        private AnimationElement FindClosestAnimationElement()
        {
            float minDistance = float.MaxValue;
            AnimationElement minElement = null;
            if (animationElement != null)
            {
                foreach (AnimationElement animationElement in animationElement.containerList)
                {
                    float distance = animationElement.transform.position.y - transform.position.y;

                    if (minDistance > Mathf.Abs(distance) && (!Directional || (Directional && animationElement.animationTypeInfo.Directional)))
                    {
                        minDistance = distance;
                        minElement = animationElement;
                    }
                }
            }
            else
            {
                Debug.LogError("Animation Attachment "+ Label + " did not have list");
            }

            if(minElement != null)
            {
                return minElement;
            }
            else
            {
                if(animationElement != null)
                {
                    return animationElement;
                }
                else
                {
                    Debug.LogError("Animation Attachment " + Label + " has no contaniner list. Going up to highest level animation elements");
                    return AnimationWindow.animationElements[0];
                }
            }
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            prevAnimationElement = animationElement;
            isDragging = true;
            transform.localScale = Vector3.one * attachmentSizeIncrese;
            transform.SetParent(animationElement.transform.parent, true);
            layoutElement.ignoreLayout = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = transform.position;
            pos.y = Input.mousePosition.y;
            transform.position = pos;
            Transform elementTransform = FindClosestAnimationElement().transform;
            if (prevTransform != null)
            {
                prevTransform.localScale = Vector3.one;
            }
            elementTransform.localScale = Vector3.one * attachedSizeIncrese;
            prevTransform = elementTransform;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            transform.localScale = Vector3.one;
            AnimationElement animationElement = FindClosestAnimationElement();
            index = animationElement.index;
            layoutElement.ignoreLayout = false;
            transform.SetParent(animationElement.attachmentHolderTransform, false);
            animationElement.transform.localScale = Vector3.one;
        }
    }
    public interface IAnimationIndexes : IEnumerable<AnimationIndexPair>, IEnumerator<AnimationIndexPair>
    {
        bool Clamp(IAnimation[] animations);
        /// <summary>
        /// Find an index with a spesific name
        /// </summary>
        /// <param name="s">Label of the index</param>
        /// <returns>Returns with label NULL if no appropriote string found</returns>
        AnimationIndexPair this[string s]
        {
            get;
            set;
        }
    }
    [Serializable]
    public struct AnimationIndexPair
    {
        public int index;
        public string label;
        public bool directional;

        public bool Clamp(IAnimation[] animations)
        {
            if (animations != null)
            {
                int length = animations.Length;
                if (length > 0)
                {
                    //Upper Bounds
                    index = length <= index ? length - 1 : index;
                    //lowerbounds
                    index = 0 > index ? 0 : index;

                    return true;
                }
                else
                {
                    Debug.LogWarning("No animations found on file");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("No animations found on file");
                return false;
            }
        }
        public AnimationIndexPair(int index, string label, bool directional = false)
        {
            this.index = index;
            this.label = label;
            this.directional = directional;
        }
        public static implicit operator int(AnimationIndexPair a) => a.index;
    }
}
