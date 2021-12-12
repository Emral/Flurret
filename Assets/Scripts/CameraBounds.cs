using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private BoxCollider2D _boundary;

    public bool snap = false;

    [ContextMenuItem("Setup Collider", "Awake")]
    public Camera defaultCameraRef;

    // Start is called before the first frame update
    private void Start()
    {
        _boundary = GetComponent<BoxCollider2D>();
        _boundary.isTrigger = true;

        Camera cam = defaultCameraRef;
        if (Manager.instance != null)
        {
            cam = Manager.instance.mainCam;
        }
        Vector2 minSize = cam.GetCameraSize();
        _boundary.size = new Vector2(Mathf.Max(minSize.x, _boundary.size.x), Mathf.Max(minSize.y, _boundary.size.y));
        ;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player p)
        {
            CameraMovement.instance.SetBounds(_boundary, snap);
        }
    }

    public void OnDrawGizmos()
    {
        if (_boundary == null)
        {
            _boundary = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(_boundary.size.x, _boundary.size.y, 1));
        Gizmos.color = Color.white;
    }
}