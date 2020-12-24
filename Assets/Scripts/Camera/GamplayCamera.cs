using System;
using UnityEngine;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Camera))]
public class GamplayCamera : MonoBehaviour
{
    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    private static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);
    public bool fixedCursor = true;

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
            angleChanged = true;
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
    private Vector3 desieredFoucus;
    public Vector3 Foucus { get => foucus; private set => foucus = value; }
    public Vector3 DesieredFoucus
    {
        get => desieredFoucus;
        set
        {
            desieredFoucus = value;
            finishedPositionChangedEvent?.Invoke();
        }
    }

    [SerializeField]
    private Vector3 foucus;

    private bool angleChanged;
    private bool positionChanged;
    /// <summary>
    /// Event when camera positon has changed
    /// </summary>
    public Action<Vector2> CameraMoved;
    /// <summary>
    /// Event when the camera has finished going to the current desierd position
    /// </summary>
    public Action finishedPositionChangedEvent;


    public void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        instance = this;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        PlayerInputs();
        FoucusInputs();
    }

    public void FoucusInputs()
    {
        angleChanged = Angle != desieredAngle;
        if (angleChanged)
        {
            float amountOfChange = rotationSpeed * Time.deltaTime;
            Angle = amountOfChange > Mathf.Abs(desieredAngle - Angle)
                ? desieredAngle
                : Angle + (Angle.Sign(Angle, desieredAngle)
                    ? amountOfChange
                    : -amountOfChange);
        }
        bool pastMovementState = positionChanged;
        positionChanged = Foucus != DesieredFoucus;
        if (positionChanged)
        {
            Foucus = Vector3.Distance(Foucus, DesieredFoucus) > distToSnap
                ? Vector3.Lerp(Foucus, DesieredFoucus, positionSpeed)
                : DesieredFoucus;
        }

        if (positionChanged || angleChanged)
        {
            Vector3 finalPosition = CalcPostion(Foucus, Angle, offsetFromFoucus, offsetFromZ0);
            Vector3 initialPosition = transform.position;
            CameraMoved?.Invoke(transform.position = finalPosition);
            if (!angleChanged && fixedCursor)
            {
                MoveCursor(initialPosition, finalPosition);
            }
            transform.rotation = Quaternion.LookRotation(Foucus - finalPosition, Vector3.forward);
            SetSortMode();
        }

        if (pastMovementState && !positionChanged)
        {
            finishedPositionChangedEvent?.Invoke();
        }
    }

    private void PlayerInputs()
    {
        //Rotate
        DesieredAngle -= angleChanged ? 0 : Input.GetAxisRaw("Camera") * Mathf.PI / 2;

        //Zoom
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (Input.mouseScrollDelta.y * positionSpeed * 100 * Time.deltaTime), zoomMinMax.x, zoomMinMax.y);
    }

    public static Vector3 CalcPostion(Vector3 origin, Angle angle, float offsetFromFoucus, float offsetFromZ0)
    {
        Vector3 offset = angle.Vector;
        offset *= offsetFromFoucus;
        offset.z = offsetFromZ0;
        return origin + offset;
    }

    private void SetSortMode()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = transform.forward;
    }

    public static void MoveCursor(Vector3 initial, Vector3 final, bool reversed = true)
    {
        Vector3 initalScreenPoint = instance.cam.WorldToScreenPoint(initial);
        Vector3 finalScreenPoint = instance.cam.WorldToScreenPoint(final);
        Vector3 direction = reversed ? initalScreenPoint - finalScreenPoint : finalScreenPoint - initalScreenPoint;
        GetCursorPos(out System.Drawing.Point position);
        SetCursorPos(position.X + (int)direction.x, position.Y - (int)direction.y);
    }

    private void OnDrawGizmos()
    {
        if (Angle != desieredAngle || DesieredFoucus != Foucus)
        {
            Vector3 final = CalcPostion(DesieredFoucus, desieredAngle, offsetFromFoucus, offsetFromZ0);
            Gizmos.DrawIcon(final, "Camera Gizmo", true, Color.cyan);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(GamplayCamera))]
[CanEditMultipleObjects]
public class GamplayCameraEditor : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("zoomMinMax"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedCursor"));
        EditorGUILayout.EndHorizontal();

        SerializedProperty angle = serializedObject.FindProperty("rotationSpeed");
        if (angle.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(angle.isExpanded, "Angle"))
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

        SerializedProperty position = serializedObject.FindProperty("positionSpeed");
        if (position.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(position.isExpanded, "Position"))
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

        if (serializedObject.ApplyModifiedProperties())
        {
            ((GamplayCamera)target).FoucusInputs();
        }
    }
}
#endif