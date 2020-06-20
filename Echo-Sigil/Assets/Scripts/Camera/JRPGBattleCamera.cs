using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGBattleCamera : MonoBehaviour
{
    public Camera cam;

    public float offsetFromFoucus = 2;
    public float offsetFromZ0 = 2;

    private void SetCamera()
    {
        Vector3 centerPoint = GetCenterPoint();
        centerPoint.z += -offsetFromZ0;

        Vector3 forward = Vector3.left;
        if (BattleData.instagator.leftSide)
        {
            transform.forward = -BattleData.instagator.transform.forward;
        } 
        else
        {
            transform.forward = BattleData.instagator.transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(forward, Vector3.back);
        transform.position = centerPoint + offsetFromFoucus * -transform.forward;
        
        cam.orthographic = false;

    }

    private Vector3 GetCenterPoint()
    {
        if (BattleData.instagator == null || BattleData.combatant == null)
        {
            Debug.LogError("Not enogh targets for battle! Sending Camera to center of map");
            return Vector3.zero;
        }

        var bounds = new Bounds(BattleData.instagator.transform.position, Vector3.zero);
        bounds.Encapsulate(BattleData.combatant.transform.position);

        return bounds.center;
    }

    public void SetCameraLookOreietaiton(Vector3 direction)
    {
        foreach (JRPGBattle j in BattleData.leftCombatants)
        {
            j.transform.rotation = Quaternion.Euler(direction);
        }
        foreach (JRPGBattle j in BattleData.rightCombatants)
        {
            j.transform.rotation = Quaternion.Euler(direction);
        }
    }

    public void SwitchCamera(bool isBattle)
    {
        if (!isBattle)
        {
            enabled = false;
            GetComponentInParent<TacticsMovementCamera>().enabled = true;
            SetCameraLookOreietaiton(new Vector3(0,0,0));
        } else
        {
            enabled = true;
            GetComponentInParent<TacticsMovementCamera>().enabled = false;
            SetCamera();
            SetCameraLookOreietaiton(new Vector3(0, 90, -90));
        }
    }
}
