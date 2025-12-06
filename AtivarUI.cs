using UnityEngine;

public class AtivarBossUI : MonoBehaviour
{
    [Header("UI do Boss")]
    public GameObject bossInterface;

    [Header("Música")]
    public GameObject musicaFase;
    public GameObject musicaBoss;

    [Header("Configuração")]
    private string tagDoPlayer = "Player";

    void Start()
    {
        // Garante que a UI comece desligada
        if (bossInterface != null)
        {
            bossInterface.SetActive(false);
        }

        // Garante que a música do boss não comece tocando antes da hora
        if (musicaBoss != null)
        {
            musicaBoss.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica a tag
        if (other.CompareTag(tagDoPlayer))
        {
            // Ativa a UI
            if (bossInterface != null) bossInterface.SetActive(true);

            // Troca a música
            if (musicaFase != null) musicaFase.SetActive(false); // Para a música antiga
            if (musicaBoss != null) musicaBoss.SetActive(true);  // Toca a nova

            // Destroi o gatilho para não acontecer de novo
            Destroy(gameObject);
        }
    }
}