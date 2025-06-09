using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public float detectionRadius = 5.0f; // Lazerin alg�lama menzili
    public float viewAngle = 90.0f; // G�r�� a��s�
    public GameObject bulletPrefab; // Ate�lenecek mermi prefab�
    public Transform firePoint; // Merminin ate�lenece�i nokta
    public LayerMask playerLayer; // Oyuncu layer'�
    public AudioSource shootSound; // Ate� etme sesi
    public GameObject muzzleFlash; // Muzzle flash efekti
    public List<Transform> patrolPoints; // Devriye noktalar�
    public float patrolSpeed = 3.5f; // Devriye h�z�
    private int currentPatrolIndex = 0; // Mevcut devriye noktas� indeksi
    private NavMeshAgent navMeshAgent; // NavMeshAgent referans�
    private Transform playerTransform;
    private int shotsFired = 0; // At�lan mermi say�s�
    private FieldOfView fieldOfView;
    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent bile�eni eksik!");
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
        // Ate� etme sesini �al
        if (shootSound != null)
        {
            shootSound.Play();
            shootSound.Play();
            shootSound.Play();
        }

        // Muzzle flash efektini �al��t�r
        if (muzzleFlash != null)
        {
            StartCoroutine(MuzzleFlashEffect());
        }

        // Mermi olu�tur ve ate�le
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = (playerTransform.position - firePoint.position).normalized * 10.0f; // Mermi h�z�n� ayarlay�n

        // At�lan mermi say�s�n� art�r
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
        // Muzzle flash efektini 3 defa a��p kapat
        for (int i = 0; i < 3; i++)
        {
            // Muzzle flash efektini a�
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
