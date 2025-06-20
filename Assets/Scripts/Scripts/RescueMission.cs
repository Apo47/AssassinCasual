using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // SceneManager i�in gerekli

public class RescueMission : MonoBehaviour
{
    // Rehin nesnesi
    public GameObject hostage;

    // UI butonu (Next Level)
    public Image nextLevelButton; // Image yerine Button kullanaca��z ��nk� t�klanabilir olmal�

    // Rehin kurtarma kontrol�
    private bool isHostageRescued = false;
    public AudioSource music;
    public AudioSource lose;

    void Start()
    {
        // Ba�lang��ta Next Level butonunu gizle
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    // Player'�n collider'�
    private void OnTriggerEnter(Collider other)
    {
        // E�er Player rehineye �arpt�ysa
        if (other.CompareTag("Player") && !isHostageRescued)
        {
            // Rehini kurtarma i�lemi
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

    // Rehin kurtarma i�lemi
    private void RescueHostage()
    {
       
    }

    // Next Level butonuna bas�ld���nda �a�r�lacak fonksiyon
    public void GoToNextLevel()
    {
        // �u anki sahne indeksini al
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // E�er bir sonraki sahne varsa ge�i� yap
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Seviyeyi kaydet
            PlayerPrefs.SetInt("CurrentLevel", nextSceneIndex);
            PlayerPrefs.Save();

            // Bir sonraki levele ge�
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // E�er son seviyedeyse (�rne�in, oyunu bitirme ekran�na ge�mek i�in)
            Debug.Log("All levels completed!");
            // Opsiyonel: Ana men�ye d�nmek i�in
            // SceneManager.LoadScene(0);
        }
    }
}