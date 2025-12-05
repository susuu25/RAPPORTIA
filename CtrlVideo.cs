using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CtrlVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string NomeCena = "NomeCena"; 

    void Start()
    {
        // Inscreve a nossa função no evento do video terminar
        videoPlayer.loopPointReached += AoTerminarVideo;
    }

    void Update()
    {
        // Pular o vídeo se apertar Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CarregarJogo();
        }
    }

    void AoTerminarVideo(VideoPlayer vp)
    {
        CarregarJogo();
    }

    void CarregarJogo()
    {
        SceneManager.LoadScene(NomeCena);
    }
}