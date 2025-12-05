using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Status")]
    public float vidaMaxima = 100f;

    public bool podePerseguir = true;

    [Header("Combate")]
    public float distanciaVisao = 15f;
    public float distanciaAtaque = 2f;
    public float tempoEntreAtaques = 2f;
    public float tempoAnimacaoAtaque = 1.0f;

    [Header("Opcionais")]
    public healthBar barraDeVida;
    public Porta scriptPorta;

    protected NavMeshAgent agente;
    protected Animator animador;
    protected Transform jogador;

    protected bool estaMorto = false;
    protected bool podeAtacar = true;

    protected float vidaAtual;

    private float timerDestino = 0f;

    protected virtual void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animador = GetComponent<Animator>();
        vidaAtual = vidaMaxima;

        if (barraDeVida != null)
            barraDeVida.SetSliderMax(vidaMaxima);

        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
        if (objJogador != null)
            jogador = objJogador.transform;

        if (agente != null)
        {
            agente.stoppingDistance = distanciaAtaque - 0.5f;

            if (!podePerseguir)
                agente.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (estaMorto || jogador == null) return;

        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia > distanciaVisao)
        {
            PararMovimento();
        }
        else if (distancia <= distanciaAtaque)
        {
            PararMovimento();
            OlharParaJogador();

            if (podeAtacar)
                StartCoroutine(RotinaAtaque());
        }
        else
        {
            if (podePerseguir)
                MoverParaJogador();
        }
    }

    protected void MoverParaJogador()
    {
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;

        animador.SetBool("Moving", true);
        agente.isStopped = false;

        OlharParaJogador();

        timerDestino += Time.deltaTime;
        if (timerDestino > 0.25f)
        {
            agente.SetDestination(jogador.position);
            timerDestino = 0f;
        }
    }

    protected void PararMovimento()
    {
        if (agente != null && agente.enabled && agente.isOnNavMesh)
            agente.isStopped = true;

        animador.SetBool("Moving", false);
    }

    protected void OlharParaJogador()
    {
        if (jogador == null) return;

        Vector3 direcao = jogador.position - transform.position;
        direcao.y = 0;

        if (direcao != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direcao);
    }

    protected IEnumerator RotinaAtaque()
    {
        podeAtacar = false;

        ExecutarAtaque();

        yield return new WaitForSeconds(tempoAnimacaoAtaque);

        float espera = tempoEntreAtaques - tempoAnimacaoAtaque;
        if (espera > 0)
            yield return new WaitForSeconds(espera);

        podeAtacar = true;
    }

    protected abstract void ExecutarAtaque();

    public virtual void ReceberDano(float dano)
    {
        if (estaMorto) return;

        vidaAtual -= dano;

        if (barraDeVida != null)
            barraDeVida.SetSlider(vidaAtual);

        if (vidaAtual <= 0)
            Morrer();
    }

    protected virtual void Morrer()
    {
        estaMorto = true;

        if (scriptPorta != null)
            scriptPorta.AbrirSaida();

        StopAllCoroutines();
        animador.SetTrigger("Die");

        if (agente != null && agente.isOnNavMesh)
        {
            agente.ResetPath();
            agente.isStopped = true;
        }

        if (agente != null) agente.enabled = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;

        this.enabled = false;

        Destroy(gameObject, 5f);
    }
}
