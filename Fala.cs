using UnityEngine;
using TMPro;

public class Fala : MonoBehaviour
{
    // Array público para armazenar 5 componentes TextMeshProUGUI, cada um com uma fala
    public TextMeshProUGUI[] dialogos = new TextMeshProUGUI[5];
    // Referência ao GameObject do painel que contém os textos de diálogo
    public GameObject painel;
    // Indica se o jogador está dentro do colisor (área de gatilho)
    private bool perto = false;
    // Indica se o diálogo está ativo (pronto para avançar com a tecla)
    private bool ativo = false;
    // Índice da fala atual no array dialogos (0 a 4 para 5 falas)
    private int falaAtual = 0;
    // Variável pública que indica se todas as falas foram exibidas
    public bool condicao = false;
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou tem a tag "Player" e se os diálogos ainda não terminaram
        if (other.CompareTag("Player") && !condicao)
        {
            perto = true; // Marca que o jogador está dentro do colisor
            painel.SetActive(true); // Ativa o painel de diálogo
            AtivarFala(falaAtual); // Ativa a fala correspondente ao índice atual
            ativo = true; // Marca o diálogo como ativo para permitir avanço com tecla
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            perto = false; // Marca que o jogador saiu do colisor
            painel.SetActive(false); // Desativa o painel de diálogo
            DesativarTodasFalas(); // Desativa todos os textos de diálogo
            ativo = false; // Desmarca o diálogo como ativo
            // Reseta o diálogo para a primeira fala, mas apenas se ainda não terminou
            if (!condicao)
            {
                falaAtual = 0; // Volta para a primeira fala (índice 0)
            }
        }
    }
    void Start()
    {
        painel.SetActive(false); // Garante que o painel começa desativado
        DesativarTodasFalas(); // Garante que todos os textos de diálogo começam desativados
    }
    void Update()
    {
        if (perto && ativo && Input.GetKeyDown(KeyCode.F) && !condicao)
        {
            falaAtual++; // Avança para a próxima fala
            // Verifica se ainda há falas disponíveis no array
            if (falaAtual < dialogos.Length)
            {
                DesativarTodasFalas(); // Desativa o texto atual
                AtivarFala(falaAtual); // Ativa o próximo texto
            }
            else
            {
                // Se não houver mais falas, finaliza o diálogo
                DesativarTodasFalas(); // Desativa todos os textos
                painel.SetActive(false); // Desativa o painel
                ativo = false; // Desmarca o diálogo como ativo
                condicao = true; // Marca que os diálogos terminaram
            }
        }
    }
    private void AtivarFala(int indice)
    {
        // Verifica se o índice é válido (dentro dos limites do array)
        if (indice >= 0 && indice < dialogos.Length)
        {
            // Ativa o GameObject do TextMeshProUGUI correspondente
            dialogos[indice].gameObject.SetActive(true);
        }
    }
    private void DesativarTodasFalas()
    {
        // Itera sobre cada TextMeshProUGUI no array
        foreach (TextMeshProUGUI dialogo in dialogos)
        {
            // Desativa o GameObject de cada texto
            dialogo.gameObject.SetActive(false);
        }
    }
}