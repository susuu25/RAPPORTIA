using UnityEngine;
using System;
using System.Collections;

public class otimizacao : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo em segundos entre cada limpeza de memória.")]
    public float intervaloEmSegundos = 60f; // Padrão: 60 segundos

    [Tooltip("Marque para ver mensagens no Console quando a limpeza acontecer.")]
    public bool mostrarLog = true;

    private void Start()
    {
        // Inicia o processo automático assim que o jogo começa
        StartCoroutine(RotinaDeLimpeza());
    }

    private IEnumerator RotinaDeLimpeza()
    {
        while (true) // Loop infinito
        {
            // Espera o tempo definido antes de rodar novamente
            yield return new WaitForSeconds(intervaloEmSegundos);

            if (mostrarLog)
                Debug.Log($"[Limpador] Iniciando limpeza... Memória atual: {ObterMemoriaAtual()} MB");

            // 1. Descarrega assets da Unity (Texturas, Sons, etc) que não estão mais em cena
            yield return Resources.UnloadUnusedAssets();

            // 2. Força o Coletor de Lixo (Garbage Collector) a limpar variáveis soltas
            System.GC.Collect();

            // Opcional: Espera os finalizadores terminarem (garante limpeza profunda)
            System.GC.WaitForPendingFinalizers();

            if (mostrarLog)
                Debug.Log($"[Limpador] Limpeza concluída. Memória após limpeza: {ObterMemoriaAtual()} MB");
        }
    }

    // Função auxiliar para converter bytes em Megabytes (apenas visual)
    private float ObterMemoriaAtual()
    {
        return (float)System.GC.GetTotalMemory(false) / (1024 * 1024);
    }
}