using UnityEngine;

public class agua : MonoBehaviour
{
    public Transform posicao;
    CharacterController character;

    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("Water"))
        {
            ForaDoMapa();
        }
    }

    void ForaDoMapa()
    {
        character.enabled = false;
        transform.position = posicao.position;
        character.enabled = true;
    }
}