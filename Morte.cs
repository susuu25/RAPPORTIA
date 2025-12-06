using UnityEngine;
using System.Collections;

public class Morte : EnemyBase
{
    public bool modeloInvertido = true;
    public float distanciaMelee = 3.0f;

    [Header("Ataque à Distância")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;
    public float forcaDoTiro = 20f;
    public float delayDisparo = 0.5f;

    [Header("Ataque Corpo a Corpo")]
    public GameObject hitboxMelee;
    public float delayHitbox = 0.3f; // Tempo até a ativação da hitbox
    public float duracaoHitbox = 0.5f; // Tempo de permanência da hitbox ativa

    protected override void Start()
    {
        base.Start();
        // Garante que a hitbox do soco inicie desativada
        if (hitboxMelee != null) hitboxMelee.SetActive(false);
    }
    protected override void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Lógica de Visão
        if (dist <= distanciaVisao)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;

            // Se o modelo for invertido, inverte o vetor de direção.
            // Isso faz apontar para o jogador.
            if (modeloInvertido) dir = -dir;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }

            // Lógica de Ataque
            if (dist <= distanciaAtaque)
            {
                PararMovimento();
                if (podeAtacar) StartCoroutine(RotinaDeAtaque());
            }
            // Lógica de Perseguição
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

    protected override void ExecutarAtaque()
    {
        // Verifica a distância real para decidir o tipo de ataque
        float distanciaAtual = Vector3.Distance(transform.position, player.position);

        if (distanciaAtual <= distanciaMelee)
        {
            animator.SetTrigger("AttackMelee");
            StartCoroutine(RotinaMelee());
        }
        else
        {
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque); // Alcance Ranged (máximo)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMelee); // Alcance Melee (mínimo)
    }
}