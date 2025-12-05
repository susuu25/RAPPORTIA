using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInical : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;

    [SerializeField] private GameObject grupoControles;
    [SerializeField] private GameObject grupoVolume;
    [SerializeField] private GameObject grupoSensiblidade;

     public float mouseSensitivity = 250f;

    public void LoadScene(string cena)
    {
        SceneManager.LoadScene(cena);
    }

    public void SairJogo()
    {
        Application.Quit();
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelMenuInicial.SetActive(true);
        painelOpcoes.SetActive(false);
    }

    public void ShowControles()
    {
        grupoControles.SetActive(true);
        grupoVolume.SetActive(false);
        grupoSensiblidade.SetActive(false);
    }

    public void ShowVolumes()
    {
        grupoControles.SetActive(false);
        grupoVolume.SetActive(true);
        grupoSensiblidade.SetActive(false);
    }

    public void ShowSensibilidade()
    {
        grupoControles.SetActive(false);
        grupoVolume.SetActive(false);
        grupoSensiblidade.SetActive(true);
    }

    public void mouseSensitivitySlider(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.Save();
    }
 
}
