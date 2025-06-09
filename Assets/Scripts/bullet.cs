using UnityEngine;

public class bullet : MonoBehaviour
{
    private Transform target;

    public void Initialize(Transform enemyTarget)
    {
        target = enemyTarget; // Düþmaný hedef al
    }

    private void Update()
    {
        if (target != null)
        {
            // Mermiyi hedefe doðru hareket ettir
            transform.position = Vector3.MoveTowards(transform.position, target.position, 100f * Time.deltaTime);

            // Eðer hedefe ulaþtýysa
            if (Vector3.Distance(transform.position, target.position) < 1f)
            {
                Guard guard = target.GetComponent<Guard>();
                if (guard != null)
                {
                    guard.Die(); // Guard'ýn ölüm sürecini baþlat
                }
                Destroy(gameObject); // Mermiyi yok et
            }
        }
    }
}
