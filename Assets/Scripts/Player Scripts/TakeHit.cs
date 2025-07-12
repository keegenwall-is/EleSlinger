using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeHit : MonoBehaviour
{

    private Rigidbody rb;
    private float projPower;
    private Vector3 direction;
    private PlayerAttack PAscript;
    private CharacterBase baseScript;
    private GameObject attacker;
    private PlayerStunned stunnedScript;

    public float flySpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PAscript = GetComponent<PlayerAttack>();
        baseScript = GetComponent<CharacterBase>();
        stunnedScript = GetComponent<PlayerStunned>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitReaction()
    {
        if (gameObject.tag == "Player" || gameObject.tag == "Dummy")
        {
            StartCoroutine(FlyAway());
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void SetProjPower(float projPower)
    {
        this.projPower = projPower;
    }

    public void SetAttacker(GameObject attacker)
    {
        this.attacker = attacker;
    }

    public GameObject GetAttacker()
    {
        return attacker;
    }

    IEnumerator FlyAway()
    {
        if (baseScript.GetState() == CharacterBase.playerState.Attacking)
        {
            PAscript.CancelAttack();
        }

        transform.forward = direction;
        baseScript.SetState(CharacterBase.playerState.TakingHit);
        rb.velocity =  direction.normalized * projPower * flySpeed;
        yield return new WaitForSeconds(0.5f);
        if (baseScript.GetState() != CharacterBase.playerState.Dead)
        {
            baseScript.SetState(CharacterBase.playerState.Idle);
        }
        rb.velocity = Vector3.zero;
        attacker = null;
    }
}
