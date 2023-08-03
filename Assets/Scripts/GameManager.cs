using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BubbleSpawner bubbleSpawner;
    [Space, SerializeField] private GameObject player;
    [Space,SerializeField] private TextMeshProUGUI popsText;
    
    [Header("Death")]
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource outOfPopsSound;
    [Space,SerializeField] private CanvasGroup deathScreen;
    [Space, SerializeField] private TextMeshProUGUI deathReason;
    [Space, SerializeField] private string deathText;
    [SerializeField] private string noMorePopsText;
    
    [Header("Win")]
    [SerializeField] private AudioSource winSound;
    [SerializeField] private CanvasGroup winScreen;
    [SerializeField] private TextMeshProUGUI secondsText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    
    [Header("Settings")]
    [SerializeField] private int maxPops = 250;
    

    private float timer = 0.00f;
    private float bestTime = 0.00f;

    private int pops;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is more than one GameManager in the scene");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        pops = maxPops;
        popsText.text = pops.ToString();
        
        bestTime = PlayerPrefs.GetFloat("BestTime", 0.00f);
        
        deathScreen.alpha = 0;
        deathScreen.gameObject.SetActive(false);
        
        SetCursor(false);
        
        bubbleSpawner.SpawnBubbles();
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void Die()
    {
        deathReason.text = deathText;
        
        player.GetComponent<PlayerMovement>().enabled = false;

        float animationTime = deathSound.clip.length;
        CameraShaker.Instance.Shake(2.2f, animationTime);
        deathSound.Play();

        player.transform.DOScale(0, animationTime).OnComplete(() =>
        {
            Destroy(player);
        });
        
        SetCursor(true);
        
        deathScreen.gameObject.SetActive(true);
        deathScreen.DOFade(1, animationTime);
    }

    public void DieTooManyPops()
    {
        deathReason.text = noMorePopsText;
        
        player.GetComponent<PlayerMovement>().enabled = false;

        float animationTime = outOfPopsSound.clip.length;
        CameraShaker.Instance.Shake(2.2f, animationTime);
        outOfPopsSound.Play();

        player.transform.DOScale(0, animationTime).OnComplete(() =>
        {
            Destroy(player);
        });
        
        SetCursor(true);
        
        deathScreen.gameObject.SetActive(true);
        deathScreen.DOFade(1, animationTime);
    }
    
    public void Win()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        float animationTime = winSound.clip.length;
        winSound.Play();

        player.transform.DOScale(0, animationTime).OnComplete(() =>
        {
            Destroy(player);
        });
        
        // Format to "0.00s" and set seconds text
        secondsText.text = FormatTime(timer).ToString() + "s";
        bestTimeText.text = FormatTime(GetHighScore()).ToString() + "s";
        
        SetCursor(true);

        winScreen.gameObject.SetActive(true);
        winScreen.DOFade(1, animationTime);
    }

    private double FormatTime(float time)
    {
        return Math.Round(timer, 2);
    }

    private float GetHighScore()
    {
        if (timer < bestTime)
        {
            bestTime = timer;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        return bestTime;
    }
    
    public void SetCursor(bool status)
    {
        if (status)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void OnBubblePop()
    {
        pops--;
        popsText.text = pops.ToString();
        
        if (pops <= 0)
        {
            DieTooManyPops();
        }
    }
}
