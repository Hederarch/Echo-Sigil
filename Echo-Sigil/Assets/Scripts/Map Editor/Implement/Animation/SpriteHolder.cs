using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Animations
{
    public class SpriteHolder : MonoBehaviour
    {
        public Image image;
        public event Action<int,Sprite> ChangeEvent;
        public event Action<int> RemoveEvent;
        /// <summary>
        /// Change index of a sprite. Toggle if move right
        /// </summary>
        public event Action<int, bool> MoveEvent;
        public Text indexText;
        public int Index { get => int.Parse(indexText.text); set => indexText.text = value.ToString(); }

        public void InvokeChange()
        {
            Sprite sprite = SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Select sprite to change to",Directory.GetCurrentDirectory(),"png"), Vector2.one / 2);
            ChangeEvent?.Invoke(Index, sprite);
        }

        public void InvokeRemove()
        {
            RemoveEvent?.Invoke(Index);
        }

        public void InvokeMove(bool right)
        {
            MoveEvent?.Invoke(Index, right);
        }
    }

}