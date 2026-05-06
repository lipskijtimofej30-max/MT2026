using Game.Scripts.CameraPhoto.PhotoAlbum;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Photo
{
    public class ButtonCardDelete : MonoBehaviour
    {
        [SerializeField] private PhotoAlbumCardUI _card;
        [SerializeField] private Button _buttonDelete;
        private PhotoService _photoService;

        public void Initialize(PhotoService photoService)
        {
            _photoService = photoService;
            _card = GetComponent<PhotoAlbumCardUI>();
            _buttonDelete.onClick.AddListener(() => DeleteCard());
        }
        
        private void DeleteCard()
        {
            _photoService.DeletePhoto(_card.PhotoRecord);
            Destroy(_card.gameObject);
        }
    }
}