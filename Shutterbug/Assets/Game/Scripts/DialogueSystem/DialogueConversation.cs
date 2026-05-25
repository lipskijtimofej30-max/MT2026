using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/Dialogue/Dialogue Conversation")]
public class DialogueConversation : ScriptableObject
{
    [field:SerializeField] public List<DialogueData> Lines {get; set;}
}
