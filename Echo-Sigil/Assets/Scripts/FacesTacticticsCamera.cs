using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacesTacticticsCamera : MonoBehaviour
{
    public SpriteRenderer sprite;
    // Update is called once per frame
    void SpriteFaceCamera()
    {
        sprite.transform.right = TacticsCamera.right;
    }

    public virtual void Update()
    {
        SpriteFaceCamera();
    }
}
