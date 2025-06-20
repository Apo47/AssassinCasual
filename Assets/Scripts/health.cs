using UnityEngine;

public class health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5; // Maksimum can
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Baþlangýçta maksimum cana sahip
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Alýnan hasarý uygula
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
