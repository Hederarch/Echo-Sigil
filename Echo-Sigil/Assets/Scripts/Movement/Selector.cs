using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GamplayCamera))]
public class Selector : MonoBehaviour
{
    Camera cam;
    Vector3 oldMousePos;
    public BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        if(mousePosition != oldMousePos)
        {
            if(Physics.Raycast(cam.ScreenToWorldPoint(mousePosition), transform.rotation.eulerAngles, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    boxCollider = (BoxCollider)hit.collider;

                }
            }
            oldMousePos = mousePosition;
        }
    }
}
