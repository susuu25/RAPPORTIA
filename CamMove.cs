using UnityEngine;

public class CamMove : MonoBehaviour
{
    [Header("Referência")]
    public Transform cameraPivot;

    [Header("Posição")]
    public float distance = 5f;
    public float height = 2f;   // Altura relativa ao pivot

    [Header("Movimento do Mouse")]
    public float mouseSensitivity = 3.0f;
    public float minYAngle = -20f;
    public float maxYAngle = 60f;

    [Header("Colisão da Câmera")]
    public LayerMask collisionMask;
    public float collisionOffset = 0.05f; 
    public float cameraSmoothSpeed = 25f; 

    private float currentX = 0f;
    private float currentY = 10f;
    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDistance = distance;
    }

    void Update()
    {
        currentX += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        if (cameraPivot == null) return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // A origem agora é o Pivot físico, garantindo que o Raycast saia do lugar certo
        Vector3 origin = cameraPivot.position;

        // Direção
        Vector3 desiredOffset = new Vector3(0, height, -distance);
        Vector3 desiredPosition = origin + rotation * desiredOffset;

        // Vetor de direção para o Raycast
        Vector3 direction = (desiredPosition - origin).normalized;
        float targetDistance = distance;

        // Colisão
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, collisionMask))
        {
            // Se bater na parede, puxa para perto do impacto
            float hitDistance = hit.distance - collisionOffset;
            if (hitDistance < 0.1f) hitDistance = 0.1f; // Evita valor zero ou negativo
            currentDistance = Mathf.Lerp(currentDistance, hitDistance, Time.deltaTime * cameraSmoothSpeed * 2f);
        }
        else
        {
            // Volta suavemente ao normal
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * cameraSmoothSpeed);
        }

        // Define posição final
        transform.position = origin + direction * currentDistance;

        // Olha para o pivot
        transform.LookAt(origin);
    }
}