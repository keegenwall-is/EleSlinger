using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBehaviour : MonoBehaviour
{

    public float destroyAfterSeconds;
    public AudioSource audioPlayer;
    public AudioClip[] hitSounds;
    public AudioClip failedHitSound;

    private bool successfulHit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Delete());
        audioPlayer = GetComponent<AudioSource>();

        float volume = Mathf.Clamp01(transform.localScale.magnitude / 3f);

        //Audio volume is determined by size of hit
        if (successfulHit)
        {
            audioPlayer.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)], volume);
        }
        else
        {
            audioPlayer.PlayOneShot(failedHitSound, volume);
        }
    }

    public IEnumerator Delete()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }

    public void SetSize(Vector3 size)
    {
        transform.localScale = size / 2;
    }

    public void SetSuccessfulHit(bool success)
    {
        successfulHit = success;
    }
}
