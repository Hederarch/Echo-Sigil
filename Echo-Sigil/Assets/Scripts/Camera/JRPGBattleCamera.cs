using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGBattleCamera : MonoBehaviour
{
    public Camera cam;
    public List<JRPGBattle> units = new List<JRPGBattle>();

    public float offsetFromFoucus = 4;
    public float offsetFromZ0 = 4;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 90, -90);
        Vector3 centerPoint = GetCenterPoint();

        centerPoint.z += -offsetFromZ0;
        Vector2 directionToUnit0 = units[0].transform.position - transform.position;
        if (units[0].leftSide)
        {
            transform.right = -directionToUnit0;
        } else
        {
            transform.right = directionToUnit0;
        }

        Vector3 newPosition = centerPoint + offsetFromFoucus * -transform.forward;

        transform.position = newPosition;
        
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
