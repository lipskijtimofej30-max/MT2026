using UnityEngine;

namespace Game.Scripts.UI
{
    using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

    public class RadialMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private float _radius = 200f;
        [SerializeField] private Image _selectionHighlight;

        private PlayerBait _playerBait;
        private List<GameObject> _spawnedButtons = new();
        private int _hoveredIndex = -1;

        [Inject]
        private void Construct(PlayerBait playerBait)
        {
            _playerBait = playerBait;
            Close();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0.2f;
            BuildMenu();
        }

        private void BuildMenu()
        {
            foreach (var btn in _spawnedButtons) Destroy(btn);
            _spawnedButtons.Clear();

            var baits = _playerBait.AvailableBaitTypes;
            for (int i = 0; i < baits.Count; i++)
            {
                float angle = i * Mathf.PI * 2 / baits.Count;
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;

                var btn = Instantiate(_buttonPrefab, _container);
                btn.GetComponent<RectTransform>().anchoredPosition = pos;
                
                btn.GetComponentInChildren<Image>().sprite = baits[i].Icon; 
                _spawnedButtons.Add(btn);
            }
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, Input.mousePosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _container,
                Input.mousePosition,
                null, // Если Canvas в Overlay, тут null. Если Camera — укажите камеру.
                out Vector2 localMousePos);
            if (localMousePos.sqrMagnitude < 100f * 100f) return;

            float angle = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            float step = 360f / _playerBait.AvailableBaitTypes.Count;
            _hoveredIndex = Mathf.RoundToInt(angle / step) % _playerBait.AvailableBaitTypes.Count;

            if (_selectionHighlight)
                _selectionHighlight.rectTransform.localRotation = Quaternion.Euler(0, 0,angle);
        }

        public void Close()
        {
            if (_hoveredIndex != -1)
            {
                _playerBait.SetBaitIndex(_hoveredIndex);
            }

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.SetActive(false);
        }
    }
}