using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FieldOfView : MonoBehaviour
{
    public float viewRadius; // G�r�� yar��ap�
    [Range(0, 360)]
    public float viewAngle; // G�r�� a��s�
    public LayerMask targetMask; // Hedef layer'�
    public LayerMask obstacleMask; // Engel layer'�
    private LineRenderer lineRenderer; // LineRenderer referans�

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer bile�enini al
        lineRenderer.positionCount = 0; // Ba�lang��ta hi� nokta yok

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
                    // Hedef g�r�lebilir
                    // Burada hedefe ate� edebilirsiniz veya ba�ka bir i�lem yapabilirsiniz
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
                // Engellere �arpmad�, bu nokta g�r�� alan�nda
                points.Add(transform.position + new Vector3(direction.x, direction.y, 0) * viewRadius);
            }
            else
            {
                // Engellere �arpt�, bu nokta g�r�� alan�n�n sonu
                points.Add(hit.point);
            }

            angle -= angleIncrease;
        }

        // LineRenderer ile g�r�� alan�n� olu�tur
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
