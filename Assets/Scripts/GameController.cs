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
    private GameObject[] spawnPoints;

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
        StartCoroutine(SpawnAfterSceneLoaded());
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

            for (int i = 0; i < playerNo; i++)
            {
                GameObject newPlayer = Instantiate(characters[playerCharacterSelections[i]], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                CharacterBase baseScript = newPlayer.GetComponent<CharacterBase>();
                baseScript.SetController(playerControllers[i]);
            }
        }
    }
}
