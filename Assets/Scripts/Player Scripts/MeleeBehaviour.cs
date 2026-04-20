using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : AttackBase
{

    public float meleePower;
    public float meleeTime;
    public AudioSource audioPlayer;
    public AudioClip[] hitSounds;
    public AudioClip failedHitSound;
    public float volume;

    private bool success;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForFixedUpdate();
        if (success)
        {
            audioPlayer.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)], volume);
        }
        else
        {
            audioPlayer.PlayOneShot(failedHitSound, volume);
        }
    }

    protected override void SetSuccessfulHit(bool success, HitBehaviour hitScript)
    {
        this.success = success;
    }

    protected override float SetDeleteTime()
    {
        return meleeTime;
    }

    public override Vector3 GetDirection(GameObject enemy)
    {
        Vector3 dir = (enemy.transform.position - thrower.transform.position).normalized;
        return dir;
    }

    public override float GetPower()
    {
        return meleePower;
    }
}
