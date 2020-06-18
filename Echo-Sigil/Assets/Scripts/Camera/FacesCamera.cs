using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacesCamera : MonoBehaviour
{
    public SpriteRenderer unitSprite;
    // Update is called once per frame
    protected virtual void Update()
    {
        unitSprite.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - unitSprite.transform.position, Camera.main.transform.up);
        transform.rotation = Quaternion.identity;
    }

    public Vector3 GetCenterPoint()
    {
        return transform.position + (transform.up * unitSprite.bounds.extents.z);
    }
}