using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue Database", menuName = "Game/Dialogue/Dialogue Database", order = 0)]
    public class DialogueDatabase : Database<DialogueEntry> { }
    
    [Serializable]
    public struct DialogueEntry
    {
        public KeyDialogueType EventKey;
        public DialogueConversation Dialogue;
    }
}