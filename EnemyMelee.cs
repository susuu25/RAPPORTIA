using UnityEngine;
using System.Collections;

public class EnemyMelee : EnemyBase
{
    [Header("Ataque Espada")]
    public GameObject hitboxEspada;
    public float tempoParaLigarDano = 0.5f;
    public float tempoDanoLigado = 0.2f;

    protected override void Start()
    {
        base.Start();
        // Garante que começa desligado
        hitboxEspada.SetActive(false);
    }

    protected override void ExecutarAtaque()
    {
        animador.SetTrigger("attack");
        StartCoroutine(LigarEspada());
    }

    private IEnumerator LigarEspada()
    {
        yield return new WaitForSeconds(tempoParaLigarDano);

        hitboxEspada.SetActive(true); // Liga o colisor

        yield return new WaitForSeconds(tempoDanoLigado);

        hitboxEspada.SetActive(false); // Desliga o colisor
    }

}