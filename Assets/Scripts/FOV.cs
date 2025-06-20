using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewRadius = 10f; // G�r�� menzili
    [Range(0, 360)]
    public float viewAngle = 110f; // G�r�� a��s�
    public LayerMask targetLayer; // Hedefler (player'� i�eriyor)

    public bool CanSeeTarget(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget); // Hedef ile g�r�� a��s�

        if (angle < viewAngle / 2f) // G�r�� a��s� i�inde mi?
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= viewRadius) // Mesafeyi kontrol et
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToTarget.normalized, out hit, viewRadius, targetLayer))
                {
                    if (hit.transform == target) // Hedefi vurmu�sak
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
