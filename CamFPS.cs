using UnityEngine;

public class CamFPS : MonoBehaviour
{
    public Helia helia;

    [Header("Referência (Crie um Empty nos olhos da Helia)")]
    public Transform cameraPivot; // ARRASTE O "PontoCamera" AQUI

    [Header("Ajuste FPS")]
    public float distanciaNariz = 0.1f; // Reduzi para escala pequena

    [Header("Sensibilidade")]
    public float mouseSensitivity = 2.0f; // Valor menor pois removemos o DeltaTime
    public float minY = -60f;
    public float maxY = 60f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (helia == null) helia = Object.FindFirstObjectByType<Helia>();

        transform.SetParent(null);

        // Pega a rotação inicial correta
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (helia != null && helia.IsAiming() && cameraPivot != null)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, minY, maxY);

            // Gira a câmera
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // A rotação do corpo da Helia segue apenas o Y (horizontal) da câmera
            Quaternion rotacaoCorpo = Quaternion.Euler(0f, rotationY, 0f);
            helia.transform.rotation = rotacaoCorpo;

            // Posiciona a câmera baseada no Pivot físico (PontoCamera)
            // Isso evita erros matemáticos de escala
            Vector3 posicaoFinal = cameraPivot.position + (rotacaoCorpo * Vector3.forward * distanciaNariz);
            transform.position = posicaoFinal;
        }
    }

    public void SetRotationX(float x) { rotationX = x; }
    public void SetRotationY(float y) { rotationY = y; }
}