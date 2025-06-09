using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //the list of characters that the characterID refers to
    public List<GameObject> characters = new List<GameObject>();

    private List<InputDevice> playerControllers = new List<InputDevice>();
    private List<int> playerCharacterSelections = new List<int>();
    private int playerNo = 0;
    private List<GameObject> players = new List<GameObject>();
    private GameObject[] spawnPoints;
    private GameObject[] playerInterfaces;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlayerNo(int playerNo)
    {
        this.playerNo = playerNo;
    }

    public int GetPlayerNo()
    {
        return playerNo;
    }

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public void AddController(InputDevice device)
    {
        playerControllers.Add(device);
    }

    public void SetCharacter(int characterID)
    {
        playerCharacterSelections.Add(characterID);
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene(1);
        players.Clear();
        StartCoroutine(SpawnAfterSceneLoaded());
        StartCoroutine(ActivateUIAfterSceneLoaded());
    }

    private IEnumerator ActivateUIAfterSceneLoaded()
    {
        yield return null;

        ActivateUI();
    }

    private void ActivateUI()
    {
        if (playerNo != 0)
        {
            playerInterfaces = GameObject.FindGameObjectsWithTag("Player UI");

            //deactivate unnecessary UI
            if (playerInterfaces != null)
            {
                for (int i = 0; i < 4/*max player number*/; i++)
                {
                    if (i > playerNo - 1)
                    {
                        playerInterfaces[i].SetActive(false);
                    }
                }
            }
        }
    }

    private IEnumerator SpawnAfterSceneLoaded()
    {
        yield return null;

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (playerNo != 0)
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            //spawn players at spawn points at the start of the game
            for (int i = 0; i < playerNo; i++)
            {
                GameObject newPlayer = Instantiate(characters[playerCharacterSelections[i]], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                CharacterBase baseScript = newPlayer.GetComponent<CharacterBase>();
                baseScript.SetController(playerControllers[i]);
                players.Add(newPlayer);
            }
        }

        //Once the players have been loaded into the scene, add players to the bounds of the camera
        GameObject[] Cam = GameObject.FindGameObjectsWithTag("MainCamera");
        CameraMovement camMoveScript = Cam[0].GetComponent<CameraMovement>();
        camMoveScript.FindPlayers();
    }
}
