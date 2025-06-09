using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public int playerNo;
    public List<GameObject> effectedObjects = new List<GameObject>();

    private GameController gameController;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
        players = gameController.GetPlayers();
        playerNo = gameController.GetPlayerNo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerObstacleEvent(GameObject actor)
    {
        OnObstacleEvent(actor);
    }

    protected virtual void OnObstacleEvent(GameObject actor)
    {
        //Default is to do nothing (obstacles don't have the same effect in all minigames)
    }
}
