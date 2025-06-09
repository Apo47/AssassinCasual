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
    [SerializeField] private Image healthBarImage; // Sa�l�k bar� Image
    private float maxHealth = 1f; // Maksimum can
    private float currentHealth;
    public GameObject healthBar;
    public AudioSource music;
    public AudioSource lose;

    private bool gameStarted = false;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 10f; // D��man alg�lama menzili
    [SerializeField] private float shootingDistance = 10f;
    [SerializeField] private LayerMask enemyLayer; // D��man layer
    [SerializeField] private GameObject bulletPrefab; // Kur�un Prefab�
    [SerializeField] private GameObject muzzleFlash; // Ate� etme efekti
    [SerializeField] private AudioSource shootSound; // Ate� etme sesi

    // D��manlar�n ate� etme say�s�n� tutmak i�in bir Dictionary
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

    // Lose m�zi�inin bir kere �almas�n� kontrol etmek i�in
    private bool hasLosePlayed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Sa�l�k bar�n� ba�lat
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }

        // Try Again butonunu ba�ta kapat
        if (tryAgainButton != null)
        {
            tryAgainButton.SetActive(false);
        }

        // M�zik ba�lang��ta �als�n
        if (music != null && !music.isPlaying)
        {
            music.loop = true; // Arka plan m�zi�i d�ng�de �als�n
            music.Play();
        }

        // Lose m�zi�i ba�lang��ta kapal� ve d�ng�s�z
        if (lose != null)
        {
            lose.loop = false; // Lose m�zi�i bir kere �al�p bitsin
            lose.Stop();
        }
    }

    void Update()
    {
        // E�er oyuncu �lm��se dokunma i�lemlerini devre d��� b�rak
        if (!IsAlive())
        {
            return;
            // �ld�yse hi�bir i�lem yap�lmaz
        }

        HandleMovement(); // Hareketi devam ettir
        DetectAndShootEnemy(); // D��man alg�lama ve ate� etme i�lemleri
    }

    public void OnStartButtonClicked()
    {
        gameStarted = true;

        // Oyunu yeniden ba�lat�rken
        if (!IsAlive()) // E�er oyuncu �lm��se ve tekrar ba�l�yorsa
        {
            // Sa�l�k ve di�er durumlar� s�f�rla
            currentHealth = maxHealth;
            if (healthBarImage != null)
            {
                healthBarImage.fillAmount = currentHealth / maxHealth;
            }
            healthBar.SetActive(true);
            tryAgainButton.SetActive(false);
            enabled = true; // Script�i yeniden etkinle�tir
            anim.Play("Idle"); // Oyuncu tekrar canlans�n
            hasLosePlayed = false; // Lose m�zi�i tekrar �alabilir hale gelsin

            // M�zi�i yeniden ba�lat
            if (music != null && !music.isPlaying)
            {
                music.Play();
            }
        }
    }

    private void HandleMovement()
    {
        if (!IsAlive()) // E�er oyuncu �l�rse dokunma i�lemleri yap�lmaz
        {
            return; // Hi�bir hareket i�lemi yap�lmaz
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
                // E�er bu d��man daha �nce ate� edilmediyse
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
        currentHealth -= damage; // Can� eksilt
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth; // Health bar'� g�ncelle
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0; // E�er can 0'dan b�y�kse hayatta
    }

    private void HandleDeath()
    {
        _isMoving = false; // Hareketi engelle
        anim.Play("Die"); // �l�m animasyonu oynat

        // M�zik kontrol�
        if (music != null && music.isPlaying)
        {
            music.Stop(); // Arka plan m�zi�ini kapat
        }
        if (lose != null && !hasLosePlayed) // Lose bir kere �als�n
        {
            lose.Play();
            hasLosePlayed = true; // Tekrar �almas�n� engelle
        }

        // �l�mde butonun 2 saniye sonra aktif olmas�n� sa�lamak i�in coroutine ba�lat
        StartCoroutine(ActivateTryAgainButtonAfterDelay());

        // �ld���nde touch i�lemelerini tamamen durdurmak i�in bu iki de�i�keni s�f�rlay�n
        _dragStarted = false;
        _touchDown = Vector3.zero;
        _touchUp = Vector3.zero;

        // Ayr�ca hareket etmeyi engelleyen ba�ka bir kontrol de eklenebilir
        enabled = false; // Bu script'i devre d��� b�rak, b�ylece Update ve di�er fonksiyonlar �al��maz
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
        // D��man alg�lama menzili i�in gizmos �izin
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}