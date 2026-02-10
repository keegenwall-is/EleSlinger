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

    // Start is called before the first frame update
    void Start()
    {
        
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

    public override void AttackSound(bool successfulHit, HitBehaviour hitScript)
    {
        if (successfulHit)
        {
            audioPlayer.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)], volume);
        }
        else
        {
            audioPlayer.PlayOneShot(failedHitSound, volume);
        }
    }
}
