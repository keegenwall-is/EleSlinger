using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManagement : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject characterSelectMenu;
    public GameObject mainCamera;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;
    public int playerNo = 0;
    public List<GameObject> joinIcons = new List<GameObject>();
    public List<GameObject> mainMenuButtons = new List<GameObject>();
    //Each player has an row of characters that they can each scroll through separately
    public List<GameObject> characterLists = new List<GameObject>();
    public float cameraSpeed;
    public float characterDistance;
    public float animFadeDur;
    public GameObject readyImg;
    public GameController GCscript;

    private List<InputDevice> playerControllers = new List<InputDevice>();
    private bool characterSelect = false;
    private int mainMenuBtnSelect = 0;
    //integer representations of each players position within the list of characters. 0 represents that player hasn't joined.
    private int[] playerCharacterSelections = {0, 0, 0, 0};
    //total number of available characters to choose from
    private int noOfCharacters = 0;
    private bool[] canChangeSelection = {true, true, true, true};
    private bool[] isLockedIn = {false, false, false, false};
    private bool allLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        Image startingBtn = mainMenuButtons[0].gameObject.GetComponent<Image>();
        startingBtn.color = new Color(1f, 1f, 0f, 1f);
        noOfCharacters = characterLists[0].gameObject.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterSelect)
        {
            OperateMainMenu();
        }
        else
        {
            if (playerNo > 0)
            {
                OperateCharacterSelect();
            }
            if (playerNo < 4)
            {
                CheckNewPlayers();
            }
        }
    }

    //onClick function
    public void OpenCharacterSelect()
    {
        characterSelect = true;
        mainMenu.SetActive(false);
        mainCamera.SetActive(false);
        characterSelectMenu.SetActive(true);
        camera1.SetActive(true);
        camera2.SetActive(true);
        camera3.SetActive(true);
        camera4.SetActive(true);
    }

    //onClick function
    public void BackToMainMenu()
    {
        characterSelect = false;
        mainMenu.SetActive(true);
        mainCamera.SetActive(true);
        characterSelectMenu.SetActive(false);
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);
    }

    private void OperateMainMenu()
    {
        //Allows any controller select and click buttons on the title page
        foreach (var device in InputSystem.devices)
        {
            if (device is Keyboard keyboard)
            {
                if (keyboard.enterKey.wasPressedThisFrame)
                {
                    Button thisButton = mainMenuButtons[mainMenuBtnSelect].GetComponent<Button>();
                    thisButton.onClick.Invoke();
                }
            }
            else if (device is Gamepad gamepad)
            {
                if (gamepad.leftStick.down.wasPressedThisFrame && mainMenuBtnSelect < mainMenuButtons.Count - 1)
                {
                    mainMenuBtnSelect++;
                    UIFeedback(-1);
                }
                else if (gamepad.leftStick.up.wasPressedThisFrame && mainMenuBtnSelect > 0)
                {
                    mainMenuBtnSelect--;
                    UIFeedback(1);
                }
                else if (gamepad.buttonEast.wasPressedThisFrame)
                {
                    Button thisButton = mainMenuButtons[mainMenuBtnSelect].GetComponent<Button>();
                    thisButton.onClick.Invoke();
                }
            }
        }
    }

    private void UIFeedback(int lastBtn)
    {
        //remove select colouring from old button
        Image oldBtn = mainMenuButtons[mainMenuBtnSelect + lastBtn].gameObject.GetComponent<Image>();
        oldBtn.color = new Color(1f, 1f, 1f, 1f);
        //add select colouring to new button
        Image newBtn = mainMenuButtons[mainMenuBtnSelect].gameObject.GetComponent<Image>();
        newBtn.color = new Color(1f, 1f, 0f, 1f);
    }

    private void CheckNewPlayers()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Keyboard keyboard)
            {
                if (keyboard.enterKey.wasPressedThisFrame && !playerControllers.Contains(keyboard))
                {
                    AddNewPlayer(keyboard);
                }
            }
            else if (device is Gamepad gamepad)
            {
                if (gamepad.buttonEast.wasPressedThisFrame && !playerControllers.Contains(gamepad))
                {
                    AddNewPlayer(gamepad);
                }
            }
        }
    }

    private void AddNewPlayer(InputDevice device)
    {
        joinIcons[playerNo].gameObject.SetActive(false);
        playerControllers.Add(device);
        //playerCharacterSelections[playerNo] = 1;

        playerNo++;
        CheckLockedIn();
    }

    private void OperateCharacterSelect()
    {
        for (int i = 0; i < playerControllers.Count; i++)
        {
            if (playerControllers[i] is Keyboard keyboard)
            {
                if (keyboard.aKey.wasPressedThisFrame && canChangeSelection[i])
                {
                    if (playerCharacterSelections[i] > 1)
                    {
                        playerCharacterSelections[i]--;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(characterDistance, 0, 0), cameraSpeed, i));
                    }
                } else if (keyboard.dKey.wasPressedThisFrame && canChangeSelection[i])
                {
                    if (playerCharacterSelections[i] < noOfCharacters - 1)
                    {
                        playerCharacterSelections[i]++;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(-characterDistance, 0, 0), cameraSpeed, i));
                    }
                } 
                //stops the player from being able to lock in if the camera is still moving, but allows them to unlock if they are locked in
                else if (keyboard.enterKey.wasPressedThisFrame && (canChangeSelection[i] || isLockedIn[i]))
                {
                    ToggleLockIn(i);
                }
                else if (keyboard.spaceKey.wasPressedThisFrame && allLocked)
                {
                    GameReady();
                }
            }
            else if (playerControllers[i] is Gamepad gamepad)
            {
                if (gamepad.leftStick.left.wasPressedThisFrame && canChangeSelection[i])
                {
                    if (playerCharacterSelections[i] > 1)
                    {
                        playerCharacterSelections[i]--;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(characterDistance, 0, 0), cameraSpeed, i));
                    }
                }
                else if (gamepad.leftStick.right.wasPressedThisFrame && canChangeSelection[i])
                {
                    if (playerCharacterSelections[i] < noOfCharacters - 1)
                    {
                        playerCharacterSelections[i]++;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(-characterDistance, 0, 0), cameraSpeed, i));
                    }
                }
                //stops the player from being able to lock in if the camera is still moving, but allows them to unlock if they are locked in
                else if (gamepad.buttonEast.wasPressedThisFrame && (canChangeSelection[i] || isLockedIn[i]))
                {
                    ToggleLockIn(i);
                }
                else if (gamepad.buttonNorth.wasPressedThisFrame && allLocked)
                {
                    GameReady();
                }
            }
        }
    }

    private void ToggleLockIn(int player)
    {
        if (isLockedIn[player] == true)
        {
            isLockedIn[player] = false;
            canChangeSelection[player] = true;
            //Finds the animator of the previously selected character and sets the animation back to idle
            Transform deselectedCharacterT = characterLists[player].gameObject.transform.GetChild(playerCharacterSelections[player]);
            PlayAnim(deselectedCharacterT, "Idle");
            CheckLockedIn();
        }
        else
        {
            isLockedIn[player] = true;
            canChangeSelection[player] = false;
            //Finds the animator of the currently selected Character and sets the animation to a selected pose
            Transform selectedCharacterT = characterLists[player].gameObject.transform.GetChild(playerCharacterSelections[player]);
            PlayAnim(selectedCharacterT, "Selected");
            CheckLockedIn();
        }
    }

    private void CheckLockedIn()
    {
        bool thisAllLocked = true;

        for (int i = 0; i < playerNo; i++)
        {
            if (!isLockedIn[i])
            {
                thisAllLocked = false;
                break;
            }
        }

        if (thisAllLocked)
        {
            allLocked = true;
            readyImg.SetActive(true);
        } else
        {
            allLocked = false;
            readyImg.SetActive(false);
        }
    }

    IEnumerator CameraMove(Transform selectionTransform, Vector3 targetPos, float speed, int playerNo)
    {
        canChangeSelection[playerNo] = false;
        Vector3 currentPos = selectionTransform.position;
        targetPos = currentPos + targetPos;
        float elapsed = 0f;

        //Moves the selection list to the next character
        while (elapsed < speed)
        {
            selectionTransform.position = Vector3.Lerp(currentPos, targetPos, elapsed / speed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        selectionTransform.position = targetPos;
        canChangeSelection[playerNo] = true;
    }

    private void PlayAnim(Transform selectedCharacterT, string seg)
    {
        GameObject selectedCharacter = selectedCharacterT.gameObject;
        Animator anim = selectedCharacter.GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains(seg))
            {
                anim.CrossFade(clip.name, animFadeDur);
                break;
            }
        }
    }

    private void GameReady()
    {
        GCscript.SetPlayerNo(playerNo);

        foreach (var device in playerControllers)
        {
            GCscript.AddController(device);
        }

        for (int i = 0; i < playerNo; i++)
        {
            GCscript.SetCharacter(playerCharacterSelections[i]);
        }

        GCscript.StartTutorial();
    }
}
