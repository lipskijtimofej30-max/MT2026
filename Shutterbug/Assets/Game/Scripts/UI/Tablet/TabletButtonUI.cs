using System;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class TabletButtonUI : MonoBehaviour
    {
        [SerializeField] private TabletWindow _window;
        [SerializeField] private Button _button;
        [SerializeField] private bool _enabled = true;

        private void Start()
        {
            if (_button == null)
                _button = GetComponent<Button>();
            _button.onClick.AddListener(() => _window.SetVisible(_enabled));
        }
    }
}