using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //the list of characters that the characterID refers to
    public List<GameObject> characters = new List<GameObject>();
    public GameObject scoreBoard;
    public int maxRounds;
    public GameObject continueUI;

    private List<InputDevice> playerControllers = new List<InputDevice>();
    private List<int> playerCharacterSelections = new List<int>();
    private int playerNo = 0;
    private List<GameObject> players = new List<GameObject>();
    private GameObject[] spawnPoints;
    private GameObject[] playerInterfaces;
    private int[] roundWins = { 0, 0, 0, 0 };
    private bool gameOver = false;
    private bool canContinue = false;
    private bool stage1 = true;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (canContinue)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (playerControllers[i] is Keyboard keyboard)
                {
                    if (keyboard.enterKey.wasPressedThisFrame)
                    {
                        if (gameOver)
                        {
                            //Do something
                        }
                        else
                        {
                            LoadRandomMinigame();
                        }
                    }
                }
                else if (playerControllers[i] is Gamepad gamepad)
                {
                    if (gamepad.buttonEast.wasPressedThisFrame)
                    {
                        if (gameOver)
                        {

                        }
                        else
                        {
                            LoadRandomMinigame();
                        }
                    }
                }
            }
        }
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

    public List<InputDevice> GetControllers()
    {
        return playerControllers;
    }

    public void AddController(InputDevice device)
    {
        playerControllers.Add(device);
    }

    public void SetCharacter(int characterID)
    {
        playerCharacterSelections.Add(characterID);
    }

    public void LoadRandomMinigame()
    {
        canContinue = false;
        /*if (stage1)
        {
            SceneManager.LoadScene(2);
            stage1 = false;
        }
        else
        {
            SceneManager.LoadScene(3);
            stage1 = true;
        }*/

        SceneManager.LoadScene(1);

        //Clear players as new versions of the players will spawn each minigame
        players.Clear();
    }

    private void DeactivateUnusedUI(GameObject[] UIElements)
    {
        if (UIElements != null)
        {
            for (int i = 0; i < 4/*max player number*/; i++)
            {
                if (i > playerNo - 1)
                {
                    UIElements[i].SetActive(false);
                }
            }
        }
    }

    public void SpawnPlayers()
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

    public void IncreaseRoundWins(GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                roundWins[i]++;
            }
        }

        GameObject thisScoreBoard = Instantiate(scoreBoard, Vector3.zero, Quaternion.identity);

        GameObject[] scoreBackgrounds = new GameObject[thisScoreBoard.transform.childCount];
        for (int i = 0; i < thisScoreBoard.transform.childCount; i++)
        {
            scoreBackgrounds[i] = thisScoreBoard.transform.GetChild(i).gameObject;
        }

        DeactivateUnusedUI(scoreBackgrounds);

        GameObject[,] scorePoints = new GameObject[scoreBackgrounds.Length, maxRounds + 1];
        for (int i = 0; i < scoreBackgrounds.Length; i++)
        {
            for (int j = 0; j < maxRounds + 1; j++)
            {
                //the first entry of each column will be the text score version, due to children order in score board prefab
                scorePoints[i, j] = scoreBackgrounds[i].transform.GetChild(j).gameObject;
                scorePoints[i, j].SetActive(false);
            }
        }

        StartCoroutine(DisplayWins(scorePoints));

        //Stop players from performing actions once the round is done
        for (int i = 0; i < players.Count; i++)
        {
            PlayerMove thisMoveScript = players[i].GetComponent<PlayerMove>();
            PlayerAttack thisAttackScript = players[i].GetComponent<PlayerAttack>();
            CharacterBase thisBaseScript = players[i].GetComponent<CharacterBase>();
            PlayerMelee thisMeleeScript = players[i].GetComponent<PlayerMelee>();
            Destroy(thisMoveScript);
            Destroy(thisAttackScript);
            Destroy(thisBaseScript);
            Destroy(thisMeleeScript);
        }
    }

    private IEnumerator DisplayWins(GameObject[,] scorePoints)
    {
        int maxRoundWins = 0;
        for (int i = 0; i < roundWins.Length; i++)
        {
            if (roundWins[i] > maxRoundWins)
            {
                maxRoundWins = roundWins[i];
            }
        }

        if (maxRoundWins == maxRounds)
        {
            gameOver = true;
        }

        for (int i = 0; i < maxRoundWins + 1; i++)
        {
            for (int j = 0; j < scorePoints.GetLength(0); j++)
            {
                if (roundWins[j] >= i)
                {
                    scorePoints[j, i].SetActive(true);
                    Text thisScoreText = scorePoints[j, 0].GetComponent<Text>();
                    thisScoreText.text = i.ToString();
                }
            }
            yield return new WaitForSeconds(1.0f);
        }

        //display continue text
        Instantiate(continueUI, Vector3.zero, Quaternion.identity);
        canContinue = true;
    }
}
