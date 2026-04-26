using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoAlbumCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _selectedButton;
        [SerializeField] private RawImage _image;
        [SerializeField] private Image _outlineImage;
        
        private PhotoRecord _photoRecord;
        private Action<PhotoRecord> _onSelected;
        

        public void Init(PhotoRecord photoRecord, Action<PhotoRecord> onSelected, bool isActive = false)
        {
            _photoRecord = photoRecord;
            _onSelected = onSelected;
            
            _image.texture = _photoRecord.thumbnail;
            
            _descriptionText.text = $"Животное: {_photoRecord.animalType}" +
                                    $"\nв состояние: {_photoRecord.animalStateType}" + 
                                    $"\nочков: {_photoRecord.photoScore.TotalScore}" +
                                    $"\n(размер - {_photoRecord.photoScore.SizePoints}, положение - {_photoRecord.photoScore.PlacementPoints}, множитель редкости - {_photoRecord.photoScore.PoseMultiplier})";
            
            if (_outlineImage != null)
                _outlineImage.gameObject.SetActive(isActive);
            
            _selectedButton.onClick.RemoveAllListeners();
            _selectedButton.onClick.AddListener(() => _onSelected?.Invoke(_photoRecord));
        }
    }
}