using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{ 
    [Space, SerializeField] private CanvasGroup optionsMenu;
    [SerializeField] private CanvasGroup mainMenu;

    [Space, SerializeField] private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
        UpdateVolume();
        
        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.gameObject.SetActive(true);

        optionsMenu.alpha = 0;
        optionsMenu.interactable = false;
        optionsMenu.gameObject.SetActive(false);
    }

    public void OpenOptions()
    {
        mainMenu.interactable = false;
        mainMenu.DOFade(0, 0.2f).OnComplete(() =>
        {
            mainMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
            optionsMenu.DOFade(1, 0.2f).OnComplete(() =>
            {
                optionsMenu.interactable = true;
            });
        });
    }

    public void CloseOptions()
    {
        optionsMenu.interactable = false;
        optionsMenu.DOFade(0, 0.2f).OnComplete(() =>
        {
            optionsMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            mainMenu.DOFade(1, 0.2f).OnComplete(() =>
            {
                mainMenu.interactable = true;
            });
        });
    }

    public void UpdateVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void OnVolumeSliderChanged()
    {
        UpdateVolume();
    }

}
