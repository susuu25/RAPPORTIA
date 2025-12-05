using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocaCena : MonoBehaviour
{
    public string nomeCena;
    private string tagPlayer = "Player";
    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            SceneManager.LoadScene(nomeCena);
        }
    }
}
