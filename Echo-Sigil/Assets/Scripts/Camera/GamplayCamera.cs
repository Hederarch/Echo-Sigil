using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GamplayCamera : MonoBehaviour
{
    public Transform foucus;
    [SerializeField]
    private Vector3 foucusPoint;
    [Range(0f, 1f)]
    public float speed = .1f;
    public float distToSnap = .5f;

    public float offsetFromFoucus = 4;
    public float offsetFromZ0 = 4;

    public Vector2 zoomMinMax = new Vector2(1, 5);
    private bool cameraMoved;

    public float DesieredAngle
    {
        get => desieredAngle; set
        {
            desieredAngle = value;
            cameraMoved = true;
        }
    }
    public float angle { get; private set; } = 0;

    public Camera cam;
    private float desieredAngle = 0;
    public static Action<Vector2> CameraMoved;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Unit.IsTurnEvent += SetAsFoucus;
        FacesCamera.gameplayCamera = this;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        if (angle != desieredAngle || (foucus != null && foucusPoint != foucus.position))
        {
            FoucusInputs();
            SetSortMode();
        }
    }

    public void FoucusInputs()
    {
        if (angle != desieredAngle)
        {
            angle = Mathf.Abs(DesieredAngle - angle) > distToSnap ? Mathf.Lerp(angle, DesieredAngle, speed) : DesieredAngle;
        }
        Vector2 angleVector = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        if (foucus != null)
        {
            foucusPoint = Vector3.Distance(foucusPoint, foucus.transform.position) > distToSnap
                ? Vector3.Lerp(foucusPoint, foucus.transform.position, speed)
                : foucus.position;
        }
        CameraMoved?.Invoke(transform.position = CalcPostion(foucusPoint, angleVector, offsetFromFoucus, offsetFromZ0));
        transform.rotation = CalcRotation(foucusPoint, transform.position, angleVector, angle);
    }

    private void PlayerInputs()
    {
        //Rotate
        if (Input.GetAxisRaw("Camera") != 0 && !cameraMoved)
        {
            DesieredAngle -= Input.GetAxisRaw("Camera") * Mathf.PI / 2;
            //clamp between 0 and 360
            if (desieredAngle > Mathf.PI * 2)
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
        else if (Input.GetAxisRaw("Camera") == 0)
        {
            cameraMoved = false;
        }

        //Zoom
        float orthographicSize = cam.orthographicSize;
        float scroll = Input.mouseScrollDelta.y;
        orthographicSize -= scroll * speed * 100 * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(orthographicSize, zoomMinMax.x, zoomMinMax.y);
    }

    public static Vector3 CalcPostion(Vector3 origin, Vector2 angleVector, float offsetFromFoucus, float offsetFromZ0)
    {
        Vector3 offset = angleVector;
        offset *= offsetFromFoucus;
        offset.z = -offsetFromZ0;
        return origin + offset;
    }

    public static Vector3 CalcPostion(Vector3 origin, float angle, float offsetFromFoucus, float offsetFromZ0) => CalcPostion(origin, new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle)), offsetFromFoucus, offsetFromZ0);

    public static Quaternion CalcRotation(Vector3 origin, Vector3 position, Vector2 angleVector, float angleInRadians)
    {
        float opisite = position.z - origin.z;
        float hypotonose = Vector3.Distance(origin, position);
        float inverseSin = Mathf.Asin(opisite / hypotonose);
        Vector3 angleVectorInverse = new Vector3(-angleVector.y, angleVector.x);
        angleVectorInverse *= inverseSin * Mathf.Rad2Deg;
        angleVectorInverse.z -= angleInRadians * Mathf.Rad2Deg;
        return Quaternion.Euler(angleVectorInverse);
    }

    public static Quaternion CalcRotation(Vector3 origin, Vector3 position, float angle) => CalcRotation(origin, position, new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle)), angle);

    private void SetSortMode()
    {
        Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.main.transparencySortAxis = transform.up - transform.forward;
    }

    public void SetAsFoucus(Unit unit)
    {
        if (unit != null)
        {
            foucus = unit.transform;
        }
        else
        {
            foucus = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (angle != desieredAngle || (foucus != null && foucus.transform.position != foucusPoint))
        {
            Gizmos.color = Color.cyan;
            Vector3 orgin = foucus != null ? foucus.transform.position : foucusPoint;
            Vector3 final = CalcPostion(orgin, desieredAngle, offsetFromFoucus, offsetFromZ0);
            Gizmos.DrawIcon(final, "Camera Gizmo", true, Color.cyan);
            Gizmos.DrawWireSphere(final, distToSnap);
        }
    }

}
