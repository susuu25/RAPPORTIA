using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Geral")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;

    protected override void ExecutarAtaque()
    {
        animator.SetTrigger("attack");

        // Força o tiro a sair em 0.5 segundos, sem depender da animação
        Invoke(nameof(DispararProjetil), 0.5f);
    }
    public void DispararProjetil()
    {
        Instantiate(projetilPrefab, pontoDeDisparo.position, transform.rotation);
    }
}