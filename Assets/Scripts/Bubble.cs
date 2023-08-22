using System;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Bubble : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField, Range(0f, 1f)] private float bombChance = 0.05f;
    [SerializeField, Range(0f, 1f)] private float powerupChance = 0.05f;
    [SerializeField, Range(0f, 1f)] private float giverChance = 0.005f;

    [SerializeField, Range(0f, 0.1f)] private float audioPitchVariation = 0.05f;

    [Header("References")] 
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject powerup;
    [SerializeField] private GameObject giver;
    
    [Space, SerializeField] private AudioSource audioSource;
    
    [Header("Values")]
    public bool hasBomb;
    public bool hasPowerup;
    public bool hasGiver;

    private void Start()
    {
        if (Random.Range(0f, 1f) <= bombChance)
        {
            hasBomb = true;
            UpdateBomb();
        }
        else
        {
            hasBomb = false;
            UpdateBomb();
        }
        
        if (Random.Range(0f, 1f) <= powerupChance && !hasBomb)
        {
            hasPowerup = true;
            UpdatePowerup();
        }
        else
        {
            hasPowerup = false;
            UpdatePowerup();
        }
        
        if (Random.Range(0f, 1f) <= giverChance && !hasBomb && !hasPowerup)
        {
            hasGiver = true;
            UpdateGiver();
        }
        else
        {
            hasGiver = false;
            UpdateGiver();
        }
    }

    private void UpdateBomb()
    {
        if (!hasBomb) bomb.SetActive(false);
        else bomb.SetActive(true);
    }

    private void UpdatePowerup()
    {
        if (!hasPowerup) powerup.SetActive(false);
        else powerup.SetActive(true); 
    }
    
    private void UpdateGiver()
    {
        if (!hasGiver) giver.SetActive(false);
        else giver.SetActive(true); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<Collider2D>().enabled = false; // disable collider so it doesn't trigger again
        audioSource.pitch += Random.Range(audioPitchVariation * -1, audioPitchVariation);
        
        transform.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
        
        audioSource.Play();

        if (hasBomb)
        {
            GameManager.Instance.Die();
        }

        if (hasPowerup)
        {
            GameManager.Instance.Powerup();
        }
        
        if (hasGiver)
        {
            GameManager.Instance.Giver();
        }
        
        GameManager.Instance.OnBubblePop();
    }
}
