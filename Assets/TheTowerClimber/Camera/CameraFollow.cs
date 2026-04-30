using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 플레이어 연결
    [SerializeField] private float yoffset = 10.0f; // 위로 올릴 높이

    private float fixedX;
    private float fixedZ;

    private void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3 (fixedX, target.position.y + yoffset, fixedZ);
    }
}
