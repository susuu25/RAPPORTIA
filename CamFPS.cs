using UnityEngine;

public class CamFPS : MonoBehaviour
{
    public Helia helia;

    [Header("Ajuste FPS")]
    public float alturaOlhos = 2f;
    public float distanciaNariz = 0.6f;

    [Header("Sensibilidade")]
    public float mouseSensitivity = 200f;
    public float minY = -60f;
    public float maxY = 60f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (helia == null) helia = Object.FindFirstObjectByType<Helia>();

        transform.SetParent(null);

        Vector3 angles = transform.eulerAngles;
        rotationX = 0f;
        rotationY = angles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (helia != null && helia.IsAiming())
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, minY, maxY);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

            Vector3 posicaoPe = helia.transform.position;
            Quaternion rotacaoCorpo = Quaternion.Euler(0f, rotationY, 0f);

            Vector3 posicaoFinal = posicaoPe
                                 + (Vector3.up * alturaOlhos)
                                 + (rotacaoCorpo * Vector3.forward * distanciaNariz);

            transform.position = posicaoFinal;

            helia.transform.rotation = rotacaoCorpo;
        }
    }

    public void SetRotationX(float x) { rotationX = x; }
    public void SetRotationY(float y) { rotationY = y; }
}