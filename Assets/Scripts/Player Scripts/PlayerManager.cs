using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    private List<InputDevice> playerControllers = new List<InputDevice>();
    private List<int> playerCharacterSelections = new List<int>();
    private int playerNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddController(InputDevice device)
    {
        playerControllers.Add(device);
    }

    public void SetPlayerNo(int playerNo)
    {
        this.playerNo = playerNo;
    }

    public void SetCharacter(int characterID)
    {
        playerCharacterSelections.Add(characterID);
    }
}
