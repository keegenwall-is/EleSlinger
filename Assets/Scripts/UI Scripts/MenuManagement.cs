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

    private bool characterSelect = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (characterSelect)
        {
            CheckNewPlayers();
        }
    }

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

    private void CheckNewPlayers()
    {
        
    }
}
