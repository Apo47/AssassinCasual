using UnityEngine;

public class health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5; // Maksimum can
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Ba�lang��ta maksimum cana sahip
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Al�nan hasar� uygula
        Debug.Log($"{gameObject.name} has {currentHealth} health remaining.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); // Hedefi yok et
    }
}
