using DG.Tweening;
using UnityEngine;

public class GoalBubble : MonoBehaviour
{
    [SerializeField, Range(0f, 0.1f)] private float audioPitchVariation = 0.05f;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        audioSource.pitch += Random.Range(audioPitchVariation * -1, audioPitchVariation);
        
        transform.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
        
        audioSource.Play();
        
        GameManager.Instance.Win();
        
    }
}
