using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource sound;
    [SerializeField] AudioClip ItemHitSound;
    

    void Start()
    {
        sound = GetComponent<AudioSource>();
        if (sound == null)
        {
            Debug.LogError("AudioSource yok!");
        }
    }

    IEnumerator ChangeTag()
    {
        tag = "Sound";

        MakeSound();

        yield return new WaitForSeconds(3f);

        tag = "item";
    }
    void MakeSound()
    {
        if (sound != null)
        {
            sound.PlayOneShot(ItemHitSound);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("item"))
        {
            StartCoroutine(ChangeTag());            
        }
    }
}