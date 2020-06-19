using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGBattleCamera : MonoBehaviour
{
    public Camera cam;
    public List<JRPGBattle> units = new List<JRPGBattle>();

    public float offsetFromFoucus = 2;
    public float offsetFromZ0 = 2;

    private void LateUpdate()
    {
        Vector3 centerPoint = GetCenterPoint();
        centerPoint.z += -offsetFromZ0;

        Vector3 forward = Vector3.left;
        if (units[0].leftSide)
        {
            transform.forward = -units[0].transform.forward;
        } 
        else
        {
            transform.forward = units[0].transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(forward, Vector3.back);
        transform.position = centerPoint + offsetFromFoucus * -transform.forward;
        
        cam.orthographic = false;

    }

    private Vector3 GetCenterPoint()
    {
        if (!(units.Count >= 2))
        {
            Debug.LogError("Not enogh targets for battle!");
            return units[0].transform.position;
        }

        var bounds = new Bounds(units[0].transform.position, Vector3.zero);
        for(int i = 0; i <= 1; i++)
        {
            bounds.Encapsulate(units[i].transform.position);
            units[i].transform.rotation = Quaternion.Euler(0,90,-90);
        }
        return bounds.center;
    }

}
