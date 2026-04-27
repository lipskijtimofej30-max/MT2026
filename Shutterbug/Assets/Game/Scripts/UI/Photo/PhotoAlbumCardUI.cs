using System;
using Game.Scripts.UI.Photo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    [RequireComponent(typeof(ButtonCardDelete))]
    public class PhotoAlbumCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _selectedButton;
        [SerializeField] private RawImage _image;
        [SerializeField] private Image _outlineImage;
        
        private PhotoRecord _photoRecord;
        private Action<PhotoRecord> _onSelected;
        private ButtonCardDelete _deleteButton;
        
        public PhotoRecord PhotoRecord => _photoRecord;

        public void Initialize(PhotoRecord photoRecord, PhotoService photoService, Action<PhotoRecord> onSelected, bool isActive = false)
        {
            _photoRecord = photoRecord;
            _onSelected = onSelected;
            _deleteButton = GetComponent<ButtonCardDelete>();
            
            _image.texture = _photoRecord.thumbnail;
            
            _descriptionText.text = $"Животное: {_photoRecord.animalType}" +
                                    $"\nв состояние: {_photoRecord.animalStateType}" + 
                                    $"\nочков: {_photoRecord.photoScore.TotalScore}" +
                                    $"\n(размер - {_photoRecord.photoScore.SizePoints}, положение - {_photoRecord.photoScore.PlacementPoints}, множитель редкости - {_photoRecord.photoScore.PoseMultiplier})" +
                                    $"\nвремя {_photoRecord.timestamp}";
            
            if (_outlineImage != null)
                _outlineImage.gameObject.SetActive(isActive);
            
            _selectedButton.onClick.RemoveAllListeners();
            _selectedButton.onClick.AddListener(() => _onSelected?.Invoke(_photoRecord));
            _deleteButton.Initialize(photoService);
        }
    }
}