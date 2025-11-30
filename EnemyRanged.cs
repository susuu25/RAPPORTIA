using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Geral")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;

    protected override void ExecutarAtaque()
    {
        animator.SetTrigger("attack");

        // MUDANÇA: Força o tiro a sair em 0.5 segundos, sem depender da animação
        Invoke(nameof(DispararProjetil), 0.5f);
    }
    // No seu script (EnemyRanged ou EnemyController)
    public void DispararProjetil()
    {
        // DEBUG 1: O código chegou aqui?
        Debug.Log("1. Tentando atirar...");

        if (projetilPrefab == null)
        {
            // DEBUG 2: O Prefab sumiu?
            Debug.LogError("ERRO: O campo 'Projetil Prefab' está vazio (None) no Inspector!");
            return;
        }

        if (pontoDeDisparo == null)
        {
            // DEBUG 3: A mão sumiu?
            Debug.LogError("ERRO: O campo 'Ponto De Disparo' está vazio no Inspector!");
            return;
        }

        // Se chegou aqui, VAI criar
        Debug.Log("2. Instanciando agora!");
        Instantiate(projetilPrefab, pontoDeDisparo.position, transform.rotation);
    }
}