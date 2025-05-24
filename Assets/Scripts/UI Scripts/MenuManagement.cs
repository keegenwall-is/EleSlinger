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

    private bool characterSelect = false;
    private int mainMenuBtnSelect = 0;
    private int[] playerCharacterSelections = {0, 0, 0, 0};
    private int noOfCharacters = 0;

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
                if (keyboard.aKey.wasPressedThisFrame)
                {
                    if (playerCharacterSelections[i] > 1)
                    {
                        playerCharacterSelections[i]--;
                        Vector3 pos = characterLists[i].gameObject.transform.position;
                        pos.x += 10;
                        characterLists[i].gameObject.transform.position = pos;
                    }
                } else if (keyboard.dKey.wasPressedThisFrame)
                {
                    if (playerCharacterSelections[i] < noOfCharacters)
                    {
                        playerCharacterSelections[i]++;
                        Vector3 pos = characterLists[i].gameObject.transform.position;
                        pos.x -= 10;
                        characterLists[i].gameObject.transform.position = pos;
                    }
                }
            }
            else if (playerControllers[i] is Gamepad gamepad)
            {
                
            }
        }
    }
}
