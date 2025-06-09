using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; // Oyuncunun transformu
    public float detectionRange = 7f; // D��man�n oyuncuyu fark etme mesafesi
    public float moveSpeed = 3f; // D��man�n hareket h�z�
    public float shootInterval = 1.5f; // Ate� etme aral���
    public GameObject bulletPrefab; // Kur�un prefab�
    public Transform shootPoint; // Kur�unun f�rlat�laca�� yer
    public GameObject muzzleFlash; // Ate� etme efekti i�in
    public AudioSource au; // Ses efekti

    private bool isPlayerDetected = false; // Oyuncunun alg�lan�p alg�lanmad���n� kontrol eder
    private float shootTimer = 0f; // Ate� etme zamanlay�c�s�
    private player playerScript; // Oyuncunun scripti

    void Start()
    {
        // Oyuncunun player scriptine eri�im
        if (player != null)
        {
            playerScript = player.GetComponent<player>();
        }
    }

    void Update()
    {
        // E�er oyuncu �l� de�ilse d��man i�levselliklerine devam eder
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
            // Oyuncu �ld�yse hareket ve ate�i durdur
            isPlayerDetected = false;
        }
    }

    void DetectPlayer()
    {
        // Oyuncunun d��mana olan mesafesini kontrol et
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
        // Oyuncuya do�ru hareket et
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Oyuncuya do�ru d�n
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ShootPlayer()
    {
        // Ate� etme zamanlay�c�s�n� kontrol et
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            // Kur�un olu�tur
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            au.Play();

            // Bullet scriptine hedef olarak player'� ata
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.target = player; // Player'� hedef olarak ata
            }

            // Muzzle flash efektini g�ster
            StartCoroutine(ShowMuzzleFlash());

            shootTimer = 0f; // Zamanlay�c�y� s�f�rla
        }
    }

    System.Collections.IEnumerator ShowMuzzleFlash()
    {
        // Ate� etme efektini k�sa s�reli�ine aktif et
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }
}
