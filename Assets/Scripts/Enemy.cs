using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; // Oyuncunun transformu
    public float detectionRange = 7f; // Düþmanýn oyuncuyu fark etme mesafesi
    public float moveSpeed = 3f; // Düþmanýn hareket hýzý
    public float shootInterval = 1.5f; // Ateþ etme aralýðý
    public GameObject bulletPrefab; // Kurþun prefabý
    public Transform shootPoint; // Kurþunun fýrlatýlacaðý yer
    public GameObject muzzleFlash; // Ateþ etme efekti için
    public AudioSource au; // Ses efekti

    private bool isPlayerDetected = false; // Oyuncunun algýlanýp algýlanmadýðýný kontrol eder
    private float shootTimer = 0f; // Ateþ etme zamanlayýcýsý
    private player playerScript; // Oyuncunun scripti

    void Start()
    {
        // Oyuncunun player scriptine eriþim
        if (player != null)
        {
            playerScript = player.GetComponent<player>();
        }
    }

    void Update()
    {
        // Eðer oyuncu ölü deðilse düþman iþlevselliklerine devam eder
        if (playerScript != null && playerScript.IsAlive())
        {
            DetectPlayer();

            if (isPlayerDetected)
            {
                ShootPlayer();
                MoveTowardsPlayer();
            }
        }
        else
        {
            // Oyuncu öldüyse hareket ve ateþi durdur
            isPlayerDetected = false;
        }
    }

    void DetectPlayer()
    {
        // Oyuncunun düþmana olan mesafesini kontrol et
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    void MoveTowardsPlayer()
    {
        // Oyuncuya doðru hareket et
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Oyuncuya doðru dön
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ShootPlayer()
    {
        // Ateþ etme zamanlayýcýsýný kontrol et
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            // Kurþun oluþtur
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            au.Play();

            // Bullet scriptine hedef olarak player'ý ata
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.target = player; // Player'ý hedef olarak ata
            }

            // Muzzle flash efektini göster
            StartCoroutine(ShowMuzzleFlash());

            shootTimer = 0f; // Zamanlayýcýyý sýfýrla
        }
    }

    System.Collections.IEnumerator ShowMuzzleFlash()
    {
        // Ateþ etme efektini kýsa süreliðine aktif et
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }
}
