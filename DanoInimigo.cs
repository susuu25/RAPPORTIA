using UnityEngine;

public class DanoInimigo : MonoBehaviour
{
    [Header("Configura��o")]
    public float danoAoPlayer = 10f;
    public bool ehProjetil = false;

    private bool jaDeuDano = false;
    private Rigidbody corpoRigido;

    void OnEnable()
    {
        jaDeuDano = false;
    }

    void Start()
    {
        corpoRigido = GetComponent<Rigidbody>();

        if (ehProjetil)
            Destroy(gameObject, 5f);
    }

    void Update()
    {
        // ? Projetil anda com f�sica (evita leaks do Translate)
        if (ehProjetil && corpoRigido != null)
        {
            corpoRigido.linearVelocity = transform.forward * 30f;
        }
    }

    void OnTriggerEnter(Collider outroObjeto)
    {
        if (jaDeuDano && !ehProjetil) return;

        if (outroObjeto.CompareTag("Player"))
        {
            playerStats vidaDoJogador = outroObjeto.GetComponent<playerStats>();

            if (vidaDoJogador != null)
            {
                vidaDoJogador.EnableDamagePostProcess();
                vidaDoJogador.TakeDamage(danoAoPlayer);
                jaDeuDano = true;
            }

            if (ehProjetil)
                Destroy(gameObject);
        }
        else if (!outroObjeto.CompareTag("Enemy") && !outroObjeto.isTrigger)
        {
            if (ehProjetil)
                Destroy(gameObject);
        }
    }
}
