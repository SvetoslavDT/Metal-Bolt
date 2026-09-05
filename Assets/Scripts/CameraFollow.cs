using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void MoveToRoom(Vector3 roomCenterPosition, float newCameraSize)
    {
        transform.position = new Vector3(roomCenterPosition.x, roomCenterPosition.y, -10f);
        
        if (cam != null)
        {
            cam.orthographicSize = newCameraSize;
        }
    }
}
