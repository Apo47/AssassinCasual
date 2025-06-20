using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 offset; // Kameranýn oyuncunun arkasýnda kalmasýný saðlayacak pozisyon offset
    [SerializeField] private Vector3 rotationOffset; // Kameranýn rotasyonunu doðrudan ayarlamak için offset (derece cinsinden)
    [SerializeField] private float chaseSpeed = 5f; // Kameranýn takip etme hýzý
    private Quaternion initialRotation; // Kameranýn baþlangýç rotasyonu (sabit referans)

    [Header("Wall Transparency Settings")]
    [SerializeField] private float raycastDistance = 10f; // Raycast mesafesi
    [SerializeField] private LayerMask obstacleLayer; // Engellerin layer'ý
    [SerializeField] private Material transparentMaterial; // Þeffaf malzeme

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>(); // Orijinal malzemeleri saklamak için

    void Start()
    {
        if (!target)
        {
            target = GameObject.FindObjectOfType<player>().transform; // Oyuncu hedefi bulunur
        }

        // Kameranýn baþlangýç rotasyonunu sakla
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Kameranýn pozisyonunu oyuncunun pozisyonu + offset ile ayarla
        Vector3 targetPosition = target.position + offset;

        // Kamera pozisyonunu hedef pozisyona yumuþak bir þekilde taþý
        transform.position = Vector3.Lerp(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

        // Kameranýn rotasyonunu rotationOffset ile doðrudan ayarla
        transform.rotation = Quaternion.Euler(rotationOffset);

        // Kameranýn raycast iþlemi
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