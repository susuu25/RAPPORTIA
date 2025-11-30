using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummoner : MonoBehaviour
{
    [Header("Prefab dos aliados")]
    public GameObject aliadoPrefab;

    [Header("Pontos de Spawn")]
    public List<Transform> pontosSpawn = new List<Transform>();

    [Header("Tempo após todos morrerem (e primeiro spawn)")]
    public float tempoRespawn = 10f;

    private List<GameObject> aliadosAtivos = new List<GameObject>();
    private bool podeSpawnar = true;
    private bool jaSpawneiPrimeiro = false;

    void Start()
    {
        StartCoroutine(PrimeiroSpawn());
    }

    void Update()
    {
        if (jaSpawneiPrimeiro && AliadosTodosMortos() && podeSpawnar)
        {
            StartCoroutine(RespawnarDepois());
        }
    }

    IEnumerator PrimeiroSpawn()
    {
        yield return new WaitForSeconds(tempoRespawn);

        SpawnarAliados();
        jaSpawneiPrimeiro = true;
    }

    void SpawnarAliados()
    {
        aliadosAtivos.Clear();

        foreach (Transform ponto in pontosSpawn)
        {
            if (ponto != null)
            {
                GameObject aliado = Instantiate(
                    aliadoPrefab,
                    ponto.position,
                    ponto.rotation
                );

                aliadosAtivos.Add(aliado);
            }
        }
    }

    bool AliadosTodosMortos()
    {
        foreach (GameObject aliado in aliadosAtivos)
        {
            if (aliado != null)
                return false;
        }

        return true;
    }

    IEnumerator RespawnarDepois()
    {
        podeSpawnar = false;

        yield return new WaitForSeconds(tempoRespawn);

        SpawnarAliados();
        podeSpawnar = true;
    }
}
