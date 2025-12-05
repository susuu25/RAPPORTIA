using UnityEngine;
using System.Collections;

public class EnemyRanged : EnemyBase
{
    [Header("Ataque Tiro")]
    public GameObject projetilPrefab;
    public Transform saidaDoTiro; // Ponto de disparo
    public float tempoParaAtirar = 0.5f;

    protected override void ExecutarAtaque()
    {
        animador.SetTrigger("attack");
        StartCoroutine(Disparar());
    }

    private IEnumerator Disparar()
    {
        yield return new WaitForSeconds(tempoParaAtirar);

        // Cria o tiro na posição do ponto de disparo
        Instantiate(projetilPrefab, saidaDoTiro.position, transform.rotation);
    }
}