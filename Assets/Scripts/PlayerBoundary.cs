using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    public float minX = -24f;
    public float maxX = 24f;
    public float minY = -15f;
    public float maxY = 14f;

    void Update()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        transform.position = position;
    }
}