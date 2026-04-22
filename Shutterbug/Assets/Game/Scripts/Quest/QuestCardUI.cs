using System;
using Game.Scripts.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _selectButton;

        private PhotoQuest _quest;
        private Action<PhotoQuest> _onSelected;

        public void Setup(PhotoQuest quest, Action<PhotoQuest> onSelected)
        {
            _quest = quest;
            _onSelected = onSelected;
            _titleText.text = quest.Description.ShortTitle;
            
            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(HandleClick);
        }
        
        private void HandleClick()
        {   
            _onSelected?.Invoke(_quest);
        }
    }
}