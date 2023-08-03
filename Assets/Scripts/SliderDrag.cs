using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
    
public class SliderDrag : MonoBehaviour,IPointerUpHandler
{

    [SerializeField] private AudioSource endDragSound;
    
    public void OnPointerUp(PointerEventData eventData)
    {
        endDragSound.Play();
    }
}