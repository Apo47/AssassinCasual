using UnityEngine;

public class bullet : MonoBehaviour
{
    private Transform target;

    public void Initialize(Transform enemyTarget)
    {
        target = enemyTarget; // D��man� hedef al
    }

    private void Update()
    {
        if (target != null)
        {
            // Mermiyi hedefe do�ru hareket ettir
            transform.position = Vector3.MoveTowards(transform.position, target.position, 100f * Time.deltaTime);

            // E�er hedefe ula�t�ysa
            if (Vector3.Distance(transform.position, target.position) < 1f)
            {
                Guard guard = target.GetComponent<Guard>();
                if (guard != null)
                {
                    guard.Die(); // Guard'�n �l�m s�recini ba�lat
                }
                Destroy(gameObject); // Mermiyi yok et
            }
        }
    }
}
