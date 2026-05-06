using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts
{
    public class PlayerBaseHandler : MonoBehaviour
    {
        private bool _inBase;
        public bool InBase => _inBase;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Base>())
                _inBase = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Base>())
                _inBase = false;
        }
    }
}