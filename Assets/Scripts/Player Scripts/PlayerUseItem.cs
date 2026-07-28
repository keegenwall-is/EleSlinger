using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseItem : MonoBehaviour
{
    public Transform firingTrans;
    public Transform stowedTrans;

    private CharacterBase baseScript;
    private PlayerMove moveScript;
    private GameObject thisItem;
    private GameObject itemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = GetComponent<CharacterBase>();
        moveScript = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.GetHasActiveItem())
        {
            //Checks the kind of controller the player is using
            if (baseScript.thisController is Keyboard keyboard)
            {
                //Starts the attack if the player is idle or running and presses the attack button
                if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
                {
                    if (keyboard.iKey.wasPressedThisFrame)
                    {
                        UseItem();
                    }
                }
                else if (baseScript.GetState() == CharacterBase.playerState.UsingItem)
                {
                    if (keyboard.iKey.wasReleasedThisFrame)
                    {
                        StopUsingItem();
                    }
                }
            }
            else if (baseScript.thisController is Gamepad controller)
            {
                if (baseScript.GetState() == CharacterBase.playerState.Idle || baseScript.GetState() == CharacterBase.playerState.Running)
                {
                    if (controller.rightTrigger.wasPressedThisFrame || controller.rightShoulder.wasPressedThisFrame)
                    {
                        UseItem();
                    }
                }
                else if (baseScript.GetState() == CharacterBase.playerState.UsingItem)
                {
                    if (controller.rightTrigger.wasReleasedThisFrame || controller.rightShoulder.wasReleasedThisFrame)
                    {
                        StopUsingItem();
                    }
                }
            }
        }
    }

    public void SetItem(GameObject itemPrefab)
    {
        this.itemPrefab = itemPrefab;
        thisItem = Instantiate(itemPrefab, stowedTrans);
    }
    
    private void UseItem()
    {
        baseScript.SetState(CharacterBase.playerState.UsingItem);
        moveScript.IncreaseSpeed(0.5f);
        Destroy(thisItem);
        thisItem = Instantiate(itemPrefab, firingTrans);
    }

    public void StopUsingItem()
    {
        baseScript.SetState(CharacterBase.playerState.Idle);
        moveScript.DecreaseSpeed();
        Destroy(thisItem);
        thisItem = Instantiate(itemPrefab, stowedTrans);
    }
}
