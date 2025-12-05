using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAliados : EnemyMelee
{
    [Header("Invocação")]
    public GameObject prefabAliado;
    public int quantidade = 3;
    public float raioSpawn = 4f;

    [Header("Buffs")]
    public float danoExtraPorAliado = 0.1f;
    public float defesaPorAliado = 0.1f;

    [Header("Tempos")]
    public float tempoInicio = 5f;
    public float tempoRespawn = 5f;
    public float tempoStun = 5f;

    private List<AliadoDoBoss> aliadosVivos = new List<AliadoDoBoss>();
    private bool estaStunado = false;
    private float defesaAtual = 0f;
    private float danoBaseArma;

    protected override void Start()
    {
        base.Start();

        // Procura pelo danoAoPlayer
        if (hitboxEspada != null)
        {
            // segurança: checar se o componente existe antes de acessar
            DanoInimigo di = hitboxEspada.GetComponent<DanoInimigo>();
            if (di != null)
            {
                danoBaseArma = di.danoAoPlayer;
            }
        }

        StartCoroutine(RotinaChefe());
    }

    protected override void Update()
    {
        // Se estiver tonto, não faz nada
        if (estaStunado) return;

        base.Update();
    }

    IEnumerator RotinaChefe()
    {
        yield return new WaitForSeconds(tempoInicio);

        while (!estaMorto)
        {
            SpawnarAliados();

            // Espera todos morrerem
            // ? agora limpa referências nulas periodicamente para evitar loop infinito
            while (aliadosVivos.Count > 0)
            {
                // Remove entradas nulas (objetos destruídos sem notificação)
                aliadosVivos.RemoveAll(a => a == null);

                // Se ainda há aliados (vivos ou não notificados), espera 0.2s para não travar CPU
                if (aliadosVivos.Count > 0)
                    yield return new WaitForSeconds(0.2f);
                else
                    break;
            }

            // Fica vulnerável
            yield return StartCoroutine(FicarTonto());

            yield return new WaitForSeconds(tempoRespawn);
        }
    }

    void SpawnarAliados()
    {
        for (int i = 0; i < quantidade; i++)
        {
            Vector3 posAleatoria = transform.position + Random.insideUnitSphere * raioSpawn;
            NavMeshHit hit;

            // Acha um lugar válido no chão
            if (NavMesh.SamplePosition(posAleatoria, out hit, 2.0f, NavMesh.AllAreas))
            {
                GameObject novoAliado = Instantiate(prefabAliado, hit.position, Quaternion.identity);

                AliadoDoBoss script = novoAliado.GetComponent<AliadoDoBoss>();
                if (script != null)
                {
                    script.chefe = this;
                    aliadosVivos.Add(script);
                }
                else
                {
                    // Se o prefab não tiver o componente esperado, destrói e evita leaks
                    Debug.LogWarning("[BossAliados] prefabAliado não tem AliadoDoBoss: " + prefabAliado.name);
                    Destroy(novoAliado);
                }
            }
        }

        AtualizarPoderes();
    }

    public void AliadoMorreu(AliadoDoBoss aliado)
    {
        // proteção: se nulo ou já removido, ignora
        if (aliado == null) return;

        if (aliadosVivos.Contains(aliado))
        {
            aliadosVivos.Remove(aliado);
        }

        AtualizarPoderes();
    }

    void AtualizarPoderes()
    {
        int total = aliadosVivos.Count;

        // Atualiza defesa
        defesaAtual = total * defesaPorAliado;
        if (defesaAtual > 0.9f) defesaAtual = 0.9f;

        // Atualiza dano
        float bonus = total * danoExtraPorAliado;

        // Atualiza o danoAoPlayer com o bônus
        if (hitboxEspada != null)
        {
            DanoInimigo di = hitboxEspada.GetComponent<DanoInimigo>();
            if (di != null)
            {
                di.danoAoPlayer = danoBaseArma * (1 + bonus);
            }
        }
    }

    IEnumerator FicarTonto()
    {
        estaStunado = true;
        animador.SetBool("Stunned", true);

        if (agente != null) agente.isStopped = true;

        yield return new WaitForSeconds(tempoStun);

        animador.SetBool("Stunned", false);
        estaStunado = false;
    }

    public override void ReceberDano(float dano)
    {
        // Reduz o dano baseado no escudo
        float danoFinal = dano - (dano * defesaAtual);
        base.ReceberDano(danoFinal);
    }
}
