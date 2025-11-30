using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SeqFinal : MonoBehaviour
{
    public GameObject canvasFinal;

    void Start()
    {
        if (canvasFinal != null)
        {
            canvasFinal.SetActive(false);
        }

        // Inicia a sequência automática
        StartCoroutine(SequenciaDeEncerramento());
    }

    IEnumerator SequenciaDeEncerramento()
    {
        // 1. Espera os primeiros 10 segundos
        yield return new WaitForSeconds(10f);

        // 2. Ativa o Canvas (mostrando a mensagem/imagem final)
        if (canvasFinal != null)
        {
            canvasFinal.SetActive(true);
        }

        // 3. Espera mais 5 segundos com a mensagem na tela
        yield return new WaitForSeconds(5f);

        // 4. Carrega a próxima cena (Menu ou Créditos)
        SceneManager.LoadScene("Menu");
    }
}