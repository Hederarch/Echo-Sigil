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
        unitSprite.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
    }
}