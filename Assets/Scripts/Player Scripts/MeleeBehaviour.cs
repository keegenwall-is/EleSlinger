using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : AttackBase
{

    public float meleePower;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void SetDeleteTime()
    {
        deleteTime = 0.3f;
    }

    protected override Vector3 GetDirection(GameObject enemy)
    {
        Vector3 dir = (enemy.transform.position - thrower.transform.position).normalized;
        return dir;
    }

    protected override float GetPower()
    {
        return meleePower;
    }
}
