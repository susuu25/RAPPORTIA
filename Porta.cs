using UnityEngine;

public class Porta : MonoBehaviour
{
    [Header("Portas")]
    public GameObject portaFechada;
    public GameObject portaAberta;

    [Header("UI Nova Habilidade")]
    public GameObject painelHabilidade;

    void Start()
    {
        if (portaFechada != null) portaFechada.SetActive(true);
        if (portaAberta != null) portaAberta.SetActive(false);
        if (painelHabilidade != null) painelHabilidade.SetActive(false);
    }

    public void AbrirSaida()
    {
        // Troca as portas
        if (portaFechada != null) portaFechada.SetActive(false);
        if (portaAberta != null) portaAberta.SetActive(true);

        // Ativa o aviso da nova habilidade
        if (painelHabilidade != null)
        {
            painelHabilidade.SetActive(true);
        }
    }
}