using UnityEngine;

public class CameraCursorOffset : MonoBehaviour
{
    [Header("Настройки смещения")]
    public float maxOffset = 0.1f;            // Максимальное смещение камеры
    public float smoothSpeed = 8f;            // Скорость сглаживания
    public float movementThreshold = 0.2f;    // Порог сдвига курсора (0..1)

    private Vector3 baseLocalPos;

    void Start()
    {
        baseLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mousePos = Input.mousePosition;

        // Нормализованное смещение курсора [-1..1]
        float offsetX = Mathf.Clamp((mousePos.x - screenCenter.x) / screenCenter.x, -1f, 1f);
        float offsetY = Mathf.Clamp((mousePos.y - screenCenter.y) / screenCenter.y, -1f, 1f);

        Vector2 offsetVec = new Vector2(offsetX, offsetY);

        Vector3 targetPosition;

        if (offsetVec.magnitude > movementThreshold)
        {
            // Применяем смещение, если за пределами порога
            Vector3 desiredOffset = new Vector3(offsetX * maxOffset, offsetY * maxOffset, 0f);
            targetPosition = baseLocalPos + desiredOffset;
        }
        else
        {
            // Если меньше порога — остаемся на базовой позиции
            targetPosition = baseLocalPos;
        }

        // Плавное смещение камеры
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}