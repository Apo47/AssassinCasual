using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Transform target;  // Hedef referans� (player)
    public float speed = 10f; // Kur�un h�z�
    private float lifeTime = 5f; // Kur�unun ya�am s�resi
    private float timer = 0f; // Zamanlay�c� (kur�un ya�ama s�resi)

    void Update()
    {
        if (target != null)
        {
            // Hedefe do�ru hareket et
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Hedefe �arpma kontrol� (iste�e ba�l�)
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < 0.1f)
            {
                Destroy(gameObject);  // Hedefe �arpt���nda kur�unu yok et
            }
        }

        // Kur�unun ya�am s�resi doldu�unda yok et
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);  // Zaman doldu�unda kur�unu yok et
        }
    }
}
