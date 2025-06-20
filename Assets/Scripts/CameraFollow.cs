using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 offset; // Kameran�n oyuncunun arkas�nda kalmas�n� sa�layacak pozisyon offset
    [SerializeField] private Vector3 rotationOffset; // Kameran�n rotasyonunu do�rudan ayarlamak i�in offset (derece cinsinden)
    [SerializeField] private float chaseSpeed = 5f; // Kameran�n takip etme h�z�
    private Quaternion initialRotation; // Kameran�n ba�lang�� rotasyonu (sabit referans)

    [Header("Wall Transparency Settings")]
    [SerializeField] private float raycastDistance = 10f; // Raycast mesafesi
    [SerializeField] private LayerMask obstacleLayer; // Engellerin layer'�
    [SerializeField] private Material transparentMaterial; // �effaf malzeme

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>(); // Orijinal malzemeleri saklamak i�in

    void Start()
    {
        if (!target)
        {
            target = GameObject.FindObjectOfType<player>().transform; // Oyuncu hedefi bulunur
        }

        // Kameran�n ba�lang�� rotasyonunu sakla
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Kameran�n pozisyonunu oyuncunun pozisyonu + offset ile ayarla
        Vector3 targetPosition = target.position + offset;

        // Kamera pozisyonunu hedef pozisyona yumu�ak bir �ekilde ta��
        transform.position = Vector3.Lerp(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

        // Kameran�n rotasyonunu rotationOffset ile do�rudan ayarla
        transform.rotation = Quaternion.Euler(rotationOffset);

        // Kameran�n raycast i�lemi
        CheckForObstacles();
    }

    void CheckForObstacles()
    {
        RaycastHit hit;
        Vector3 direction = target.position - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, obstacleLayer))
        {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();

            if (wallRenderer != null && !originalMaterials.ContainsKey(wallRenderer))
            {
                originalMaterials[wallRenderer] = wallRenderer.material;
                SetWallTransparency(wallRenderer, true);
            }
        }
        else
        {
            foreach (var wallRenderer in originalMaterials.Keys)
            {
                SetWallTransparency(wallRenderer, false);
            }
            originalMaterials.Clear();
        }
    }

    void SetWallTransparency(Renderer wallRenderer, bool isTransparent)
    {
        if (isTransparent)
        {
            wallRenderer.material = transparentMaterial;
        }
        else
        {
            if (originalMaterials.ContainsKey(wallRenderer))
            {
                wallRenderer.material = originalMaterials[wallRenderer];
            }
        }
    }
}