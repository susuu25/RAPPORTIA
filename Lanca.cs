using UnityEngine;

public class Lanca : Weapon
{
    // Variáveis públicas
    [Header("Configurações de Arremesso")]
    public float velocidade = 60f;    // Força com que a lança é jogada
    public float tempoMaximo = 5f;    // Tempo de segurança: se jogar pro céu, volta depois desse tempo

    [Header("Ciclo de Retorno")]
    public float tempoFincada = 1.0f; // Quanto tempo ela fica visível e parada na parede após bater
    public float tempoRespawn = 2.0f; // Quanto tempo ela fica "invisível" (sumida) antes de reaparecer na mão

    [Header("Ajuste Visual")]
    public Vector3 angulacao = new Vector3(90, 0, 0); // Rotação da lança quando está voando (ponta para frente)
    // Removida a variável 'suavidade' pois a rotação será instantânea

    [Header("Combate")]
    public Collider triggerDano;

    // Variáveis privadas
    private Transform visual;         // Referência á malha visual da lança (para girar)
    private Rigidbody rb;             // Componente de física (adicionado/removido dinamicamente)
    private Collider colisor;         // A caixa de colisão da lança (física - para bater na parede)
    private Helia player;             // Referência ao script da personagem
    private Collider colisorPlayer;   // Referência ao corpo da personagem (para não bater nela)

    // Controle de estados 
    private bool arremessada = false; // Está voando?
    private bool naMao = true;        // Está equipada?
    private bool retornando = false;  // Está no processo de voltar?
    private bool colidiu = false;     // Já bateu em algo?

    // Dados Originais (Salvos no Start para saber como resetar a lança na mão)
    private Vector3 escalaOrig;
    private Vector3 posOrig;
    private Quaternion rotOrigVisual; // Rotação original da malha 
    private Quaternion rotOrigMao;    // Rotação original da pegada

    void Start()
    {
        // Busca o colisor automaticamente (pode estar no pai ou no filho)
        // Será usado para física (bater na parede)
        colisor = GetComponentInChildren<Collider>();

        // Garante que o trigger de dano comece desligado para não machucar ninguém enquanto está na mão
        if (triggerDano != null) triggerDano.enabled = false;

        // Busca o objeto visual (o primeiro filho do lancemature)
        if (transform.childCount > 0) visual = transform.GetChild(0);

        // Salva como a lança é quando está na mão (Backup)
        escalaOrig = transform.localScale;
        posOrig = transform.localPosition;
        rotOrigMao = transform.localRotation;
        if (visual != null) rotOrigVisual = visual.localRotation;

        // Procura o jogador na cena pela Tag "Player"
        GameObject objPlayer = GameObject.FindWithTag("Player");
        if (objPlayer != null)
        {
            player = objPlayer.GetComponent<Helia>();
            colisorPlayer = objPlayer.GetComponent<Collider>();
        }

        // Garante que começa sem física (estado de repouso)
        ConfigurarFisica(false);
    }

    void Update()
    {
        // Se não tem visual ou ele está desligado (lança destruída), não faz nada
        if (visual == null || !visual.gameObject.activeSelf) return;

        // Verifica se a Helia está mirando
        bool mirando = (player != null && player.IsAiming());

        // Rotação da lança:
        // Se estiver voando OU mirando -> Define a angulação de ataque (90 graus)
        // Se estiver parada na mão -> Mantém a rotação original
        Quaternion alvo = (arremessada || mirando) ? Quaternion.Euler(angulacao) : rotOrigVisual;

        // Aplica a rotação
        visual.localRotation = alvo;
    }

    // Liga ou Desliga a física completamente para evitar bugs
    void ConfigurarFisica(bool ativar)
    {
        if (ativar)
        {
            // Liga colisor e adiciona Rigidbody para voar
            if (colisor != null) colisor.enabled = true;

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                // Configurações para colisão rápida e precisa
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // Destrava o movimento
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        else
        {
            // Desliga o colisor físico
            if (colisor != null) colisor.enabled = false;

            // Desliga também o trigger de dano
            if (triggerDano != null) triggerDano.enabled = false;

            // Destrói o Rigidbody
            if (rb != null) { Destroy(rb); rb = null; }

            // Garante que a escala não deformou
            transform.localScale = escalaOrig;
        }
    }

    // Chamado pelo script da Helia para alinhar a mira
    public void SetLaunchDirection(Vector3 dir)
    {
        transform.forward = dir.normalized;
    }

    // Arremesso da lança
    public override void Fire()
    {
        // Se já jogou ou não está na mão, ignora
        if (arremessada || !naMao) return;

        // Atualiza estados
        arremessada = true;
        naMao = false;
        colidiu = false;

        // Garante que está visível
        if (visual != null) visual.gameObject.SetActive(true);

        transform.SetParent(null); // Solta da mão

        ConfigurarFisica(true);    // Liga a física e gravidade

        // Ativa o dano
        if (triggerDano != null) triggerDano.enabled = true;

        // Ignora colisão entre a lança e a Helia para não travar na saída
        if (colisorPlayer != null)
        {
            if (colisor != null) Physics.IgnoreCollision(colisor, colisorPlayer, true);
            if (triggerDano != null) Physics.IgnoreCollision(triggerDano, colisorPlayer, true);
        }

        // Empurra a lança para frente
        if (rb != null)
        {
            rb.isKinematic = false; // Ativa física
            rb.linearVelocity = transform.forward * velocidade;
        }

        // Inicia o timer de segurança (volta em 5s se não bater em nada)
        Invoke(nameof(IniciarRetorno), tempoMaximo);
    }

    // Colisão com algo (Física)
    void OnCollisionEnter(Collision hit)
    {
        // Validações para não processar colisão errada
        if (!arremessada || retornando || colidiu) return;
        if (hit.gameObject.CompareTag("Player")) return; // Ignora player

        colidiu = true; // Marca que bateu
        CancelInvoke(nameof(IniciarRetorno)); // Cancela o timer de 5s

        // 1. Para a física imediatamente (Finca na parede)
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Trava posição
        }

        // 2. Desliga o colisor físico imediatamente (para não atrapalhar navegação)
        if (colisor != null) colisor.enabled = false;

        // Não desliga o dano agora. Espera 0.1s para garantir que o Trigger funcione
        // se bateu no inimigo ao mesmo tempo que bateu na "parede/chão".
        Invoke(nameof(DesligarDano), 0.1f);

        // 3. Agenda o "sumiço visual" daqui a X segundos
        Invoke(nameof(SimularDestruicao), tempoFincada);
    }

    // Método auxiliar para desligar o dano com atraso
    void DesligarDano()
    {
        if (triggerDano != null) triggerDano.enabled = false;
    }

    // Desativar a lança visualmente
    void SimularDestruicao()
    {
        // Desliga o desenho 3D (parece que foi destruída)
        if (visual != null) visual.gameObject.SetActive(false);

        // Agenda o retorno final para a mão daqui a X segundos (tempoRespawn)
        Invoke(nameof(IniciarRetorno), tempoRespawn);
    }

    // Retorna a lança para a mão do player
    void IniciarRetorno()
    {
        // Só volta se realmente estiver longe e ainda não estiver voltando
        if (arremessada && !retornando && player != null)
            RetornarParaMao(player.handTransform);
    }

    void RetornarParaMao(Transform mao)
    {
        retornando = true;
        CancelInvoke(); // Limpa qualquer timer pendente para evitar bugs

        ConfigurarFisica(false); // Remove a física completamente (e desliga triggerDano)

        // Permite colidir com o player de novo no futuro (reseta ignores)
        if (colisorPlayer != null)
        {
            if (colisor != null) Physics.IgnoreCollision(colisor, colisorPlayer, false);
            if (triggerDano != null) Physics.IgnoreCollision(triggerDano, colisorPlayer, false);
        }

        // Teleporta para mão
        transform.SetParent(mao);
        transform.localPosition = posOrig;      // Reseta posição local
        transform.localRotation = rotOrigMao;   // Reseta rotação local
        transform.localScale = escalaOrig;      // Reseta escala

        // Reaparece visualmente
        if (visual != null)
        {
            visual.localRotation = rotOrigVisual; // Reseta rotação do desenho
            visual.gameObject.SetActive(true);    // Torna visível novamente
        }

        // Reseta todas as flags para o estado inicial
        arremessada = false;
        retornando = false;
        naMao = true;
        colidiu = false;

        // Avisa a Helia que a arma voltou (para liberar troca de arma, etc)
        if (player != null) player.OnLanceReturned();
    }

    public override void OnEquip()
    {
        // Segurança: Se trocar de arma, reseta tudo
        if (arremessada) return;
        arremessada = false;
        retornando = false;
        naMao = true;
        ConfigurarFisica(false);
        if (visual != null) visual.gameObject.SetActive(true);
    }

    public bool IsLaunched() => arremessada;
}