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
    public static GamplayCamera instance;

    public Vector2 zoomMinMax = new Vector2(1, 5);

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
    public Angle Angle { get => angle; private set => angle = value; }
    [SerializeField]
    private Angle angle = 0;
    public float offsetFromFoucus = 4;
    public float offsetFromZ0 = 4;

    public float positionSpeed = .05f;
    public float distToSnap = .5f;
    public Vector3 desieredFoucus;
    public Vector3 Foucus { get => foucus; private set => foucus = value; }
    [SerializeField]
    private Vector3 foucus;

    private bool cameraMoved;
    public static Action<Vector2> CameraMoved;


    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Unit.IsTurnEvent += SetAsFoucus;
        instance = this;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        PlayerInputs();
        if (Angle != desieredAngle || Foucus != desieredFoucus)
        {
            FoucusInputs();
            SetSortMode();
        }
    }

    public void FoucusInputs()
    {
        if (Angle != desieredAngle)
        {
            bool sign = Angle.Sign(Angle, desieredAngle);
            float amountOfChange = rotationSpeed * Time.deltaTime;
            bool snap = amountOfChange > Mathf.Abs(desieredAngle - Angle);
            if (snap)
            {
                Angle = desieredAngle;
            }
            else
            {
                Angle += sign ? amountOfChange : -amountOfChange;
            }
        }
        if (Foucus != desieredFoucus)
        {
            Foucus = Vector3.Distance(Foucus, desieredFoucus) > distToSnap
                ? Vector3.Lerp(Foucus, desieredFoucus, positionSpeed)
                : desieredFoucus;
        }
        CameraMoved?.Invoke(transform.position = CalcPostion(Foucus, Angle, offsetFromFoucus, offsetFromZ0));
        transform.rotation = Quaternion.LookRotation(Foucus - transform.position, Vector3.forward);
    }

    private void PlayerInputs()
    {
        //Rotate
        if (Input.GetAxisRaw("Camera") != 0 && !cameraMoved)
        {
            DesieredAngle -= Input.GetAxisRaw("Camera") * Mathf.PI / 2;
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
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = -transform.forward;
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
        if (Angle != desieredAngle || desieredFoucus != Foucus)
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

    public override string ToString()
    {
        return (angleInRadians * Mathf.Rad2Deg).ToString();
    }

    /// <summary>
    /// Is it faster to add to a to get to b?
    /// </summary>
    /// <param name="a">Sourse</param>
    /// <param name="b">Destination</param>
    /// <returns></returns>
    public static bool Sign(Angle a, Angle b)
    {
        bool bigger = a.angleInRadians > b.angleInRadians;
        float minusAmount = bigger ? a.angleInRadians - b.angleInRadians : b.angleInRadians - (a.angleInRadians * Mathf.PI * 2);
        float plusAmount = bigger ? a.angleInRadians - Mathf.PI * 2 + b.angleInRadians : b.angleInRadians - a.angleInRadians;
        return plusAmount < minusAmount;
    }

    public static implicit operator float(Angle a) => a.angleInRadians;
    public static implicit operator Vector2(Angle a) => a.Vector;

    public static implicit operator Angle(float f) => new Angle(f);
    public static explicit operator Angle(Vector2 v)
    {
        v.Normalize();
        return new Angle(Mathf.Atan2(v.y, v.x));
    }

    public static Angle operator +(Angle a, float f)
    {
        a.angleInRadians += f;
        //clamp between 0 and 360
        if (a.angleInRadians > Mathf.PI * 2)
        {
            a.angleInRadians -= Mathf.PI * 2;
        }
        else if (a.angleInRadians < 0)
        {
            a.angleInRadians += Mathf.PI * 2;
        }
        //just in case the top bit dosent work;
        a.angleInRadians = Mathf.Clamp(a.angleInRadians, 0, Mathf.PI * 2);
        return a;
    }
    public static Angle operator -(Angle a, float f) => a + (-f);
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
            EditorGUI.DrawRect(anglePoint, GUI.enabled ? Color.green : Color.green * .5f);

            position.height = GetPropertyHeight(property, label) / 3;
            position.y += GetPropertyHeight(property, label) / 3;
        }
        angleInRadians.floatValue = EditorGUI.Slider(position, angleInRadians.floatValue * Mathf.Rad2Deg, 0, 360) * Mathf.Deg2Rad;
    }
}

[CustomEditor(typeof(GamplayCamera))]
[CanEditMultipleObjects]
public class GamplayCmaeraEditor : Editor
{
    private bool angle;
    private bool position;

    public override void OnInspectorGUI()
    {

        EditorGUILayout.PropertyField(serializedObject.FindProperty("zoomMinMax"));

        if (angle = EditorGUILayout.BeginFoldoutHeaderGroup(angle, "Angle"))
        {
            EditorGUI.indentLevel += 1;

            EditorGUILayout.Slider(serializedObject.FindProperty("rotationSpeed"), 0, 10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetFromFoucus"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetFromZ0"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("desieredAngle"));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angle"));
            GUI.enabled = true;

            EditorGUI.indentLevel -= 1;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (position = EditorGUILayout.BeginFoldoutHeaderGroup(position, "Position"))
        {
            EditorGUI.indentLevel += 1;

            EditorGUILayout.Slider(serializedObject.FindProperty("positionSpeed"), 0, 1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distToSnap"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("desieredFoucus"));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("foucus"));
            GUI.enabled = true;

            EditorGUI.indentLevel -= 1;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif