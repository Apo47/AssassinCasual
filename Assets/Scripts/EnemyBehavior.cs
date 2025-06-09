using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public float detectionRadius = 5.0f; // Lazerin algýlama menzili
    public float viewAngle = 90.0f; // Görüþ açýsý
    public GameObject bulletPrefab; // Ateþlenecek mermi prefabý
    public Transform firePoint; // Merminin ateþleneceði nokta
    public LayerMask playerLayer; // Oyuncu layer'ý
    public AudioSource shootSound; // Ateþ etme sesi
    public GameObject muzzleFlash; // Muzzle flash efekti
    public List<Transform> patrolPoints; // Devriye noktalarý
    public float patrolSpeed = 3.5f; // Devriye hýzý
    private int currentPatrolIndex = 0; // Mevcut devriye noktasý indeksi
    private NavMeshAgent navMeshAgent; // NavMeshAgent referansý
    private Transform playerTransform;
    private int shotsFired = 0; // Atýlan mermi sayýsý
    private FieldOfView fieldOfView;
    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent bileþeni eksik!");
            return;
        }
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        fieldOfView = GetComponentInChildren<FieldOfView>();
        if (fieldOfView != null)
        {
            fieldOfView.viewRadius = detectionRadius;
            fieldOfView.viewAngle = viewAngle;
        }
    }

    void Update()
    {
        if (navMeshAgent == null)
            return;

        Patrol();

        if (PlayerInFieldOfView() && shotsFired < 3)
        {
            FireAtPlayer();
        }
    }

    void Patrol()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void FireAtPlayer()
    {
        // Ateþ etme sesini çal
        if (shootSound != null)
        {
            shootSound.Play();
            shootSound.Play();
            shootSound.Play();
        }

        // Muzzle flash efektini çalýþtýr
        if (muzzleFlash != null)
        {
            StartCoroutine(MuzzleFlashEffect());
        }

        // Mermi oluþtur ve ateþle
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = (playerTransform.position - firePoint.position).normalized * 10.0f; // Mermi hýzýný ayarlayýn

        // Atýlan mermi sayýsýný artýr
        shotsFired++;
    }

    bool PlayerInFieldOfView()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator MuzzleFlashEffect()
    {
        // Muzzle flash efektini 3 defa açýp kapat
        for (int i = 0; i < 3; i++)
        {
            // Muzzle flash efektini aç
            muzzleFlash.SetActive(true);

            // 0.1 saniye bekle
            yield return new WaitForSeconds(0.1f);

            // Muzzle flash efektini kapat
            muzzleFlash.SetActive(false);

            // 0.1 saniye bekle
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && shotsFired < 3)
        {
            FireAtPlayer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // playerDetected = false;
        }
    }
}
