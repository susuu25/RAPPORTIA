using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOver : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject menuCanvas; 
    public playerStats player; 

    // protected para o script filho (Pause) conseguir ler
    protected bool isMenuActive = false;

    protected virtual void Start()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);

        // Garante jogo rodando
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Virtual permite o filho Pause substituir pela lógica do  ESC
    protected virtual void Update()
    {
        if (isMenuActive) return;

        // Lógicado Game Over: Verifica se morreu
        if (player != null && player.IsDead)
        {
            ToggleMenu(true);
        }
    }

    // Ligar/Desligar o menu
    public void ToggleMenu(bool ativar)
    {
        isMenuActive = ativar;

        if (menuCanvas != null) menuCanvas.SetActive(ativar);

        if (ativar)
        {
            Time.timeScale = 0f; // Pausa
            Cursor.lockState = CursorLockMode.None; // Libera mouse
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; // Despausa
            Cursor.lockState = CursorLockMode.Locked; // Trava mouse
            Cursor.visible = false;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}