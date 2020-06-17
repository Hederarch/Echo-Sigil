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
        //seems like treating symptom, not cause. Pls find better solution
        if (TacticsCamera.IsPi)
        {
            sprite.transform.rotation = Quaternion.Euler(0,0,180);
        }
        Debug.DrawRay(sprite.transform.position, sprite.transform.right);
    }

    public virtual void Update()
    {
        SpriteFaceCamera();
    }
}
