using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Camera))]
public class GamplayCamera : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;

    public Vector2 zoomMinMax = new Vector2(1, 5);

    [Range(0, 1f)]
    public float rotationSpeed = .1f;
    public Angle DesieredAngle
    {
        get => desieredAngle; set
        {
            desieredAngle = value;
            cameraMoved = true;
        }
    }
    [SerializeField]
    private Angle desieredAngle = 0;
    public Angle angle { get; private set; } = 0;
    public float offsetFromFoucus = 4;
    public float offsetFromZ0 = 4;

    [Range(0f, 1f)]
    public float positionSpeed = .05f;
    public float distToSnap = .5f;
    public Vector3 desieredFoucus;
    public Vector3 foucus { get; internal set; }

    private bool cameraMoved;
    public static Action<Vector2> CameraMoved;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Unit.IsTurnEvent += SetAsFoucus;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        PlayerInputs();
        if (angle != desieredAngle || foucus != desieredFoucus)
        {
            FoucusInputs();
            SetSortMode();
        }
    }

    public void FoucusInputs()
    {
        if (angle != desieredAngle)
        {
            float sign = Mathf.Sign(desieredAngle - angle);
            float amountOfChange = (10 * rotationSpeed) * sign * Time.deltaTime;
            bool snap = Mathf.Abs(amountOfChange) > Mathf.Abs(desieredAngle - angle);
            if (snap)
            {
                angle = desieredAngle;
            }
            else
            {
                angle += amountOfChange;
            }
        }
        if (foucus != desieredFoucus)
        {
            foucus = Vector3.Distance(foucus, desieredFoucus) > distToSnap
                ? Vector3.Lerp(foucus, desieredFoucus, positionSpeed)
                : desieredFoucus;
        }
        CameraMoved?.Invoke(transform.position = CalcPostion(foucus, angle, offsetFromFoucus, offsetFromZ0));
        transform.rotation = Quaternion.LookRotation(foucus - transform.position, Vector3.forward);
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
        orthographicSize -= scroll * positionSpeed * 100 * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(orthographicSize, zoomMinMax.x, zoomMinMax.y);
    }

    public static Vector3 CalcPostion(Vector3 origin, Angle angle, float offsetFromFoucus, float offsetFromZ0)
    {
        Vector3 offset = angle.Vector;
        offset *= offsetFromFoucus;
        offset.z = -offsetFromZ0;
        return origin + offset;
    }

    private void SetSortMode()
    {
        Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.main.transparencySortAxis = transform.up - transform.forward;
    }

    private void SetAsFoucus(Unit unit)
    {
        if (unit != null)
        {
            desieredFoucus = unit.transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (angle != desieredAngle || desieredFoucus != foucus)
        {
            Gizmos.color = Color.cyan;
            Vector3 final = CalcPostion(desieredFoucus, desieredAngle, offsetFromFoucus, offsetFromZ0);
            Gizmos.DrawIcon(final, "Camera Gizmo", true, Color.cyan);
            Gizmos.DrawWireSphere(final, distToSnap);
        }
    }

}

[Serializable]
public struct Angle
{
    public float angleInRadians;
    public float AngleInDegrees { get => angleInRadians * Mathf.Rad2Deg; set => angleInRadians = value * Mathf.Deg2Rad; }
    public float X => Mathf.Sin(angleInRadians);
    public float Y => Mathf.Cos(angleInRadians);
    public Vector2 Vector => new Vector2(X, Y);

    public Angle(float angleInRadians)
    {
        this.angleInRadians = angleInRadians;
    }

    public static implicit operator float(Angle a) => a.angleInRadians;
    public static implicit operator Vector2(Angle a) => a.Vector;

    public static implicit operator Angle(float f) => new Angle(f);
    public static explicit operator Angle(Vector2 v)
    {
        v.Normalize();
        return new Angle(Mathf.Atan2(v.y, v.x));
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Angle))]
public class AngleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * (property.isExpanded ? 3 : 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty angleInRadians = property.FindPropertyRelative("angleInRadians");
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, false);
        position = EditorGUI.PrefixLabel(position, new GUIContent(" "));
        if (property.isExpanded)
        {
            Rect drawSpace = new Rect(position.x + (position.width - GetPropertyHeight(property, label)), position.y, GetPropertyHeight(property, label), GetPropertyHeight(property, label));
            EditorGUI.DrawRect(drawSpace, Color.black * .2f);
            position.width -= GetPropertyHeight(property, label);

            Vector2 angleVector = new Vector2(Mathf.Sin(angleInRadians.floatValue), Mathf.Cos(angleInRadians.floatValue));
            Vector2 centerDrawSpace = drawSpace.position;
            float halfHeight = GetPropertyHeight(property, label) * .5f;
            centerDrawSpace.x += halfHeight - 1;
            centerDrawSpace.y += halfHeight - 1;
            Rect anglePoint = new Rect(centerDrawSpace + (angleVector * (halfHeight - 4)), Vector2.one * 3);
            EditorGUI.DrawRect(anglePoint, Color.green);

            position.height = GetPropertyHeight(property, label) / 3;
            position.y += GetPropertyHeight(property, label) / 3;
        }
        angleInRadians.floatValue = EditorGUI.Slider(position, angleInRadians.floatValue * Mathf.Rad2Deg, 0, 360) * Mathf.Deg2Rad;
    }
}
#endif