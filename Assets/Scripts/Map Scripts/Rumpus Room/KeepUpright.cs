using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    public bool canvas;

    void LateUpdate()
    {
        if (canvas)
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
