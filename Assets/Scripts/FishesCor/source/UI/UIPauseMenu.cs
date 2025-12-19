using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public Slider sfx;
    public Slider music;

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Hide();
        else
            Show();
    }

    public void ResetRunBtn()
    {
        Time.timeScale = 1;
        G.main.ChangeScene(0);
    }

    void Show()
    {
        //sfx.value = G.save.volSfx;
        //music.value = G.save.volMusic;
        
        G.IsPaused = true;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        Debug.Log(1);
    }

    void Update()
    {
        //G.save.volSfx = sfx.value;
        //G.save.volMusic = music.value;
        
        //G.audio.SetVolume(AudioType.SFX, G.save.volSfx);
        //G.audio.SetVolume(AudioType.Music, G.save.volMusic);
        //G.audio.SetVolume(AudioType.Ambient, G.save.volMusic);
    }

    void Hide()
    {
        G.IsPaused = false;
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    
}