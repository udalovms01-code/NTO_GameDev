using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target;         // точка игрока (обычно голова)
    public float distance = 3f;      // желаемая дистанция камеры
    public float smooth = 10f;
    public LayerMask collisionMask;  // слои препятствий

    private float currentDistance;

    void Start()
    {
        currentDistance = distance;
    }

    void LateUpdate()
    {
        Vector3 direction = (transform.position - target.position).normalized;

        // Проверяем препятствия
        if (Physics.Raycast(target.position, direction, out RaycastHit hit, distance, collisionMask))
        {
            currentDistance = hit.distance - 0.1f; // слегка смещаем вперёд
        }
        else
        {
            currentDistance = distance;
        }

        // Плавно перемещаем камеру
        Vector3 desiredPos = target.position + direction * currentDistance;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
    }
}