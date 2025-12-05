using UnityEngine;
public class Dano : MonoBehaviour
{
    [SerializeField] private float dano;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.ReceberDano(dano);
                Debug.Log("Arma causou " + dano + " de dano.");
            }
        }
    }
}