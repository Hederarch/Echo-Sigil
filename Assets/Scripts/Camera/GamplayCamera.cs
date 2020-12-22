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


    public void Start()
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
        offset.z = offsetFromZ0;
        return origin + offset;
    }

    private void SetSortMode()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = transform.forward;
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
            Vector3 final = CalcPostion(desieredFoucus, desieredAngle, offsetFromFoucus, offsetFromZ0);
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

        EditorGUILayout.PropertyField(serializedObject.FindProperty("zoomMinMax"));

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