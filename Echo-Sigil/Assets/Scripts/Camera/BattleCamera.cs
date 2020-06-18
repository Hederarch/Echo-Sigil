using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public Camera cam;
    public List<FacesCamera> units = new List<FacesCamera>();

    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, 90, -90);
        cam.orthographic = false;
    }

    private Vector3 GetCenterPoint()
    {
        if (!(units.Count >= 2))
        {
            Debug.LogError("Not enogh targets for battle!");
            return units[0].GetCenterPoint();
        }

        var bounds = new Bounds(units[0].transform.position, Vector3.zero);
        for(int i = 0; i <= 1; i++)
        {
            bounds.Encapsulate(units[i].GetCenterPoint());
            units[i].transform.rotation = Quaternion.Euler(0,90,-90);
        }
        return bounds.center;
    }

}
