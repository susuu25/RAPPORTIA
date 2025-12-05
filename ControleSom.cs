using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class ControleSom : MonoBehaviour
{
    private bool estadoSom = true;
    [SerializeField] private AudioSource fundoMusica;

    [SerializeField] private Sprite somLigadoSprite;
    [SerializeField] private Sprite somMutadoSprite;

    [SerializeField] private Image muteImage;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            fundoMusica.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            fundoMusica.volume = 0.5f;
        }
    }

    public void LigarDesligarSom()
    {
        estadoSom = !estadoSom;
        fundoMusica.enabled = estadoSom;

        if (estadoSom)
        {
            muteImage.sprite = somLigadoSprite;
        }
        else
        {
            muteImage.sprite = somMutadoSprite;
        }
    }

    public void VolumeMusica (float value)
    {
        fundoMusica.volume = value;

        PlayerPrefs.SetFloat("Volume", fundoMusica.volume);
    }
}
