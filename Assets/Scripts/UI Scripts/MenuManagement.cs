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
    public List<InputDevice> playerControllers = new List<InputDevice>();
    public List<GameObject> joinIcons = new List<GameObject>();
    public List<GameObject> mainMenuButtons = new List<GameObject>();
    public List<GameObject> characterLists = new List<GameObject>();
    public float cameraSpeed;
    public float characterDistance;
    public float animFadeDur;

    private bool characterSelect = false;
    private int mainMenuBtnSelect = 0;
    private int[] playerCharacterSelections = {0, 0, 0, 0};
    private int noOfCharacters = 0;
    private bool canChangeSelection = true;

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
            if (playerNo < 4)
            {
                CheckNewPlayers();
            }
            if (playerNo > 0)
            {
                OperateCharacterSelect();
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
                    //remove select colouring from button
                    Image oldBtn = mainMenuButtons[mainMenuBtnSelect - 1].gameObject.GetComponent<Image>();
                    oldBtn.color = new Color(1f, 1f, 1f, 1f);
                    //add select colouring to new button
                    Image newBtn = mainMenuButtons[mainMenuBtnSelect].gameObject.GetComponent<Image>();
                    newBtn.color = new Color(1f, 1f, 0f, 1f);
                }
                else if (gamepad.leftStick.up.wasPressedThisFrame && mainMenuBtnSelect > 0)
                {
                    mainMenuBtnSelect--;
                    Image oldBtn = mainMenuButtons[mainMenuBtnSelect + 1].gameObject.GetComponent<Image>();
                    oldBtn.color = new Color(1f, 1f, 1f, 1f);
                    Image newBtn = mainMenuButtons[mainMenuBtnSelect].gameObject.GetComponent<Image>();
                    newBtn.color = new Color(1f, 1f, 0f, 1f);
                }
                else if (gamepad.buttonEast.wasPressedThisFrame)
                {
                    Button thisButton = mainMenuButtons[mainMenuBtnSelect].GetComponent<Button>();
                    thisButton.onClick.Invoke();
                }
            }
        }
    }

    private void CheckNewPlayers()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Keyboard keyboard)
            {
                if (keyboard.enterKey.wasPressedThisFrame && !playerControllers.Contains(keyboard))
                {
                    joinIcons[playerNo].gameObject.SetActive(false);
                    playerControllers.Add(device);
                    playerCharacterSelections[playerNo] = 1;
                    playerNo++;
                }
            }
            else if (device is Gamepad gamepad)
            {
                if (gamepad.buttonEast.wasPressedThisFrame && !playerControllers.Contains(gamepad))
                {
                    joinIcons[playerNo].gameObject.SetActive(false);
                    playerControllers.Add(device);
                    playerCharacterSelections[playerNo] = 1;
                    playerNo++;
                }
            }
        }
    }

    private void OperateCharacterSelect()
    {
        for (int i = 0; i < playerControllers.Count; i++)
        {
            if (playerControllers[i] is Keyboard keyboard)
            {
                if (keyboard.aKey.wasPressedThisFrame && canChangeSelection)
                {
                    if (playerCharacterSelections[i] > 1)
                    {
                        playerCharacterSelections[i]--;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(characterDistance, 0, 0), cameraSpeed, playerCharacterSelections[i], 1));
                    }
                } else if (keyboard.dKey.wasPressedThisFrame && canChangeSelection)
                {
                    if (playerCharacterSelections[i] < noOfCharacters)
                    {
                        playerCharacterSelections[i]++;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(-characterDistance, 0, 0), cameraSpeed, playerCharacterSelections[i], -1));
                    }
                }
            }
            else if (playerControllers[i] is Gamepad gamepad)
            {
                if (gamepad.leftStick.left.wasPressedThisFrame && canChangeSelection)
                {
                    if (playerCharacterSelections[i] > 1)
                    {
                        playerCharacterSelections[i]--;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(characterDistance, 0, 0), cameraSpeed, playerCharacterSelections[i], 1));
                    }
                }
                else if (gamepad.leftStick.right.wasPressedThisFrame && canChangeSelection)
                {
                    if (playerCharacterSelections[i] < noOfCharacters)
                    {
                        playerCharacterSelections[i]++;
                        StartCoroutine(CameraMove(characterLists[i].gameObject.transform, new Vector3(-characterDistance, 0, 0), cameraSpeed, playerCharacterSelections[i], -1));
                    }
                }
            }
        }
    }

    IEnumerator CameraMove(Transform selectionTransform, Vector3 targetPos, float speed, int selectedCharacterIndex, int moveDir)
    {
        canChangeSelection = false;
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

        //Finds the animator of the currently selected Character and sets the animation to a selected pose
        Transform selectedCharacterT = selectionTransform.GetChild(selectedCharacterIndex - 1);
        GameObject selectedCharacter = selectedCharacterT.gameObject;
        Animator anim = selectedCharacter.GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("Selected"))
            {
                anim.CrossFade(clip.name, animFadeDur);
                break;
            }
        }

        //Finds the animator of the previously selected character and sets the animation back to idle
        Transform oldCharacterT = selectionTransform.GetChild(selectedCharacterIndex - 1 + moveDir);
        GameObject oldCharacter = oldCharacterT.gameObject;
        Animator oldAnim = oldCharacter.GetComponent<Animator>();
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("Idle"))
            {
                oldAnim.CrossFade(clip.name, animFadeDur);
                break;
            }
        }

        selectionTransform.position = targetPos;
        canChangeSelection = true;
    }
}
