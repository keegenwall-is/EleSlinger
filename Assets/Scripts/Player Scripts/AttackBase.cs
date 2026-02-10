using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{

    public GameObject thrower;
    public GameObject hit;
    public bool successfulHit;

    private GameObject thisHit;
    private float deleteTime;
    private PlayerAttack playerAttackScript;

    // Start is called before the first frame update
    void Awake()
    {
        deleteTime = SetDeleteTime();
        StartCoroutine(Delete());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
        playerAttackScript = thrower.GetComponent<PlayerAttack>();
    }

    public GameObject GetThrower()
    {
        return thrower;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != thrower && !other.isTrigger)
        {
            thisHit = Instantiate(hit, transform.position, transform.rotation);
            HitBehaviour hitScript = thisHit.GetComponent<HitBehaviour>();
            hitScript.SetSize(transform.localScale);

            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Dummy")
            {
                if (playerAttackScript.GetSpecialAttack())
                {
                    MinigameManager managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<MinigameManager>();
                    managerScript.HandleSpecialAttack(other.gameObject, thrower);
                }
                else
                {
                    TakeHit takeHitScript = other.GetComponent<TakeHit>();
                    takeHitScript.SetDirection(GetDirection(other.gameObject));
                    takeHitScript.SetProjPower(GetPower());
                    takeHitScript.SetAttacker(thrower);
                    takeHitScript.HitReaction();
                }
            }

            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Interactive Obj")
            {
                AttackSound(true, hitScript);
            }
            else
            {
                AttackSound(false, hitScript);
            }

            DeleteEarly();
        }
    }

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(gameObject);
    }

    protected virtual float SetDeleteTime()
    {
        return 0.8f;
    }

    public virtual Vector3 GetDirection(GameObject enemy)
    {
        return Vector3.forward;
    }

    public virtual float GetPower()
    {
        return 1f;
    }

    public virtual void AttackSound(bool successfulHit, HitBehaviour hitScript)
    {
        
    }

    protected virtual void DeleteEarly()
    {

    }
}
