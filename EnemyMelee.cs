using UnityEngine;
using System.Collections;

public class EnemyMelee : EnemyBase
{
    [Header("Combate")]
    public GameObject hitboxEspada;
    public float delayHitbox = 0.4f;
    public float duracaoHitbox = 0.5f;

    protected override void Start()
    {
        base.Start(); // Roda o Start do Pai
        if (hitboxEspada != null) hitboxEspada.SetActive(false);
    }
    protected override void ExecutarAtaque()
    {
        animator.SetTrigger("attack");
        StartCoroutine(AtivarEspada());
    }

    IEnumerator AtivarEspada()
    {
        yield return new WaitForSeconds(delayHitbox);
        if (hitboxEspada != null) hitboxEspada.SetActive(true);

        yield return new WaitForSeconds(duracaoHitbox);
        if (hitboxEspada != null) hitboxEspada.SetActive(false);
    }
}