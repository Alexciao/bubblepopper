using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup transition;

    private void Start()
    {
        transition.gameObject.SetActive(false);
        DontDestroyOnLoad(this);
    }

    public void SwitchScene(int buildIndex)
    {
        transition.gameObject.SetActive(true);
        transition.DOFade(1, 0.25f).OnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
            transition.DOFade(0, 0.25f).OnComplete(() =>
            {
                transition.gameObject.SetActive(false);
            });
        });
    }

    public void QuitGame()
    {
        transition.gameObject.SetActive(true);
        transition.DOFade(1, 0.5f).OnComplete(() =>
        {
            Application.Quit();
        });
    }
}
