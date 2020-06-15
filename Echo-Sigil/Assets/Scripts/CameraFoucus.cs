using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFoucus : MonoBehaviour
{
    public float offsetDistance = 10;
    public float offsetAngle = 10;
    public SpriteRenderer sprite;
    // Update is called once per frame
    void Update()
    { 
        sprite.transform.forward = -TacticsCamera.forward;
        sprite.transform.right = TacticsCamera.right;
    }

    public Vector3 CalcPostion(float angle)
    {
        Vector3 offset = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle));
        offset *= offsetDistance;
        offset.z = -offsetAngle;
        return transform.position + offset;
        
    }

    public Quaternion CalcRotation(Vector3 position, float angle)
    {
        Vector3 rotation = Quaternion.LookRotation(transform.position - position).eulerAngles;
        rotation.z = -angle * Mathf.Rad2Deg;
        return Quaternion.Euler(rotation);
    }
}
