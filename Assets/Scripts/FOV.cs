using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewRadius = 10f; // Görüþ menzili
    [Range(0, 360)]
    public float viewAngle = 110f; // Görüþ açýsý
    public LayerMask targetLayer; // Hedefler (player'ý içeriyor)

    public bool CanSeeTarget(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget); // Hedef ile görüþ açýsý

        if (angle < viewAngle / 2f) // Görüþ açýsý içinde mi?
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= viewRadius) // Mesafeyi kontrol et
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToTarget.normalized, out hit, viewRadius, targetLayer))
                {
                    if (hit.transform == target) // Hedefi vurmuþsak
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
