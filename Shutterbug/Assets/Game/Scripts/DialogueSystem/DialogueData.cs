using System;
using UnityEngine;

[Serializable]
public class DialogueData
{
    [field:SerializeField, TextArea] public string TextDialogue {get; private set;}
    [field:SerializeField] public string AuthorName {get; private set;}
}
