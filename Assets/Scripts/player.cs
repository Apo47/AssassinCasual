using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    [Header("Health Settings")]
    [SerializeField] private Image healthBarImage; // Saðlýk barý Image
    private float maxHealth = 1f; // Maksimum can
    private float currentHealth;
    public GameObject healthBar;
    public AudioSource music;
    public AudioSource lose;

    private bool gameStarted = false;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 10f; // Düþman algýlama menzili
    [SerializeField] private float shootingDistance = 10f;
    [SerializeField] private LayerMask enemyLayer; // Düþman layer
    [SerializeField] private GameObject bulletPrefab; // Kurþun Prefabý
    [SerializeField] private GameObject muzzleFlash; // Ateþ etme efekti
    [SerializeField] private AudioSource shootSound; // Ateþ etme sesi

    // Düþmanlarýn ateþ etme sayýsýný tutmak için bir Dictionary
    private Dictionary<Transform, int> enemyShotsFired = new Dictionary<Transform, int>();

    [Header("UI Settings")]
    [SerializeField] private GameObject tryAgainButton; // Try Again butonu

    [Header("Animation Settings")]
    [SerializeField] private Animator anim;

    private bool _isMoving = false;
    private bool _dragStarted = false;
    private Touch _touch;
    private Vector3 _touchDown;
    private Vector3 _touchUp;

    // Lose müziðinin bir kere çalmasýný kontrol etmek için
    private bool hasLosePlayed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Saðlýk barýný baþlat
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }

        // Try Again butonunu baþta kapat
        if (tryAgainButton != null)
        {
            tryAgainButton.SetActive(false);
        }

        // Müzik baþlangýçta çalsýn
        if (music != null && !music.isPlaying)
        {
            music.loop = true; // Arka plan müziði döngüde çalsýn
            music.Play();
        }

        // Lose müziði baþlangýçta kapalý ve döngüsüz
        if (lose != null)
        {
            lose.loop = false; // Lose müziði bir kere çalýp bitsin
            lose.Stop();
        }
    }

    void Update()
    {
        // Eðer oyuncu ölmüþse dokunma iþlemlerini devre dýþý býrak
        if (!IsAlive())
        {
            return;
            // Öldüyse hiçbir iþlem yapýlmaz
        }

        HandleMovement(); // Hareketi devam ettir
        DetectAndShootEnemy(); // Düþman algýlama ve ateþ etme iþlemleri
    }

    public void OnStartButtonClicked()
    {
        gameStarted = true;

        // Oyunu yeniden baþlatýrken
        if (!IsAlive()) // Eðer oyuncu ölmüþse ve tekrar baþlýyorsa
        {
            // Saðlýk ve diðer durumlarý sýfýrla
            currentHealth = maxHealth;
            if (healthBarImage != null)
            {
                healthBarImage.fillAmount = currentHealth / maxHealth;
            }
            healthBar.SetActive(true);
            tryAgainButton.SetActive(false);
            enabled = true; // Script’i yeniden etkinleþtir
            anim.Play("Idle"); // Oyuncu tekrar canlansýn
            hasLosePlayed = false; // Lose müziði tekrar çalabilir hale gelsin

            // Müziði yeniden baþlat
            if (music != null && !music.isPlaying)
            {
                music.Play();
            }
        }
    }

    private void HandleMovement()
    {
        if (!IsAlive()) // Eðer oyuncu ölürse dokunma iþlemleri yapýlmaz
        {
            return; // Hiçbir hareket iþlemi yapýlmaz
        }

        if (gameStarted == true)
        {
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);

                if (_touch.phase == TouchPhase.Began && !_isMoving)
                {
                    _dragStarted = true;
                    _isMoving = true;
                    anim.Play("Run");
                    _touchDown = _touch.position;
                    _touchUp = _touch.position;
                }

                if (_dragStarted)
                {
                    if (_touch.phase == TouchPhase.Moved)
                    {
                        _touchDown = _touch.position;
                    }

                    if (_touch.phase == TouchPhase.Ended)
                    {
                        _isMoving = false;
                        anim.Play("Idle");
                        _dragStarted = false;
                    }

                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        CalculateRotation(),
                        rotationSpeed * Time.deltaTime
                    );
                    transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
                }
            }
        }
    }

    private Quaternion CalculateRotation()
    {
        Vector3 direction = (_touchDown - _touchUp).normalized;
        direction.z = direction.y;
        direction.y = 0;
        return Quaternion.LookRotation(direction, Vector3.up);
    }

    private void DetectAndShootEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distanceToEnemy <= shootingDistance)
            {
                // Eðer bu düþman daha önce ateþ edilmediyse
                if (!enemyShotsFired.ContainsKey(hitCollider.transform) || enemyShotsFired[hitCollider.transform] < 3)
                {
                    Shoot(hitCollider.transform);
                }
            }
        }
    }

    private void Shoot(Transform enemyTransform)
    {
        if (!enemyShotsFired.ContainsKey(enemyTransform))
        {
            enemyShotsFired[enemyTransform] = 0;
        }

        if (enemyShotsFired[enemyTransform] < 3)
        {
            if (shootSound != null)
            {
                shootSound.Play();
            }

            StartCoroutine(MuzzleFlashEffect());

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<bullet>().Initialize(enemyTransform);

            enemyShotsFired[enemyTransform]++;
        }
    }

    private IEnumerator MuzzleFlashEffect()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Caný eksilt
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth; // Health bar'ý güncelle
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0; // Eðer can 0'dan büyükse hayatta
    }

    private void HandleDeath()
    {
        _isMoving = false; // Hareketi engelle
        anim.Play("Die"); // Ölüm animasyonu oynat

        // Müzik kontrolü
        if (music != null && music.isPlaying)
        {
            music.Stop(); // Arka plan müziðini kapat
        }
        if (lose != null && !hasLosePlayed) // Lose bir kere çalsýn
        {
            lose.Play();
            hasLosePlayed = true; // Tekrar çalmasýný engelle
        }

        // Ölümde butonun 2 saniye sonra aktif olmasýný saðlamak için coroutine baþlat
        StartCoroutine(ActivateTryAgainButtonAfterDelay());

        // Öldüðünde touch iþlemelerini tamamen durdurmak için bu iki deðiþkeni sýfýrlayýn
        _dragStarted = false;
        _touchDown = Vector3.zero;
        _touchUp = Vector3.zero;

        // Ayrýca hareket etmeyi engelleyen baþka bir kontrol de eklenebilir
        enabled = false; // Bu script'i devre dýþý býrak, böylece Update ve diðer fonksiyonlar çalýþmaz
    }

    private IEnumerator ActivateTryAgainButtonAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 2 saniye bekle
        if (tryAgainButton != null)
        {
            tryAgainButton.SetActive(true);
            healthBar.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Düþman algýlama menzili için gizmos çizin
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}