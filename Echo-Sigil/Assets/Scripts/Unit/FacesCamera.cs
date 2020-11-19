using UnityEngine;

public class FacesCamera : MonoBehaviour
{
    public static GamplayCamera gameplayCamera;
    public SpriteRenderer unitSprite;

    private void Start()
    {
        GamplayCamera.CameraMoved += FaceTarget;
    }

    private void FaceTarget(Vector2 target)
    {
        if (gameplayCamera != null)
        {
            Vector2 forward = (Vector2)gameplayCamera.transform.position - (Vector2)transform.position;
            if (forward != Vector2.zero)
            {
                unitSprite.transform.rotation = Quaternion.LookRotation(-forward, Vector3.forward);
            }
        }
    }
}