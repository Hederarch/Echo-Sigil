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
    public static float GetSignedAngle(Vector3 positon, Vector3 forward, Vector3 target, FlattenAxis flattenAxis = FlattenAxis.y)
    {
        Vector2 flattendPosition = GetFlattenedVector(positon, flattenAxis);
        Vector2 flattenedForward = GetFlattenedVector(forward, flattenAxis);
        Vector2 flattenedTarget = GetFlattenedVector(target, flattenAxis);

        Vector2 directionToTarget = flattenedTarget - flattendPosition;
        directionToTarget.Normalize();

        return Vector2.SignedAngle(flattenedForward.normalized, directionToTarget);
    }

    public static Vector2 GetFlattenedVector(Vector3 value, FlattenAxis flattenAxis)
    {
        switch (flattenAxis)
        {
            case FlattenAxis.x:
                return new Vector2(value.y, value.z);
            case FlattenAxis.y:
                return new Vector2(value.x, value.z);
            case FlattenAxis.z:
                return value;
        }
        return value;
    }

}

public enum FlattenAxis { x, y, z }
