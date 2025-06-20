using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Transform target;  // Hedef referansý (player)
    public float speed = 10f; // Kurþun hýzý
    private float lifeTime = 5f; // Kurþunun yaþam süresi
    private float timer = 0f; // Zamanlayýcý (kurþun yaþama süresi)

    void Update()
    {
        if (target != null)
        {
            // Hedefe doðru hareket et
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Hedefe çarpma kontrolü (isteðe baðlý)
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < 0.1f)
            {
                Destroy(gameObject);  // Hedefe çarptýðýnda kurþunu yok et
            }
        }

        // Kurþunun yaþam süresi dolduðunda yok et
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);  // Zaman dolduðunda kurþunu yok et
        }
    }
}
