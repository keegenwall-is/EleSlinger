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

    public float flySpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PAscript = GetComponent<PlayerAttack>();
        baseScript = GetComponent<CharacterBase>();
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

    IEnumerator FlyAway()
    {
        if (baseScript.GetState() == CharacterBase.playerState.Attacking)
        {
            PAscript.CancelAttack();
        }

        transform.forward = direction;
        baseScript.SetState(CharacterBase.playerState.TakingHit);
        rb.AddForce(Vector3.Project(rb.velocity, direction.normalized) + direction.normalized * projPower * flySpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(projPower * 0.5f);
        baseScript.SetState(CharacterBase.playerState.Idle);
        rb.velocity = new Vector3(0, 0, 0);
    }
}
