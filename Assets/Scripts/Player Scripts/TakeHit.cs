using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeHit : MonoBehaviour
{

    private Rigidbody rb;
    private float projPower;
    private Vector3 direction;

    public float flySpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        transform.forward = direction;
        CharacterBase baseScript = GetComponent<CharacterBase>();
        baseScript.SetState(CharacterBase.playerState.TakingHit);
        rb.AddForce(direction * projPower * flySpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(projPower / projPower / 2);
        baseScript.SetState(CharacterBase.playerState.Idle);
        rb.velocity = new Vector3(0, 0, 0);
    }
}
