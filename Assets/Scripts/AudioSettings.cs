using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // Müzik ve ses kaynaklarý
    public AudioSource musicSource; // Arka plan müziði için AudioSource
    public AudioSource soundSource; // Ses efektleri için AudioSource
    public AudioSource lose;        // Oyuncu öldüðünde çalacak "lose" müziði için AudioSource

    // UI Butonlarý
    public Button musicOnButton;
    public Button musicOffButton;
    public Button soundOnButton;
    public Button soundOffButton;

    // Baþlangýçta durumlarý kontrol etmek için
    private void Start()
    {
        // PlayerPrefs'ten kaydedilmiþ ayarlarý yükle
        LoadSettings();

        // Butonlarýn baþlangýç durumlarýný güncelle
        UpdateMusicButtons();
        UpdateSoundButtons();

        // Lose müziðini baþlangýçta durdur ve döngüsüz yap
        if (lose != null)
        {
            lose.Stop();
            lose.loop = false; // Lose müziði bir kere çalýp bitsin
        }

        // Arka plan müziðini döngüye al ve baþlat (eðer muted deðilse)
        if (musicSource != null)
        {
            musicSource.loop = true;
            if (!musicSource.mute && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    // Müzik Açma Fonksiyonu (Music_On butonuna atanacak)
    public void ToggleMusicOn()
    {
        musicSource.mute = false; // Arka plan müziðini aç
        lose.mute = false;        // Lose müziðini aç
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();   // Müziði baþlat (eðer çalmýyorsa)
        }
        musicOnButton.gameObject.SetActive(false);  // Music_On butonunu gizle
        musicOffButton.gameObject.SetActive(true);  // Music_Off butonunu göster
        PlayerPrefs.SetInt("MusicMuted", 0);        // Müziðin açýk olduðunu kaydet
        PlayerPrefs.Save();                         // Deðiþiklikleri kaydet
    }

    // Müzik Kapama Fonksiyonu (Music_Off butonuna atanacak)
    public void ToggleMusicOff()
    {
        musicSource.mute = true;   // Arka plan müziðini kapat
        lose.mute = true;          // Lose müziðini kapat
        musicOffButton.gameObject.SetActive(false); // Music_Off butonunu gizle
        musicOnButton.gameObject.SetActive(true);   // Music_On butonunu göster
        PlayerPrefs.SetInt("MusicMuted", 1);        // Müziðin kapalý olduðunu kaydet
        PlayerPrefs.Save();                         // Deðiþiklikleri kaydet
    }

    // Ses Açma Fonksiyonu (Sound_On butonuna atanacak)
    public void ToggleSoundOn()
    {
        soundSource.mute = false;   // Ses efektlerini aç
        soundOnButton.gameObject.SetActive(false);  // Sound_On butonunu gizle
        soundOffButton.gameObject.SetActive(true);  // Sound_Off butonunu göster
        PlayerPrefs.SetInt("SoundMuted", 0);        // Sesin açýk olduðunu kaydet
        PlayerPrefs.Save();                         // Deðiþiklikleri kaydet
    }

    // Ses Kapama Fonksiyonu (Sound_Off butonuna atanacak)
    public void ToggleSoundOff()
    {
        soundSource.mute = true;    // Ses efektlerini kapat
        soundOffButton.gameObject.SetActive(false); // Sound_Off butonunu gizle
        soundOnButton.gameObject.SetActive(true);   // Sound_On butonunu göster
        PlayerPrefs.SetInt("SoundMuted", 1);        // Sesin kapalý olduðunu kaydet
        PlayerPrefs.Save();                         // Deðiþiklikleri kaydet
    }

    // Kaydedilmiþ ayarlarý yükleme
    private void LoadSettings()
    {
        // Müzik ayarýný yükle (varsayýlan: açýk)
        int musicMuted = PlayerPrefs.GetInt("MusicMuted", 0); // 0: açýk, 1: kapalý
        if (musicSource != null)
        {
            musicSource.mute = (musicMuted == 1);
        }
        if (lose != null)
        {
            lose.mute = (musicMuted == 1); // Lose müziði de ayný ayarlarý takip etsin
        }

        // Ses ayarýný yükle (varsayýlan: açýk)
        int soundMuted = PlayerPrefs.GetInt("SoundMuted", 0); // 0: açýk, 1: kapalý
        if (soundSource != null)
        {
            soundSource.mute = (soundMuted == 1);
        }
    }

    // Müzik butonlarýnýn görünürlüðünü güncelle
    private void UpdateMusicButtons()
    {
        if (musicSource != null && musicSource.mute)
        {
            musicOnButton.gameObject.SetActive(true);
            musicOffButton.gameObject.SetActive(false);
        }
        else
        {
            musicOnButton.gameObject.SetActive(false);
            musicOffButton.gameObject.SetActive(true);
        }
    }

    // Ses butonlarýnýn görünürlüðünü güncelle
    private void UpdateSoundButtons()
    {
        if (soundSource != null && soundSource.mute)
        {
            soundOnButton.gameObject.SetActive(true);
            soundOffButton.gameObject.SetActive(false);
        }
        else
        {
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
        }
    }

    // Oyuncu öldüðünde lose müziðini çalmak için fonksiyon (player script’inden çaðrýlacak)
    public void PlayLoseMusic()
    {
        if (lose != null && !lose.mute && !lose.isPlaying)
        {
            lose.Play(); // Lose müziðini çal (eðer muted deðilse)
        }
    }

    // Oyunu yeniden baþlattýðýnda arka plan müziðini sýfýrlamak için fonksiyon
    public void ResetMusic()
    {
        if (lose != null && lose.isPlaying)
        {
            lose.Stop(); // Lose müziðini durdur
        }
        if (musicSource != null && !musicSource.mute && !musicSource.isPlaying)
        {
            musicSource.Play(); // Arka plan müziðini yeniden baþlat (eðer muted deðilse)
        }
    }
}