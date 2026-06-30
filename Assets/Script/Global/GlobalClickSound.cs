using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalClickSound : MonoBehaviour,IPointerDownHandler
{
    public AudioClip clickClip;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }
}
