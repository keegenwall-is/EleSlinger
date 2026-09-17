using UnityEngine;

public class TutorialInstructionBehaviour : MonoBehaviour
{

    public string instruction;
    public float controlRotation;

    private TutorialManager managerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("Minigame Manager").GetComponent<TutorialManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            managerScript.instructionTexts[managerScript.players.IndexOf(other.gameObject)].text = instruction;
            managerScript.instructionCanvass[managerScript.players.IndexOf(other.gameObject)].SetActive(true);
            managerScript.controlImages[managerScript.players.IndexOf(other.gameObject)].GetComponent<RectTransform>().localEulerAngles = new Vector3 (0f, 0f, controlRotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            managerScript.instructionCanvass[managerScript.players.IndexOf(other.gameObject)].SetActive(false);
        }
    }
}
