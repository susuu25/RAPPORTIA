using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Necessário para evitar spawn fora do mapa

public class BossSummoner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject aliadoPrefab; // O inimigo a ser invocado
    public List<Transform> pontosSpawn = new List<Transform>(); // Locais possíveis de nascimento

    [Header("Timers")]
    public float tempoRespawn = 10f; // Tempo de espera entre ondas

    // Lista para rastrear quem está vivo
    private List<GameObject> aliadosAtivos = new List<GameObject>();

    void Start()
    {
        // Inicia o ciclo de vida do invocador
        StartCoroutine(CicloDeInvocacao());
    }

    // Removemos o Update para economizar processamento.
    // Tudo agora roda dentro deste ciclo controlado.
    IEnumerator CicloDeInvocacao()
    {
        // Espera inicial antes da primeira onda
        yield return new WaitForSeconds(tempoRespawn);

        while (true) // Loop infinito seguro (controlado pelos yields)
        {
            SpawnarAliados();

            // Loop de Monitoramento:
            // Fica preso aqui enquanto houver inimigos vivos na lista
            while (HaInimigosVivos())
            {
                // Verifica apenas 1 vez por segundo em vez de todo frame (Otimização crítica)
                yield return new WaitForSeconds(1f);
            }

            // Quando todos morrerem, espera o tempo de recarga para a próxima onda
            yield return new WaitForSeconds(tempoRespawn);
        }
    }

    void SpawnarAliados()
    {
        // Limpa referências antigas para evitar vazamento de memória
        aliadosAtivos.Clear();

        foreach (Transform ponto in pontosSpawn)
        {
            if (ponto != null)
            {
                // Garante que o inimigo nasça no chão válido. 
                // Evita que ele caia no infinito e cause crash de física.
                NavMeshHit hit;
                if (NavMesh.SamplePosition(ponto.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    GameObject aliado = Instantiate(
                        aliadoPrefab,
                        hit.position, // Usa a posição corrigida do NavMesh
                        ponto.rotation
                    );

                    aliadosAtivos.Add(aliado);
                }
            }
        }
    }

    // Verifica a lista e remove automaticamente quem já morreu
    bool HaInimigosVivos()
    {
        // Remove da lista qualquer objeto que tenha sido destruído 
        aliadosAtivos.RemoveAll(item => item == null);

        // Se sobrou alguém na lista, retorna verdadeiro (ainda há vivos)
        return aliadosAtivos.Count > 0;
    }
}