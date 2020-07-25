using System;
using UnityEngine;

public class TacticsMovementCamera : MonoBehaviour
{
    public FacesCamera foucus;
    public Vector3 foucusPoint;
    public float speed = .5f;
    public float offsetFromFoucus = 4;


    public float offsetFromZ0 = 4;
    private static bool cameraMoved;

    private static float desieredAngle = (float)Math.PI;
    public static float angle = 0;

    private void Start()
    {
        Unit.IsTurnEvent += SetAsFoucus;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        FoucusInputs();
        SetSortMode();
        Camera.main.orthographic = true;
    }

    public void FoucusInputs()
    {
        float lerpAngle = Mathf.Abs(desieredAngle - angle) > .5f ? Mathf.Lerp(angle, desieredAngle, .1f) : desieredAngle;
        transform.position = CalcPostion(lerpAngle);
        transform.rotation = CalcRotation(transform.position,lerpAngle);
        angle = lerpAngle;
    }

    private void PlayerInputs()
    {
        Vector3 desierdPosition = transform.position;
        if(Input.GetAxisRaw("Horizontal") != 0 && !cameraMoved)
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
        transform.position = desierdPosition;
    }

    private Vector3 CalcPostion(float angle)
    {
        Vector3 offset = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle));
        offset *= offsetFromFoucus;
        offset.z = -offsetFromZ0;
        if (foucus != null)
        {
            foucusPoint = Vector3.Distance(foucusPoint, foucus.transform.position) > .5f
                ? Vector3.Lerp(foucusPoint, foucus.transform.position, .1f)
                : foucus.transform.position;
        }
        return foucusPoint + offset;
    }

    private Quaternion CalcRotation(Vector3 position, float angle)
    {
        Vector3 rotation = Quaternion.LookRotation(foucusPoint - position).eulerAngles;
        rotation.z = -angle * Mathf.Rad2Deg;
        return Quaternion.Euler(rotation);
    }

    private void SetSortMode()
    {
        Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.main.transparencySortAxis = transform.up - transform.forward;
    }

    public static Vector3 GetScreenPoint(float x, float y) => GetScreenPoint(new Vector2(x, y));
    /// <summary>
    /// Screen to world point, but corrected for camera rotation
    /// </summary>
    /// <param name="pointOnScreen"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPoint(Vector2 pointOnScreen)
    {
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(new Vector3(pointOnScreen.x, pointOnScreen.y, 0));
        Vector3 screenPointZ = screenPoint;
        screenPointZ.z = 0;
        float distToZ0 = -screenPoint.z / Mathf.Cos(GetAngleBetweenCameraForwardAndVectorForward());
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(pointOnScreen.x, pointOnScreen.y, (float)distToZ0));

        GetScreenPointGizmos(pointOnScreen, screenPoint, screenPointZ, point);

        return point;
    }

    private static void GetScreenPointGizmos(Vector2 pointOnScreen, Vector3 screenPoint, Vector3 screenPointZ, Vector3 point)
    {
        Tile tile = MapReader.GetTile(MapReader.WorldToGridSpace(point));
        if (tile != null)
        {
            Debug.DrawLine(point, new Vector3(point.x, point.y, tile.height), Color.green);
        }

        Debug.DrawLine(point, new Vector3(point.x, point.y, 0), Color.red);
        Debug.DrawLine(screenPoint, point, Color.cyan);
        Debug.DrawLine(screenPoint, screenPointZ, Color.cyan);
        Debug.DrawLine(screenPointZ, point, Color.cyan);
        Debug.DrawLine(Camera.main.ScreenToWorldPoint(new Vector3(pointOnScreen.x, 0, 0)), Camera.main.ScreenToWorldPoint(new Vector3(pointOnScreen.x, Camera.main.pixelHeight, 0)));
        Debug.DrawLine(Camera.main.ScreenToWorldPoint(new Vector3(0, pointOnScreen.y, 0)), Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, pointOnScreen.y, 0)));

        Debug.DrawLine(point, MapReader.GridToWorldSpace(MapReader.WorldToGridSpace(point)), Color.magenta);
    }

    /// <summary>
    /// Get the angle between forward and the way the camera is faceing
    /// </summary>
    /// <returns>Angle in radians</returns>
    public static float GetAngleBetweenCameraForwardAndVectorForward()
    {
        float angle = Vector3.Angle(Vector3.forward, Camera.main.transform.forward);
        angle *= Mathf.Deg2Rad;
        return angle;
    }

    public void SetAsFoucus(Unit unit)
    {
        foucus = unit;
    }
}
