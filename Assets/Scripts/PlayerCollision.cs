using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Vector3 playerStartPosition; 
    private GameObject player; 

    void Start()
    {
        // Oyuncuyu bul ve ba�lang�� pozisyonunu kaydet
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerStartPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("Player objesi bulunamad�! L�tfen 'Player' tag'ine sahip bir obje ekleyin.");
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
            // Oyuncuyu ba�lang�� pozisyonuna geri d�nd�r
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