using UnityEngine;
using System.Collections;

public class Morte : EnemyBase
{
    [Header("Configuração Visual")]
    public bool modeloInvertido = true;
    public float distanciaTrocaAtaque = 3.0f;

    [Header("Ataque: Tiro (Longe)")]
    public GameObject projetilPrefab;
    public Transform saidaTiro;
    public float delayTiro = 0.5f;
    public float forcaTiro = 20f;

    [Header("Ataque: Normal (Perto)")]
    public Collider hitboxColisor;     // ► Só o collider, não o objeto inteiro
    public float delayAtaque = 0.3f;
    public float duracaoAtaque = 0.5f;

    protected override void Start()
    {
        base.Start();
        hitboxColisor.enabled = false;
    }

    protected override void Update()
    {
        if (estaMorto || jogador == null) return;

        float dist = Vector3.Distance(transform.position, jogador.position);

        if (dist <= distanciaVisao)
        {
            Vector3 direcaoOlhar = (jogador.position - transform.position).normalized;
            direcaoOlhar.y = 0;

            if (modeloInvertido) direcaoOlhar = -direcaoOlhar;

            if (direcaoOlhar != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direcaoOlhar);

            if (dist <= distanciaTrocaAtaque)
            {
                PararMovimento();
                if (podeAtacar) StartCoroutine(RotinaAtaque());
            }
            else if (dist <= distanciaAtaque)
            {
                if (podeAtacar)
                {
                    PararMovimento();
                    StartCoroutine(RotinaAtaque());
                }
                else
                {
                    MoverParaJogador();
                }
            }
            else
            {
                MoverParaJogador();
            }
        }
    }

    protected override void ExecutarAtaque()
    {
        float dist = Vector3.Distance(transform.position, jogador.position);

        if (dist <= distanciaTrocaAtaque)
        {
            animador.SetTrigger("AttackMelee");
            StartCoroutine(UsarAtaqueFisico());
        }
        else
        {
            animador.SetTrigger("AttackRanged");
            StartCoroutine(UsarTiro());
        }
    }

    IEnumerator UsarAtaqueFisico()
    {
        yield return new WaitForSeconds(delayAtaque);

        hitboxColisor.enabled = true;

        yield return new WaitForSeconds(duracaoAtaque);

        hitboxColisor.enabled = false;
    }

    IEnumerator UsarTiro()
    {
        yield return new WaitForSeconds(delayTiro);

        if (jogador != null && saidaTiro != null && projetilPrefab != null)
        {
            Vector3 direcaoDaBala = (jogador.position - saidaTiro.position).normalized;

            GameObject bala = Instantiate(projetilPrefab, saidaTiro.position, Quaternion.LookRotation(direcaoDaBala));

            Rigidbody rb = bala.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = direcaoDaBala * forcaTiro;

            Destroy(bala, 5f);
        }
    }
}
