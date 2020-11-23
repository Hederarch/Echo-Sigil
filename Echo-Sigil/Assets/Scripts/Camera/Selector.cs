using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Selector : GamplayCamera
{
    public static Selector instance;
    public Transform foucusTransform;
    public bool followFoucusTransform;
    Vector3 oldMousePos;
    public BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Unit.IsTurnEvent += FollowUnit;
        instance = this;
    }

    internal static void GetCursor()
    {
        ScreenPointToGridSpace(Input.mousePosition, instance.transform.position);
        throw new NotImplementedException();
    }

    private static Vector2 ScreenPointToZ0(Vector3 screenPoint, Vector3 cameraPos)
    {
        throw new NotImplementedException();
    }

    private static Vector3 ScreenPointToWorldPoint(Vector3 screenPoint, Vector3 cameraPos)
    {
        throw new NotImplementedException();
    }

    public static TilePos ScreenPointToGridSpace(Vector3 screenPoint, Vector3 cameraPos)
    {
        throw new NotImplementedException();
    }

    public static BoxCollider ScreenPointToCollider(Vector3 screenPoint, Vector3 cameraPos)
    {
        throw new NotImplementedException();
    }

    public void FollowUnit(Unit foucus)
    {
        if (foucus != null)
        {
            foucusTransform = foucus.transform;
            followFoucusTransform = true;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition != oldMousePos)
        {
            if (Physics.Raycast(cam.ScreenToWorldPoint(mousePosition), transform.rotation.eulerAngles, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    boxCollider = (BoxCollider)hit.collider;

                }
            }
            oldMousePos = mousePosition;
        }

        if (followFoucusTransform && foucusTransform != null)
        {
            desieredFoucus = foucusTransform.position;
        }

        base.Update();
    }
}
