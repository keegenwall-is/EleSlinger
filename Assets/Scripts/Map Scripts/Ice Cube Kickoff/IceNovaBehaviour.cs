using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceNovaBehaviour : MonoBehaviour
{
    public int iceMashes;
    public GameObject popsicleCube;

    private GameObject thrower;
    private GameObject teammate;
    private List<GameObject> alreadyFrozen = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
    }

    public void SetTeamMate(GameObject teammate)
    {
        this.teammate = teammate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject == thrower)
            {
                return;
            }

            //Already frozen is added to fix the bug where you can score with someone while they are
            //already in the goal which will cause them to refreeze many times while the ice nova is active
            foreach (GameObject frozenPlayer in alreadyFrozen)
            {
                if (frozenPlayer == other.gameObject)
                {
                    return;
                }
            }

            PlayerAttack attackScript = thrower.GetComponent<PlayerAttack>();
            attackScript.SetSpecialAttack(false);

            List<GameObject> frosts = new List<GameObject>();

            foreach (Transform child in thrower.transform)
            {
                if (child.name.Contains("Ice"))
                {
                    frosts.Add(child.gameObject);
                }
            }

            foreach (GameObject frost in frosts)
            {
                Destroy(frost);
            }

            if (other.gameObject == teammate)
            {
                return;
            }

            PlayerStunned stunnedScript = other.gameObject.GetComponent<PlayerStunned>();
            stunnedScript.SetMashes(iceMashes);
            stunnedScript.Stunned();

            Vector3 spawnPos = other.gameObject.transform.position;
            spawnPos.y += 2.5f;
            GameObject thisIce = Instantiate(popsicleCube, spawnPos, other.gameObject.transform.rotation);
            other.gameObject.transform.SetParent(thisIce.transform);
            IceCubeBehaviour iceScript = thisIce.GetComponent<IceCubeBehaviour>();
            iceScript.SetWillShrink(false);
            iceScript.SetAttachedPlayer(other.gameObject);

            CapsuleCollider hitPlayerCC = other.gameObject.GetComponent<CapsuleCollider>();
            hitPlayerCC.enabled = false;

            other.gameObject.transform.position = thisIce.transform.position;

            alreadyFrozen.Add(other.gameObject);
        }
    }
}
