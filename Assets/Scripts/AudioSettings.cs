using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // M�zik ve ses kaynaklar�
    public AudioSource musicSource; // Arka plan m�zi�i i�in AudioSource
    public AudioSource soundSource; // Ses efektleri i�in AudioSource
    public AudioSource lose;        // Oyuncu �ld���nde �alacak "lose" m�zi�i i�in AudioSource

    // UI Butonlar�
    public Button musicOnButton;
    public Button musicOffButton;
    public Button soundOnButton;
    public Button soundOffButton;

    // Ba�lang��ta durumlar� kontrol etmek i�in
    private void Start()
    {
        // PlayerPrefs'ten kaydedilmi� ayarlar� y�kle
        LoadSettings();

        // Butonlar�n ba�lang�� durumlar�n� g�ncelle
        UpdateMusicButtons();
        UpdateSoundButtons();

        // Lose m�zi�ini ba�lang��ta durdur ve d�ng�s�z yap
        if (lose != null)
        {
            lose.Stop();
            lose.loop = false; // Lose m�zi�i bir kere �al�p bitsin
        }

        // Arka plan m�zi�ini d�ng�ye al ve ba�lat (e�er muted de�ilse)
        if (musicSource != null)
        {
            musicSource.loop = true;
            if (!musicSource.mute && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    // M�zik A�ma Fonksiyonu (Music_On butonuna atanacak)
    public void ToggleMusicOn()
    {
        musicSource.mute = false; // Arka plan m�zi�ini a�
        lose.mute = false;        // Lose m�zi�ini a�
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();   // M�zi�i ba�lat (e�er �alm�yorsa)
        }
        musicOnButton.gameObject.SetActive(false);  // Music_On butonunu gizle
        musicOffButton.gameObject.SetActive(true);  // Music_Off butonunu g�ster
        PlayerPrefs.SetInt("MusicMuted", 0);        // M�zi�in a��k oldu�unu kaydet
        PlayerPrefs.Save();                         // De�i�iklikleri kaydet
    }

    // M�zik Kapama Fonksiyonu (Music_Off butonuna atanacak)
    public void ToggleMusicOff()
    {
        musicSource.mute = true;   // Arka plan m�zi�ini kapat
        lose.mute = true;          // Lose m�zi�ini kapat
        musicOffButton.gameObject.SetActive(false); // Music_Off butonunu gizle
        musicOnButton.gameObject.SetActive(true);   // Music_On butonunu g�ster
        PlayerPrefs.SetInt("MusicMuted", 1);        // M�zi�in kapal� oldu�unu kaydet
        PlayerPrefs.Save();                         // De�i�iklikleri kaydet
    }

    // Ses A�ma Fonksiyonu (Sound_On butonuna atanacak)
    public void ToggleSoundOn()
    {
        soundSource.mute = false;   // Ses efektlerini a�
        soundOnButton.gameObject.SetActive(false);  // Sound_On butonunu gizle
        soundOffButton.gameObject.SetActive(true);  // Sound_Off butonunu g�ster
        PlayerPrefs.SetInt("SoundMuted", 0);        // Sesin a��k oldu�unu kaydet
        PlayerPrefs.Save();                         // De�i�iklikleri kaydet
    }

    // Ses Kapama Fonksiyonu (Sound_Off butonuna atanacak)
    public void ToggleSoundOff()
    {
        soundSource.mute = true;    // Ses efektlerini kapat
        soundOffButton.gameObject.SetActive(false); // Sound_Off butonunu gizle
        soundOnButton.gameObject.SetActive(true);   // Sound_On butonunu g�ster
        PlayerPrefs.SetInt("SoundMuted", 1);        // Sesin kapal� oldu�unu kaydet
        PlayerPrefs.Save();                         // De�i�iklikleri kaydet
    }

    // Kaydedilmi� ayarlar� y�kleme
    private void LoadSettings()
    {
        // M�zik ayar�n� y�kle (varsay�lan: a��k)
        int musicMuted = PlayerPrefs.GetInt("MusicMuted", 0); // 0: a��k, 1: kapal�
        if (musicSource != null)
        {
            musicSource.mute = (musicMuted == 1);
        }
        if (lose != null)
        {
            lose.mute = (musicMuted == 1); // Lose m�zi�i de ayn� ayarlar� takip etsin
        }

        // Ses ayar�n� y�kle (varsay�lan: a��k)
        int soundMuted = PlayerPrefs.GetInt("SoundMuted", 0); // 0: a��k, 1: kapal�
        if (soundSource != null)
        {
            soundSource.mute = (soundMuted == 1);
        }
    }

    // M�zik butonlar�n�n g�r�n�rl���n� g�ncelle
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

    // Ses butonlar�n�n g�r�n�rl���n� g�ncelle
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

    // Oyuncu �ld���nde lose m�zi�ini �almak i�in fonksiyon (player script�inden �a�r�lacak)
    public void PlayLoseMusic()
    {
        if (lose != null && !lose.mute && !lose.isPlaying)
        {
            lose.Play(); // Lose m�zi�ini �al (e�er muted de�ilse)
        }
    }

    // Oyunu yeniden ba�latt���nda arka plan m�zi�ini s�f�rlamak i�in fonksiyon
    public void ResetMusic()
    {
        if (lose != null && lose.isPlaying)
        {
            lose.Stop(); // Lose m�zi�ini durdur
        }
        if (musicSource != null && !musicSource.mute && !musicSource.isPlaying)
        {
            musicSource.Play(); // Arka plan m�zi�ini yeniden ba�lat (e�er muted de�ilse)
        }
    }
}