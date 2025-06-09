using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in

public class TryAgain : MonoBehaviour
{
    // Bu fonksiyon sahneyi yeniden ba�latacak
    public void TryAgainButton()
    {
        // Mevcut sahneyi yeniden y�kle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
