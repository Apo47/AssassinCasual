using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action onGuardSpottedPlayer;

    public Transform player;
    public LayerMask mask;
    public Light spotlight;
    public LineRenderer laser; // Lazer çizgisi için
    public float laserDuration = 0.2f; // Lazerin görünme süresi
    public float wiewDistance;
    public float toTimeSpotPlayer = .5f;
    public float playerVisibleTimer;
    public float speed = 15;
    public float waitTime = .3f;
    public float turningSpeed = 10;
    public Transform pathHolder;

    private Color orginSpotlight;
    private float wiewAngle;
    private int laserCount = 0; // Lazer sayacı
    private bool isShooting = false; // Lazer atarken kontrol için

    private Vector3[] waypoints;
    private int targetWaypointIndex = 0;
    private Vector3 targetWaypoint;
    public Animator animator; // Ölüm animasyonu için
    private bool isDead = false; // Ölüm kontrolü

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        orginSpotlight = spotlight.color;
        wiewAngle = spotlight.spotAngle;

        // Path waypoint dizisini oluştur
        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);  // Y düzleminde hizala
        }
        targetWaypoint = waypoints[targetWaypointIndex];
        StartCoroutine(FollowPath());  // Path takibini başlat
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, toTimeSpotPlayer);
        spotlight.color = Color.Lerp(orginSpotlight, Color.red, playerVisibleTimer / toTimeSpotPlayer);

        if (playerVisibleTimer >= toTimeSpotPlayer && !isShooting)
        {
            StartCoroutine(ShootLaser());
        }
    }

    public void Die()
    {
        if (isDead) return; // Zaten ölmüşse tekrar çalışmasın
        isDead = true;

        // Ateş etmeyi durdur
        StopAllCoroutines();
        isShooting = false;

        // Ölüm animasyonunu başlat
        if (animator != null)
        {
            animator.Play("Die");
        }

        // Ölüm animasyonu bittikten sonra yok et
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        // Ölüm animasyonu süresince bekle (örneğin 2 saniye)
        yield return new WaitForSeconds(2.5f);

        // Guard objesini yok et
        Destroy(gameObject);
    }
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < wiewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < wiewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, mask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator ShootLaser()
    {
        isShooting = true;

        // Lazer çiz
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, player.position);
        laser.enabled = true;

        // Lazerin belirli bir süre aktif olması
        yield return new WaitForSeconds(laserDuration);
        laser.enabled = false;

        laserCount++; // Lazer sayacını artır
        if (laserCount >= 0)
        {
            // Player scriptinden TakeDamage fonksiyonunu çağırarak hasar ver
            player playerScript = player.GetComponent<player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(0.4f); 
            }
        }

        yield return new WaitForSeconds(0.5f); // Lazer atış arası
        isShooting = false;
    }

    IEnumerator FollowPath()
    {
        while (true)
        {
            // Yolu takip et
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

            // Hedefe ulaşıldığında bir sonraki hedefe geç
            if (Vector3.Distance(transform.position, targetWaypoint) < 1f)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
            }

            // Yönelim yapmak için hedefe bak
            Vector3 directionToWaypoint = (targetWaypoint - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToWaypoint.x, directionToWaypoint.z) * Mathf.Rad2Deg;

            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turningSpeed * Time.deltaTime);

            // Eğer player'ı görüyorsa, path takibini bırakıp player'ı takip et
            if (CanSeePlayer())
            {
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPlayer());
                yield break;  // Player'ı gördüğümüzde path takibini sonlandırıyoruz
            }

            yield return null;
        }
    }

    IEnumerator FollowPlayer()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            animator.Play("gun");
            // Player'a bakmak için döndür
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(dirToPlayer.x, dirToPlayer.z) * Mathf.Rad2Deg;

            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turningSpeed * Time.deltaTime);

            // Eğer player tekrar kaybolursa, path'e geri dön
            if (!CanSeePlayer())
            {
                StartCoroutine(FollowPath());  // Player'ı kaybettiğinde path'e geri dön
                yield break;
            }

            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPos = startPos;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }

        Gizmos.DrawLine(previousPos, startPos);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, wiewDistance * transform.forward);
    }
}
