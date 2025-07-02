using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{

    public GameObject thrower;
    public float deleteTime;
    public GameObject hit;

    private GameObject thisHit;

    // Start is called before the first frame update
    void Awake()
    {
        SetDeleteTime();
        StartCoroutine(Delete());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other != thrower && !other.isTrigger)
        {
            thisHit = Instantiate(hit, transform.position, transform.rotation);
            HitBehaviour hitScript = thisHit.GetComponent<HitBehaviour>();
            hitScript.SetSize(transform.localScale);

            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Dummy")
            {
                TakeHit takeHitScript = other.GetComponent<TakeHit>();
                takeHitScript.SetDirection(GetDirection(other.gameObject));
                takeHitScript.SetProjPower(GetPower());
                takeHitScript.SetAttacker(thrower);
                takeHitScript.HitReaction();
            }

            DeleteEarly();
        }
    }

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    protected virtual void SetDeleteTime()
    {

    }

    protected virtual Vector3 GetDirection(GameObject enemy)
    {
        return Vector3.forward;
    }

    protected virtual float GetPower()
    {
        return 1f;
    }

    protected virtual void DeleteEarly()
    {

    }
}
