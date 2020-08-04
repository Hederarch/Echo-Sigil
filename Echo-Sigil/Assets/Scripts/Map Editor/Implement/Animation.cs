using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
            name = "New";
            framerate = 12;
            sprites = new string[0];
            curIndex = -1;
        }

        public Animation(Sprite[] sprites, string implementPath)
        {
            name = "New";
            framerate = 12;
            this.sprites = new string[sprites.Length];
            for(int i = 0; i < sprites.Length; i++)
            {
                string filePath = implementPath + "/" + name + "/" + i + ".png";
                if (sprites[i] != null)
                {
                    SaveSystem.SavePNG(filePath, sprites[i].texture);
                }

                this.sprites[i] = filePath;
            }
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

        public virtual AnimationClip GetAnimationClip(Type type)
        {
            AnimationClip clip = new AnimationClip
            {
                name = name,
                frameRate = framerate,
                wrapMode = WrapMode.Loop
            };

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = type,
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = (float)i/(float)framerate,
                    value = SaveSystem.LoadPNG(sprites[i], new Vector2(.5f, 0), 1)
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            if (!clip.isLooping)
            {
                Debug.LogError("Animation " + name + " not set to loop");
            }

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

        public override AnimationClip GetAnimationClip(Type type)
        {
            return base.GetAnimationClip(type);
        }
    }

    [Serializable]
    public class VaraintAnimation : Animation
    {
        public AnimationElement[] animations;

        public override AnimationClip GetAnimationClip(Type type)
        {
            return base.GetAnimationClip(type);
        }
    }

    [Serializable]
    public class MultiTileAnimation : Animation
    {
        int numTileWidth = 1;
        public override Sprite Current => SaveSystem.LoadPNG(sprites[curIndex], Vector2.one / 2f, numTileWidth);
        public override AnimationClip GetAnimationClip(Type type)
        {
            return base.GetAnimationClip(type);
        }
    } 
}
