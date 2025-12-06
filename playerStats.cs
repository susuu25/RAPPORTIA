using UnityEngine;
using UnityEngine.Rendering;

public class playerStats : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    public healthBar healthBar; // Referência ao script da barra

    [Header("Respawn")]
    [Tooltip("Marque para o jogador renascer ao morrer.")]
    [SerializeField] private bool usarRespawn = true;
    [Tooltip("Arraste um objeto vazio onde o player deve nascer.")]
    public Transform respawnPoint;

    [Header("Regeneração")]
    [SerializeField] private float healRatePerSecond = 3f;
    [SerializeField] private float delayToStartHeal = 3f;

    [Header("Efeitos")]
    public Volume postProcessVolume;

    // Referências e Variáveis Internas
    private CharacterController character;
    private float currentHealth;
    private float timeSpentIdle = 0f;

    // Propriedades Públicas
    public bool IsShieldActive { get; set; }
    public bool IsDead { get; private set; }

    public float GetCurrentHealth() => currentHealth;

    void Start()
    {
        character = GetComponent<CharacterController>();

        // Inicializa status
        ResetarVida();
    }

    void Update()
    {
        if (IsDead) return;

        GerenciarRegeneracao();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        // Lógica do Escudo
        if (IsShieldActive)
        {
            amount *= 0.5f;
            Debug.Log($"Dano REDUZIDO para {amount} pelo escudo!");
        }

        currentHealth -= amount;
        AtualizarBarraUI();

        // Reseta regeneração ao tomar dano
        timeSpentIdle = 0f;

        // Efeito visual de dano
        EnableDamagePostProcess();

        if (currentHealth <= 0)
        {
            Morrer();
        }
    }

    public void HealDamage(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        AtualizarBarraUI();
    }

    void Morrer()
    {
        currentHealth = 0;
        IsDead = true;
        Debug.Log("Jogador Morreu");

        if (usarRespawn && respawnPoint != null)
        {
            // Respawn imediato (pode colocar um delay com Invoke se preferir)
            Respawn();
        }
    }

    void Respawn()
    {
        if (character != null) character.enabled = false;

        transform.position = respawnPoint.position;

        if (character != null) character.enabled = true;

        ResetarVida();
        Debug.Log("Jogador Respawnou!");
    }

    void ResetarVida()
    {
        currentHealth = maxHealth;
        IsDead = false;
        IsShieldActive = false;
        DisableDamagePostProcess();

        if (healthBar != null) healthBar.SetSliderMax(maxHealth);
    }

    void AtualizarBarraUI()
    {
        if (healthBar != null) healthBar.SetSlider(currentHealth);
    }


    void GerenciarRegeneracao()
    {
     
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f ||
                        Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f;

        if (isMoving)
        {
            timeSpentIdle = 0f;
        }
        else
        {
            timeSpentIdle += Time.deltaTime;

            if (timeSpentIdle >= delayToStartHeal && currentHealth < maxHealth)
            {
                HealDamage(healRatePerSecond * Time.deltaTime);
            }
        }
    }

    public void EnableDamagePostProcess()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.weight = 0.3f;
            CancelInvoke(nameof(DisableDamagePostProcess));
            Invoke(nameof(DisableDamagePostProcess), 1f);
        }
    }

    public void DisableDamagePostProcess()
    {
        if (postProcessVolume != null) postProcessVolume.weight = 0f;
    }

}
