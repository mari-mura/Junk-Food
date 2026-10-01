using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    public float speed = 1f;
    public Transform player;
    public float edgeBuffer = 0.1f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerX = cam.WorldToViewportPoint(player.position).x;
        
        if (transform.position.x < 30f && playerX > edgeBuffer)
            transform.Translate(Vector3.right * (speed * Time.deltaTime), Space.World);
    }

    void LateUpdate()
    {
        Vector3 pos = cam.WorldToViewportPoint(player.position);
        pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f);
        player.position = cam.ViewportToWorldPoint(pos);
    }
}
