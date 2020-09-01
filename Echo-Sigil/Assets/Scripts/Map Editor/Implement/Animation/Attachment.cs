using MapEditor.Windows;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class Attachment : Selectable
    {
        public float attachmentSizeIncrese = 2f;
        public float attachedSizeIncrese = 1.1f;
        public Text text;
        public string Name { get => text.text; set => text.text = value; }
        private bool selected;
        public int index;
        public bool Directional
        {
            get => directionalIcon.color == Color.white; set => directionalIcon.color = value ? Color.white : Color.clear;
        }
        public Image directionalIcon;
        Transform prevTransform;

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
            transform.SetParent(FindObjectOfType<Canvas>().transform);
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
            base.OnPointerUp(eventData);
        }

        private AnimationElement FindClosestAnimationElement()
        {
            float minDistance = float.MaxValue;
            AnimationElement minElement = null;
            foreach (AnimationElement animationElement in AnimationWindow.AnimationElements)
            {
                float distance = animationElement.transform.position.y - transform.position.y;
                
                if (minDistance > Mathf.Abs(distance) && (!Directional || (Directional && animationElement.Directional)))
                {
                    minDistance = distance;
                    minElement = animationElement;
                }
            }
            minElement = minElement == null ? AnimationWindow.AnimationElements[0] : minElement;
            return minElement;
        }
    }
}
