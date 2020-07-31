using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace mapEditor.animations
{
    [Serializable]
    public class Animation : IEnumerable<Sprite>, IEnumerator<Sprite>
    {
        public string name;
        public int framerate;
        public string[] sprites;
        internal int curIndex;

        public Animation()
        {
            name = "temp";
            framerate = 12;
            sprites = new string[1];
            curIndex = -1;
        }

        public virtual Sprite Current
        {
            get
            {
                try
                {
                    return SaveSystem.LoadPNG(sprites[curIndex], Vector2.one / 2f);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {

        }

        public virtual AnimationClip GetAnimationClip()
        {
            AnimationClip clip = new AnimationClip
            {
                name = name,
                frameRate = framerate,
                legacy = true
            };

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = i,
                    value = SaveSystem.LoadPNG(sprites[i], new Vector2(.5f, 0), 1)
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            return clip;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            curIndex++;
            return curIndex < sprites.Length;
        }

        public void Reset()
        {
            curIndex = -1;
        }

        IEnumerator<Sprite> IEnumerable<Sprite>.GetEnumerator()
        {
            return this;
        }
    }

    [Serializable]
    public class DirectionalAnimation : Animation
    {
        public AnimationElement[] animations;

        public override AnimationClip GetAnimationClip()
        {
            return base.GetAnimationClip();
        }
    }

    [Serializable]
    public class VaraintAnimation : Animation
    {
        public AnimationElement[] animations;

        public override AnimationClip GetAnimationClip()
        {
            return base.GetAnimationClip();
        }
    }

    [Serializable]
    public class MultiTileAnimation : Animation
    {
        int numTileWidth = 1;
        public override Sprite Current => SaveSystem.LoadPNG(sprites[curIndex], Vector2.one / 2f, numTileWidth);
        public override AnimationClip GetAnimationClip()
        {
            return base.GetAnimationClip();
        }
    } 
}
