using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Vector3 playerStartPosition; 
    private GameObject player; 

    void Start()
    {
        // Oyuncuyu bul ve baþlangýç pozisyonunu kaydet
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerStartPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("Player objesi bulunamadý! Lütfen 'Player' tag'ine sahip bir obje ekleyin.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        if (player != null)
        {
            // Oyuncuyu baþlangýç pozisyonuna geri döndür
            player.transform.position = playerStartPosition;
           
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; 
            }
        }
    }
}