using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;

public class Bubble : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField, Range(0f, 1f)] private float bombChance = 0.05f;

    [SerializeField, Range(0f, 0.1f)] private float audioPitchVariation = 0.05f;

    [Header("References")] 
    [SerializeField] private GameObject bomb;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Values")]
    public bool hasBomb;

    private void Start()
    {
        if (Random.Range(0f, 1f) <= bombChance)
        {
            hasBomb = true;
            bomb.SetActive(true);
        }
        else
        {
            hasBomb = false;
            bomb.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        audioSource.pitch += Random.Range(audioPitchVariation * -1, audioPitchVariation);
        
        transform.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
        
        audioSource.Play();

        if (hasBomb)
        {
            GameManager.Instance.Die();
        }
        
        GameManager.Instance.OnBubblePop();
    }
}