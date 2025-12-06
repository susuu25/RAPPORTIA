using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Helia : MonoBehaviour
{
    CharacterController character;
    Animator animator;

    [Header("Progresso")]
    public bool possuiLanca = true;
    public bool possuiDash = true;

    [Header("Combate")]
    public float tempoTotalAtaque = 1.0f;
    public float esperaParaLigarDano = 0.3f;
    public float tempoDanoLigado = 0.4f;
    public Collider colisorEspada;

    private bool isAttacking = false;

    [Header("Movimento")]
    public float velocidade = 5f;       // Velocidade de Andar (Configure no Inspector)
    public float velocidadeCorrida = 9f; // Velocidade de Correr (Novo!)
    public float velocidadeRotacao = 10f;
    public float jumpForce = 8f;
    public float gravity = -15f;

    private float velocidadeAnimacaoAtual;

    [Header("Câmeras")]
    public Camera mainCamera;
    public Camera aimCamera;
    public CamFPS camFPS;
    public GameObject crosshair;
    private bool aiming = false;

    [Header("Armas")]
    public GameObject espadaObjeto;
    public GameObject lancaObjeto;
    public Transform handTransform;

    public Lanca lanceWeapon;

    private bool usandoEspada = true;

    [Header("Dash")]
    public KeyCode dashKey = KeyCode.Q;
    public float dashDistance = 8f;
    public float dashDuration = 0.2f;
    public float cooldown = 1f;
    public Slider dashCooldownSlider;
    public bool allowAirDash = true;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isDashing;
    private bool isOnCooldown;
    private float currentDashCooldown = 0f;
    private Vector3 dashDirection;
    private Vector3 jump;

    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (colisorEspada != null)
        {
            colisorEspada.enabled = false;
        }

        EquiparEspada();

        if (!possuiLanca && lancaObjeto != null) lancaObjeto.SetActive(false);
        if (!possuiDash && dashCooldownSlider != null) dashCooldownSlider.gameObject.SetActive(false);

        jump = Vector3.zero;
        aiming = false;
        mainCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
        crosshair.SetActive(false);

        if (camFPS == null) camFPS = Object.FindFirstObjectByType<CamFPS>();
        if (dashCooldownSlider != null) { dashCooldownSlider.maxValue = cooldown; dashCooldownSlider.value = cooldown; }
    }

    void Update()
    {
        HandleWeaponSwap();
        HandleAimInput();
        HandleAttack();

        Move();
        Dash();
        HandleDashCooldown();
        HandleAnimations();

        GerenciarVisualArmas();
        GerenciarCameraMira();
    }

    void HandleAimInput()
    {
        if (usandoEspada || !possuiLanca)
        {
            aiming = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aiming = true;
            if (camFPS != null)
            {
                camFPS.SetRotationX(0f);
                camFPS.SetRotationY(mainCamera.transform.rotation.eulerAngles.y);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1)) aiming = false;
        if (Input.GetKeyUp(KeyCode.Mouse0) && aiming) ShootFromAimCamera();
    }

    public void Dash()
    {
        if (!possuiDash) return;

        if (!isDashing && !isOnCooldown && !isAttacking && Input.GetKeyDown(dashKey))
        {
            if (allowAirDash || isGrounded) StartCoroutine(DoDash());
        }
    }

    void HandleWeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquiparEspada();
        if (possuiLanca && Input.GetKeyDown(KeyCode.Alpha2)) EquiparLanca();
    }

    void HandleAttack()
    {
        if (usandoEspada && !aiming && !isAttacking)
        {
            if (Input.GetMouseButtonDown(0)) StartCoroutine(RealizarAtaque());
        }
    }

    IEnumerator RealizarAtaque()
    {
        isAttacking = true;
        if (animator != null) { animator.ResetTrigger("Ataque"); animator.SetTrigger("Ataque"); }
        yield return new WaitForSeconds(esperaParaLigarDano);
        if (colisorEspada != null) colisorEspada.enabled = true;
        yield return new WaitForSeconds(tempoDanoLigado);
        if (colisorEspada != null) colisorEspada.enabled = false;

        float tempoRestante = tempoTotalAtaque - (esperaParaLigarDano + tempoDanoLigado);
        if (tempoRestante > 0) yield return new WaitForSeconds(tempoRestante);

        isAttacking = false;
    }

    void GerenciarVisualArmas()
    {
        if (!possuiLanca)
        {
            if (espadaObjeto != null && !espadaObjeto.activeSelf) espadaObjeto.SetActive(true);
            if (lancaObjeto != null && lancaObjeto.activeSelf) lancaObjeto.SetActive(false);
            return;
        }

        if (usandoEspada)
        {
            if (espadaObjeto != null && !espadaObjeto.activeSelf) espadaObjeto.SetActive(true);
            if (lancaObjeto != null && lancaObjeto.activeSelf) lancaObjeto.SetActive(false);
        }
        else
        {
            if (espadaObjeto != null && espadaObjeto.activeSelf) espadaObjeto.SetActive(false);
            if (lancaObjeto != null && !lancaObjeto.activeSelf) lancaObjeto.SetActive(true);
        }
    }

    void GerenciarCameraMira()
    {
        if (aiming)
        {
            if (!aimCamera.gameObject.activeSelf) { mainCamera.gameObject.SetActive(false); aimCamera.gameObject.SetActive(true); crosshair.SetActive(true); }
        }
        else
        {
            if (aimCamera.gameObject.activeSelf) { mainCamera.gameObject.SetActive(true); aimCamera.gameObject.SetActive(false); crosshair.SetActive(false); }
        }
    }

    void ShootFromAimCamera()
    {
        if (usandoEspada || lanceWeapon == null) return;
        Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, 100f)) targetPoint = hit.point;
        else targetPoint = aimCamera.transform.position + aimCamera.transform.forward * 100f;
        Vector3 launchDirection = (targetPoint - lanceWeapon.transform.position).normalized;
        lanceWeapon.SetLaunchDirection(launchDirection);
        lanceWeapon.Fire();
    }

    void EquiparEspada()
    {
        usandoEspada = true; aiming = false;
        if (animator != null) animator.SetInteger("TipoArma", 0);
    }

    void EquiparLanca()
    {
        if (lanceWeapon != null && lanceWeapon.IsLaunched()) return;
        usandoEspada = false;
        if (animator != null) animator.SetInteger("TipoArma", 1);
    }

    public void OnLanceReturned() { }

    void HandleAnimations()
    {
        if (animator == null) return;
        animator.SetBool("NoChao", isGrounded);
        animator.SetBool("Mirando", aiming);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v);

        float velocidadeAlvo = 0f;
        if (isDashing)
        {
            velocidadeAlvo = 2f;
        }
        else if (inputDir.sqrMagnitude > 0.01f && !isAttacking)
        {
            velocidadeAlvo = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        }

        float suavizacao = isDashing ? 15f : 5f;
        velocidadeAnimacaoAtual = Mathf.MoveTowards(velocidadeAnimacaoAtual, velocidadeAlvo, suavizacao * Time.deltaTime);
        animator.SetFloat("Velocidade", velocidadeAnimacaoAtual);
    }

    private void Move()
    {
        bool travado = isAttacking || isDashing;
        float horizontal = travado ? 0f : Input.GetAxisRaw("Horizontal");
        float vertical = travado ? 0f : Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        Vector3 moveDir = Vector3.zero;

        if (!travado)
        {
            if (aiming)
            {
                Vector3 camForward = aimCamera.transform.forward;
                Vector3 camRight = aimCamera.transform.right;
                camForward.y = 0f; camRight.y = 0f;
                moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
                Vector3 lookDir = aimCamera.transform.forward;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadeRotacao * 2f * Time.deltaTime);
                }
            }
            else
            {
                Vector3 camForward = mainCamera.transform.forward;
                Vector3 camRight = mainCamera.transform.right;
                camForward.y = 0f; camRight.y = 0f;
                moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                if (moveDir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadeRotacao * Time.deltaTime);
                }
            }
        }

        // --- CORREÇÃO AQUI ---
        // Se estiver travado (atacando/dash), velocidade é 0.
        // Se não estiver travado e houver input: verifica se é corrida ou caminhada baseada nas variáveis do Inspector.
        float velocidadeFinal = 0f;

        if (!travado && inputDir.sqrMagnitude > 0.01f)
        {
            // Usa as variáveis públicas 'velocidade' e 'velocidadeCorrida'
            velocidadeFinal = Input.GetKey(KeyCode.LeftShift) ? velocidadeCorrida : velocidade;
        }
        // ---------------------

        Vector3 movement = moveDir * velocidadeFinal * Time.deltaTime;
        HandleJump();
        movement += Vector3.up * velocity.y * Time.deltaTime;
        character.Move(movement);
    }

    private void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            if (animator != null) { animator.ResetTrigger("Pulo"); animator.SetTrigger("Pulo"); }
        }
        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0) velocity.y *= 0.5f;
        if (velocity.y < 0) velocity.y += gravity * 2.2f * Time.deltaTime;
        else velocity.y += gravity * Time.deltaTime;
    }

    IEnumerator DoDash()
    {
        isDashing = true; isOnCooldown = true;
        currentDashCooldown = 0f;
        if (dashCooldownSlider != null) dashCooldownSlider.value = 0;
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 camForward = aiming ? aimCamera.transform.forward : mainCamera.transform.forward;
        Vector3 camRight = aiming ? aimCamera.transform.right : mainCamera.transform.right;
        camForward.y = 0f; camRight.y = 0f;
        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        dashDirection = (moveDir.sqrMagnitude > 0.01f) ? moveDir : transform.forward;
        float dashSpeed = dashDistance / dashDuration;
        float elapsed = 0f;
        if (!isGrounded) velocity.y = 0;
        while (elapsed < dashDuration)
        {
            character.Move(dashDirection * dashSpeed * Mathf.SmoothStep(1f, 0f, elapsed / dashDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    void HandleDashCooldown()
    {
        if (isOnCooldown && currentDashCooldown < cooldown)
        {
            currentDashCooldown += Time.deltaTime;
            if (dashCooldownSlider != null) dashCooldownSlider.value = currentDashCooldown;
            if (currentDashCooldown >= cooldown)
            {
                isOnCooldown = false;
                if (dashCooldownSlider != null) dashCooldownSlider.value = cooldown;
            }
        }
    }

    public bool IsAiming() { return aiming; }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
    }
}