using MapEditor.Windows;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class AnimationAttachment : Selectable
    {
        public float attachmentSizeIncrese = 2f;
        public float attachedSizeIncrese = 1.1f;
        public Text text;
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
        private bool selected;
        public int index;
        public bool Directional
        {
            get => directionalIcon.color == Color.white; set => directionalIcon.color = value ? Color.white : Color.clear;
        }
        public Image directionalIcon;
        Transform prevTransform;

        public List<AnimationElement> animationElements;

        private void Update()
        {
            if (Input.GetMouseButton(0) && selected)
            {
                Drag(Input.mousePosition.y);
            }
        }

        private void Drag(float yPos)
        {
            Vector2 pos = transform.position;
            pos.y = yPos;
            transform.position = pos;
            Transform elementTransform = FindClosestAnimationElement().transform;
            if (prevTransform != null)
            {
                prevTransform.localScale = Vector3.one;
            }
            elementTransform.localScale = Vector3.one * attachedSizeIncrese;
            prevTransform = elementTransform;

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            selected = true;
            transform.localScale = Vector3.one * attachmentSizeIncrese;
            animationElements = transform.parent.parent.parent.GetComponent<AnimationElement>().containerList;
            transform.SetParent(transform.parent.parent.parent.parent);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            selected = false;
            transform.localScale = Vector3.one;
            AnimationElement animationElement = FindClosestAnimationElement();
            index = animationElement.index;
            transform.SetParent(animationElement.attachmentHolderTransform, false);
            animationElement.transform.localScale = Vector3.one;
            animationElements = null;
            base.OnPointerUp(eventData);
        }

        private AnimationElement FindClosestAnimationElement()
        {
            float minDistance = float.MaxValue;
            AnimationElement minElement = null;
            if (animationElements != null)
            {
                foreach (AnimationElement animationElement in animationElements)
                {
                    float distance = animationElement.transform.position.y - transform.position.y;

                    if (minDistance > Mathf.Abs(distance) && (!Directional || (Directional && animationElement.Directional)))
                    {
                        minDistance = distance;
                        minElement = animationElement;
                    }
                }
            }
            else
            {
                Debug.LogError("Animation Attachment did not reseive Animation Element container list");
            }
            minElement = minElement == null ? AnimationWindow.animationElements[0] : minElement;
            return minElement;
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
    public struct AnimationIndexPair
    {
        public int index;
        public string label;
        public bool directional;

        public bool Clamp(IAnimation[] animations)
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
                Debug.LogWarning("No amimations found on file");
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
