using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Quest
{
    public class QuestCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _selectButton;

        private PhotoQuest _quest;
        private Image _image;
        private Action<PhotoQuest> _onSelected;

        private void Awake()
        {
            _image = gameObject.GetComponentInChildren<Image>();
        }

        public void Setup(PhotoQuest quest, Action<PhotoQuest> onSelected, bool isActive = false)
        {
            _quest = quest;
            _onSelected = onSelected;
            _titleText.text = quest?.Description.ShortTitle ?? "Ничего нету??????";
            if (isActive && _image != null)
                _image.color = Color.yellow;
            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(HandleClick);
        }
        
        private void HandleClick()
        {   
            _onSelected?.Invoke(_quest);
        }
    }
}
