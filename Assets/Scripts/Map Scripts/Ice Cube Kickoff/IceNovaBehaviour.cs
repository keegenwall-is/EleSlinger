using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceNovaBehaviour : MonoBehaviour
{
    public int iceMashes;
    public GameObject popsicleCube;

    private GameObject thrower;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject == thrower)
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

            PlayerAttack attackScript = thrower.GetComponent<PlayerAttack>();
            attackScript.SetSpecialAttack(false);

            List<GameObject> frosts = new List<GameObject>();

            foreach (Transform child in thrower.transform)
            {
                if (child.name.Contains("Pop"))
                {
                    frosts.Add(child.gameObject);
                }
            }

            foreach (GameObject frost in frosts)
            {
                Destroy(frost);
            }
        }
    }
}
