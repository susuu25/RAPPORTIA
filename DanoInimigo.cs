using UnityEngine;

public class DanoInimigo : MonoBehaviour
{
    public float dano = 10f;
    public bool destruirAoBater = false;

    void Start()
    {
        if (destruirAoBater) Destroy(gameObject, 5f); // Auto-destruição se errar o alvo
    }

    // Se for bola de fogo, anda pra frente
    void Update()
    {
        if (destruirAoBater) transform.Translate(Vector3.forward * 30f * Time.deltaTime);
    }
    void OnTriggerEnter(Collider other)
    {
        // Verifica se acertou o Jogador
        if (other.CompareTag("Player"))
        {
            playerStats pStats = other.GetComponent<playerStats>();
            if (pStats != null)
            {
                // Aplica efeito visual e dano
                pStats.EnableDamagePostProcess();
                pStats.TakeDamage(dano);
            }

            // Se for projétil, destrói ao acertar o alvo
            if (destruirAoBater) Destroy(gameObject);
        }
        // Se bateu em parede/chão (ignora inimigos e outros triggers)
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            // Se for projétil, destrói ao bater no cenário
            if (destruirAoBater) Destroy(gameObject);
        }
    }
}