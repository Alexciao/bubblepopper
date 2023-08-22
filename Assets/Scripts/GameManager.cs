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
    [Space, SerializeField] private GameObject popsTextGroup;
    [SerializeField] private TextMeshProUGUI popsText;
    [Space, SerializeField] private CanvasGroup distanceTextGroup;
    [SerializeField] private TextMeshProUGUI distanceText; 
    
    [Header("Death")]
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource outOfPopsSound;
    [Space,SerializeField] private CanvasGroup deathScreen;
    [Space, SerializeField] private TextMeshProUGUI deathReason;
    [Space, SerializeField] private string deathText;
    [SerializeField] private string noMorePopsText;
    
    [Header("Powerup")]
    [SerializeField] private AudioSource powerupSound;
    
    [Header("Giver")]
    [SerializeField] private AudioSource giverSound;
    
    [Header("Win")]
    [SerializeField] private AudioSource winSound;
    [SerializeField] private CanvasGroup winScreen;
    [SerializeField] private TextMeshProUGUI secondsText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    
    [Header("Settings")]
    [SerializeField] private int maxPops = 250;
    

    private float timer = 0.00f;
    private float bestTime = 100.00f;

    private int pops;
    
    private Vector2 popsTextOriginalPosition;
    private Vector2 distanceTextOriginalPosition;
    
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
        
        popsTextOriginalPosition = popsTextGroup.transform.position;
        distanceTextOriginalPosition = distanceText.transform.parent.position;
        
        distanceTextGroup.gameObject.SetActive(false);
        
        bestTime = PlayerPrefs.GetFloat("BestTime", 100.00f);
        
        deathScreen.alpha = 0;
        deathScreen.gameObject.SetActive(false);
        
        SetCursor(false);
        
        bubbleSpawner.SpawnBubbles();
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        try
        {
            distanceText.text = FormatDistance(Vector3.Distance(player.transform.position, bubbleSpawner.goalPosition));
        }
        catch (MissingReferenceException) {}
        
    }

    public void Die()
    {
        deathScreen.DOFade(0, 0);
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
        deathScreen.DOFade(0, 0);
        deathReason.text = noMorePopsText;
        
        player.GetComponent<PlayerMovement>().enabled = false;

        float animationTime = outOfPopsSound.clip.length;
        CameraShaker.Instance.Shake(1.0f, animationTime);
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
        winScreen.DOFade(0, 0);
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

    public void Powerup()
    {
        distanceTextGroup.DOFade(0, 0);
        distanceTextGroup.gameObject.SetActive(true);
        
        // Fade in
        distanceTextGroup.DOFade(1, 0.2f);
     
        powerupSound.Play();
        
        // Fade out after 1 second
        distanceTextGroup.DOFade(0, 0.2f).SetDelay(1.0f).OnComplete(() =>
        {
            distanceTextGroup.gameObject.SetActive(false);
        });
    }
    
    private double FormatTime(float time)
    {
        return Math.Round(time, 2);
    }

    private string FormatDistance(float distance)
    {
        return Math.Round(distance, 2).ToString() + "m";
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

    public void Giver()
    {
        giverSound.Play();
        
        // Tween the pops text to the powerup text's position
        popsTextGroup.transform.DOMove(distanceTextOriginalPosition, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            IncreasePops(30);
            
            // Tween the pops text back to its original position
            popsTextGroup.transform.DOMove(popsTextOriginalPosition, 0.2f).SetEase(Ease.InOutCubic).SetDelay(0.75f);
        });
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
        IncreasePops(-1);
        
        if (pops <= 0)
        {
            DieTooManyPops();
        }
    }

    public void IncreasePops(int num)
    {
        pops += num;
        popsText.text = pops.ToString();
    }
}
