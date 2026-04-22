using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Game/Quests/Quest Database", order = 0)]
    public class QuestDatabase : Database<PhotoQuest>
    {
        
    }
}