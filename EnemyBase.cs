using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Geral")]
    public float vida = 100f;
    public bool devePerseguir = true;

    [Header("Vida UI")]
    public healthBar barraDeVidaUI;

    public Porta scriptDaPorta;


    [Header("Combate e movimento")]
    public float distanciaVisao = 15f;
    public float distanciaAtaque = 2f;
    public float cooldownAtaque = 2f;

    public float tempoTravadoAtaque = 1.0f;

    // Variáveis internas
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;
    protected bool isDead = false;
    protected bool podeAtacar = true;
    protected bool emAtaque = false;
    protected string triggerMorte = "Die";

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (barraDeVidaUI != null) barraDeVidaUI.SetSliderMax(vida);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (agent != null) agent.stoppingDistance = distanciaAtaque - 0.5f;
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Lógica de Visão
        if (dist <= distanciaVisao)
        {
            RotacionarParaPlayer();

            // Lógica de Ataque
            if (dist <= distanciaAtaque)
            {
                PararMovimento();
                if (podeAtacar) StartCoroutine(RotinaDeAtaque());
            }
            // Lógica de Perseguição
            else
            {
                // Só persegue se deve perseguir e não está atacando
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

    // Movimento
    protected void MoverParaPlayer()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("Moving", true);
        }
    }

    protected void PararMovimento()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = true;
        animator.SetBool("Moving", false);
    }

    protected void RotacionarParaPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }

    // Combate
    protected IEnumerator RotinaDeAtaque()
    {
        podeAtacar = false;
        emAtaque = true; // Trava o movimento

        ExecutarAtaque(); // Chama o filho (Melee ou Ranged)

        // Espera o tempo da animação
        yield return new WaitForSeconds(tempoTravadoAtaque);

        emAtaque = false; // Libera o movimento

        // Calcula quanto falta para o cooldown acabar
        float tempoRestante = cooldownAtaque - tempoTravadoAtaque;
        if (tempoRestante > 0) yield return new WaitForSeconds(tempoRestante);

        podeAtacar = true; // Libera um novo ataque
    }

    protected abstract void ExecutarAtaque();

    // Vida e morte
    public virtual void TakeDamage(float dano)
    {
        if (isDead) return;

        vida -= dano;
        if (barraDeVidaUI != null) barraDeVidaUI.SetSlider(vida);

        if (vida <= 0) Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        
        if (scriptDaPorta != null)
        {
            scriptDaPorta.AbrirSaida();
        }
   
        StopAllCoroutines(); // Para ataques no meio
        animator.SetTrigger(triggerMorte);

        // Desliga tudo
        if (agent != null) { agent.isStopped = true; agent.enabled = false; }
        if (TryGetComponent(out Collider col)) col.enabled = false;
        if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;

        this.enabled = false;
        Destroy(gameObject, 5f);
    }
}