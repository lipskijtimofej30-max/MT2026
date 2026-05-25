using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] private DialogueService dialogueService;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //dialogueService.StartDialogue();
        }
    }
}
