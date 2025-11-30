using UnityEngine;
using UnityEngine.UI;

public class escudo : MonoBehaviour
{
    [Header("Configurações do Escudo")]
    public GameObject shieldObject;
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private float shieldCooldown = 15f;

    [Header("UI")]
    public Slider cooldownSlider;

    // Variáveis de controle
    private float currentCooldownTimer = 0f;
    private bool isShieldReady = true;

    // Referência para o script playerStats (para anular dano)
    private playerStats playerStatsScript;

    void Start()
    {
        // Busca a referência do script de status
        playerStatsScript = GetComponent<playerStats>();

        // Define o estado inicial
        shieldObject.SetActive(false);
        isShieldReady = true;
        currentCooldownTimer = 0;

        // Configura o slider de UI (começa cheio)
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = shieldCooldown;
            cooldownSlider.value = shieldCooldown;
        }
    }

    void Update()
    {
        // Ativação pelo input
        if (Input.GetKeyDown(KeyCode.K) && isShieldReady)
        {
            ActivateShield();
        }

        // Processa a recarga (se ativa)
        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;

            // Atualiza UI (mostra o progresso de recarga)
            if (cooldownSlider != null)
            {
                // A barra enche
                cooldownSlider.value = shieldCooldown - currentCooldownTimer;
            }

            // Quando a recarga termina
            if (currentCooldownTimer <= 0)
            {
                currentCooldownTimer = 0;
                isShieldReady = true;

                // Garante que o slider fique cheio
                if (cooldownSlider != null)
                {
                    cooldownSlider.value = shieldCooldown;
                }
            }
        }
    }

    // Ativa o escudo e agenda a desativação
    private void ActivateShield()
    {
        isShieldReady = false;

        // Zera o slider da UI
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0;
        }

        shieldObject.SetActive(true);

        // Informa o script de Status (para lógica de dano)
        if (playerStatsScript != null)
        {
            playerStatsScript.IsShieldActive = true;
        }

        Debug.Log("Escudo ativado");

        // Agenda a desativação automática
        Invoke(nameof(DeactivateShield), shieldDuration);
    }

    // Chamado pelo Invoke quando a duração do escudo acaba
    private void DeactivateShield()
    {
        shieldObject.SetActive(false);

        // Informa o script de Status
        if (playerStatsScript != null)
        {
            playerStatsScript.IsShieldActive = false;
        }
        // Inicia a contagem da recarga (após a duração terminar)
        currentCooldownTimer = shieldCooldown;
    }
}