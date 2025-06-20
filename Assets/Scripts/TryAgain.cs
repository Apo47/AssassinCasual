using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için

public class TryAgain : MonoBehaviour
{
    // Bu fonksiyon sahneyi yeniden baþlatacak
    public void TryAgainButton()
    {
        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
