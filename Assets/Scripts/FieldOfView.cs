using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FieldOfView : MonoBehaviour
{
    public float viewRadius; // Görüþ yarýçapý
    [Range(0, 360)]
    public float viewAngle; // Görüþ açýsý
    public LayerMask targetMask; // Hedef layer'ý
    public LayerMask obstacleMask; // Engel layer'ý
    private LineRenderer lineRenderer; // LineRenderer referansý

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer bileþenini al
        lineRenderer.positionCount = 0; // Baþlangýçta hiç nokta yok

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.right, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // Hedef görülebilir
                    // Burada hedefe ateþ edebilirsiniz veya baþka bir iþlem yapabilirsiniz
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int rayCount = 50;
        float angle = 0f;
        float angleIncrease = viewAngle / rayCount;

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= rayCount; i++)
        {
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewRadius, obstacleMask);

            if (hit.collider == null)
            {
                // Engellere çarpmadý, bu nokta görüþ alanýnda
                points.Add(transform.position + new Vector3(direction.x, direction.y, 0) * viewRadius);
            }
            else
            {
                // Engellere çarptý, bu nokta görüþ alanýnýn sonu
                points.Add(hit.point);
            }

            angle -= angleIncrease;
        }

        // LineRenderer ile görüþ alanýný oluþtur
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
