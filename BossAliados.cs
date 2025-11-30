using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAliados : EnemyMelee
{
    [Header("Aliados")]
    public GameObject aliadoPrefab;
    public int quantidadeAliados = 3;
    public float raioSpawn = 4f;
    public float delayInicial = 5f;
    public float delayRespawn = 5f;

    [Header("Buff de Dano")]
    public float bonusDanoPorAliado = 0.1f; // 10% por aliado
    private float bonusDanoAtual = 0f;

    [Header("Buff de Resistência")]
    public float resistenciaPorAliado = 0.1f; // 10% por aliado

    [Header("Stun")]
    public float tempoStun = 5f;

    private List<AliadoDoBoss> aliadosVivos = new List<AliadoDoBoss>();
    private bool estaStunnado = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LoopDeAliados());
    }

    protected override void Update()
    {
        // Se estiver stunnado, não faz nada
        if (estaStunnado || isDead) return;

        // Mantém o comportamento normal
        base.Update();
    }


    IEnumerator LoopDeAliados()
    {
        // Espera 5s no começo
        yield return new WaitForSeconds(delayInicial);

        while (!isDead)
        {
            SpawnarAliados();

            // Espera até todos morrerem
            yield return new WaitUntil(() => aliadosVivos.Count == 0);

            // Stun
            yield return StartCoroutine(StunBoss());

            // Espera antes de respawnar
            yield return new WaitForSeconds(delayRespawn);
        }
    }

    void SpawnarAliados()
    {
        for (int i = 0; i < quantidadeAliados; i++)
        {
            Vector3 pos = transform.position
                + Random.insideUnitSphere * raioSpawn;

            pos.y = transform.position.y;

            GameObject aliadoGO = Instantiate(aliadoPrefab, pos, Quaternion.identity);

            AliadoDoBoss aliadoScript = aliadoGO.GetComponent<AliadoDoBoss>();
            aliadoScript.boss = this;

            aliadosVivos.Add(aliadoScript);
        }

        AtualizarBuff();
    }

    public void RemoverAliado(AliadoDoBoss aliado)
    {
        if (aliadosVivos.Contains(aliado))
            aliadosVivos.Remove(aliado);

        AtualizarBuff();
    }

    void AtualizarBuff()
    {
        float buffRes = aliadosVivos.Count * resistenciaPorAliado;
        buffRes = Mathf.Clamp(buffRes, 0, 0.9f);

        float buffDano = aliadosVivos.Count * bonusDanoPorAliado;

        resistenciaAtual = buffRes;
        bonusDanoAtual = buffDano;

        AtualizarDanoDoBoss();
    }

    void AtualizarDanoDoBoss()
    {
        if (hitboxEspada != null)
        {
            DanoInimigo dano = hitboxEspada.GetComponent<DanoInimigo>();
            if (dano != null)
            {
                dano.dano = dano.dano * (1f + bonusDanoAtual);
            }
        }
    }


    IEnumerator StunBoss()
    {
        estaStunnado = true;

        podeAtacar = false;
        devePerseguir = false;
        PararMovimento();

        animator.SetBool("Stunned", true);

        yield return new WaitForSeconds(tempoStun);

        animator.SetBool("Stunned", false);

        devePerseguir = true;
        podeAtacar = true;

        estaStunnado = false;
    }

    // Controle de resistência a dano
    private float resistenciaAtual = 0f;

    public override void TakeDamage(float dano)
    {
        float danoFinal = dano - (dano * resistenciaAtual);

        Debug.Log(
            $"[BOSS] Dano recebido: {dano} | Redução: {resistenciaAtual * 100}% | Dano final: {danoFinal} | Vida antes: {vida}"
        );

        base.TakeDamage(danoFinal);

        Debug.Log($"[BOSS] Vida depois: {vida}");
    }

    protected override void ExecutarAtaque()
    {
        if (estaStunnado) return;

        // usa o ataque padrão do EnemyMelee (trigger + coroutine da hitbox)
        base.ExecutarAtaque();
    }
}
