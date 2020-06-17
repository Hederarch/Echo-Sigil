using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacesTacticticsCamera : MonoBehaviour
{
    public SpriteRenderer unitSprite;
    // Update is called once per frame
    void SpriteFaceCamera()
    {
        unitSprite.transform.right = TacticsCamera.right;
        //seems like treating symptom, not cause. Pls find better solution
        if (TacticsCamera.IsPi)
        {
            unitSprite.transform.rotation = Quaternion.Euler(0,0,180);
        }
        Debug.DrawRay(unitSprite.transform.position, unitSprite.transform.right);
    }

    public virtual void Update()
    {
        SpriteFaceCamera();
    }
}
