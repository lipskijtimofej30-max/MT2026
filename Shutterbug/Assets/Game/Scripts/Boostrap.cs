using System;
using Game.Scripts.Data;
using Game.Service;
using UnityEngine;

namespace Game.Scripts
{
    public class Boostrap : MonoBehaviour
    {
        [SerializeField] private PhotoRewardConfig _photoRewardConfig;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            var photoRewardService = new PhotoRewardService(_photoRewardConfig);
        }
    }
}