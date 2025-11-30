using UnityEngine;
using System.Collections;

public class Morte : EnemyBase
{
    [Header("--- Correção Visual Exclusiva ---")]
    [Tooltip("Habilite caso o modelo 3D esteja se movendo de costas (eixo Z invertido).")]
    public bool modeloInvertido = true;

    [Header("--- Configurações Híbridas ---")]
    [Tooltip("Distância limite para alternar entre ataque corpo a corpo e à distância.")]
    public float distanciaMelee = 3.0f;

    [Header("Ataque à Distância (Tiro)")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo; // Local de spawn do projétil
    public float forcaDoTiro = 20f;
    public float delayDisparo = 0.5f; // Tempo de espera para sincronizar com a animação

    [Header("Ataque Corpo a Corpo (Melee)")]
    public GameObject hitboxMelee;
    public float delayHitbox = 0.3f; // Tempo até a ativação da hitbox
    public float duracaoHitbox = 0.5f; // Tempo de permanência da hitbox ativa

    protected override void Start()
    {
        base.Start();
        // Garante que a hitbox do soco inicie desativada
        if (hitboxMelee != null) hitboxMelee.SetActive(false);
    }

    // Sobrescrita do Update para aplicar a correção de rotação sem afetar o EnemyBase original
    protected override void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Lógica de Visão
        if (dist <= distanciaVisao)
        {
            // --- CÁLCULO DE ROTAÇÃO COM CORREÇÃO ---
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;

            // Se o modelo for invertido, inverte-se o vetor de direção.
            // Isso faz as costas (que visualmente parecem a frente) apontarem para o jogador.
            if (modeloInvertido) dir = -dir;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
            // ----------------------------------------

            // Lógica de Ataque (Mantém comportamento padrão)
            if (dist <= distanciaAtaque)
            {
                PararMovimento();
                if (podeAtacar) StartCoroutine(RotinaDeAtaque());
            }
            // Lógica de Perseguição (Mantém comportamento padrão)
            else
            {
                if (devePerseguir && !emAtaque)
                {
                    MoverParaPlayer();
                }
                else
                {
                    PararMovimento();
                }
            }
        }
        else
        {
            PararMovimento();
        }
    }

    // Função chamada automaticamente ao entrar na distância de ataque configurada no EnemyBase
    protected override void ExecutarAtaque()
    {
        // Verifica a distância real para decidir o tipo de ataque
        float distanciaAtual = Vector3.Distance(transform.position, player.position);

        if (distanciaAtual <= distanciaMelee)
        {
            // --- Lógica Melee ---
            // Requer Trigger "AttackMelee" no Animator
            animator.SetTrigger("AttackMelee");
            StartCoroutine(RotinaMelee());
        }
        else
        {
            // --- Lógica Ranged ---
            // Requer Trigger "AttackRanged" no Animator
            animator.SetTrigger("AttackRanged");
            StartCoroutine(RotinaTiro());
        }
    }

    IEnumerator RotinaMelee()
    {
        yield return new WaitForSeconds(delayHitbox);

        if (hitboxMelee != null) hitboxMelee.SetActive(true);
        yield return new WaitForSeconds(duracaoHitbox);
        if (hitboxMelee != null) hitboxMelee.SetActive(false);
    }

    IEnumerator RotinaTiro()
    {
        yield return new WaitForSeconds(delayDisparo);

        if (player == null) yield break;

        if (projetilPrefab != null && pontoDeDisparo != null)
        {
            // Pegamos a posição do pé e somamos + 1.5 metros para mirar no peito/cabeça
            Vector3 alvoNoPeito = player.position + (Vector3.up * 1.5f);

            // Calcula a direção da arma até esse ponto no peito
            Vector3 direcaoReal = (alvoNoPeito - pontoDeDisparo.position).normalized;

            // Cria e atira
            GameObject bala = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.LookRotation(direcaoReal));

            Rigidbody rbBala = bala.GetComponent<Rigidbody>();
            if (rbBala != null)
            {
                rbBala.linearVelocity = direcaoReal * forcaDoTiro;
            }
        }
    }

    // Visualização das áreas de atuação no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque); // Alcance Ranged (máximo)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMelee); // Alcance Melee (mínimo)
    }
}