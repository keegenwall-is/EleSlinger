using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : AttackBase
{

    public float meleePower;
    public float meleeTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override float SetDeleteTime()
    {
        return meleeTime;
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
