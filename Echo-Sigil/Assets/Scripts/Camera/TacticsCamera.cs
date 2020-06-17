using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    public FacesTacticticsCamera foucus;
    public float speed = .5f;
    public Camera cam;
    public float offsetFromFoucus = 4;
    public float offsetFromZ0 = 4;
    public static Vector3 right;

    static bool cameraMoved;

    public float desieredAngle = (float)Math.PI;
    private static float angle = 0;
    Vector2 previousMousePosition;

    public static bool IsPi { get => Math.Abs(angle - Math.PI) < .5f; }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        FoucusInputs();
        SetCameraFace();
        SetSortMode();
    }

    private void FoucusInputs()
    {
        float lerpAngle;
        if(Mathf.Abs(desieredAngle - angle) > .5f)
        {
            lerpAngle = Mathf.Lerp(angle, desieredAngle, .1f);
        } else
        {
            lerpAngle = desieredAngle;
        }
        if (!Input.GetMouseButtonDown(1) && foucus != null)
        {
            transform.position = CalcPostion(lerpAngle);
            transform.rotation = CalcRotation(transform.position,lerpAngle);
            angle = lerpAngle;
        }
    }

    private void PlayerInputs()
    {
        Vector3 desierdPosition = transform.position;
        if (Input.GetMouseButton(1))
        {
            desierdPosition += transform.up * (previousMousePosition.x - Input.mousePosition.x) * (speed * Time.deltaTime);
            desierdPosition += transform.right * (previousMousePosition.y - Input.mousePosition.y) * (speed * Time.deltaTime);
        }
        else if(Input.GetAxisRaw("Horizontal") != 0 && !cameraMoved)
        {
            desieredAngle -= Input.GetAxisRaw("Horizontal") * Mathf.PI/2;
            cameraMoved = true;
            //clamp between 0 and 360
            if(desieredAngle > Mathf.PI * 2)
            {
                desieredAngle -= Mathf.PI * 2;
                angle -= Mathf.PI * 2;
            }
            else if (desieredAngle < 0)
            {
                desieredAngle += Mathf.PI * 2;
                angle += Mathf.PI * 2;
            }
            //just in case the top bit dosent work;
            desieredAngle = Mathf.Clamp(desieredAngle, 0, Mathf.PI * 2);
        }
        else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            cameraMoved = false;
        }
        previousMousePosition = Input.mousePosition;
        transform.position = desierdPosition;
    }

    private Vector3 CalcPostion(float angle)
    {
        Vector3 offset = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle));
        offset *= offsetFromFoucus;
        offset.z = -offsetFromZ0;
        return foucus.transform.position + offset;
    }

    private Quaternion CalcRotation(Vector3 position, float angle)
    {
        Vector3 rotation = Quaternion.LookRotation(foucus.transform.position - position).eulerAngles;
        rotation.z = -angle * Mathf.Rad2Deg;
        Debug.DrawRay(transform.position, transform.right);
        return Quaternion.Euler(rotation);
    }

    private void SetCameraFace()
    {
        right = transform.right;
    }

    private void SetSortMode()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = transform.up;
    }
}
