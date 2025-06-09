using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // SceneManager için gerekli

public class RescueMission : MonoBehaviour
{
    // Rehin nesnesi
    public GameObject hostage;

    // UI butonu (Next Level)
    public Image nextLevelButton; // Image yerine Button kullanacaðýz çünkü týklanabilir olmalý

    // Rehin kurtarma kontrolü
    private bool isHostageRescued = false;
    public AudioSource music;
    public AudioSource lose;

    void Start()
    {
        // Baþlangýçta Next Level butonunu gizle
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    // Player'ýn collider'ý
    private void OnTriggerEnter(Collider other)
    {
        // Eðer Player rehineye çarptýysa
        if (other.CompareTag("Player") && !isHostageRescued)
        {
            // Rehini kurtarma iþlemi
            isHostageRescued = true;
            RescueHostage();
            music.Stop();
            lose.Stop();

            // Next Level butonunu aktif et
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(true);
            }
        }
    }

    // Rehin kurtarma iþlemi
    private void RescueHostage()
    {
       
    }

    // Next Level butonuna basýldýðýnda çaðrýlacak fonksiyon
    public void GoToNextLevel()
    {
        // Þu anki sahne indeksini al
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Eðer bir sonraki sahne varsa geçiþ yap
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Seviyeyi kaydet
            PlayerPrefs.SetInt("CurrentLevel", nextSceneIndex);
            PlayerPrefs.Save();

            // Bir sonraki levele geç
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Eðer son seviyedeyse (örneðin, oyunu bitirme ekranýna geçmek için)
            Debug.Log("All levels completed!");
            // Opsiyonel: Ana menüye dönmek için
            // SceneManager.LoadScene(0);
        }
    }
}