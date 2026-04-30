using Game.Data;
using UnityEngine;

namespace Game.Scripts.Module
{
    public class BaseDataModule : IDataModule
    {
        private readonly AnimalType _type;
        private readonly AnimalConfig _config;
        private readonly AnimalDataRegistry _registry;
        private readonly AnimalConfigState _configState;

        public BaseDataModule(AnimalType type,AnimalConfig config, AnimalDataRegistry registry)
        {
            _config = config;
            _registry = registry;
            _type = type;
            _configState = _registry.GetConfigState(_type);
        }

        public float GetViewDistance()
        {
            var value =_config.ViewDistance * _configState.ViewDistanceMultiplier;
            //Debug.LogWarning($"View distance: {value}");
            return value;
        }

        public float GetViewAngle()
        {
            var value = _config.ViewAngle * _configState.ViewAngleMultiplier;
            //Debug.LogWarning($"View angle: {value}");
            return value;
        }

        public float GetDistanceForSpecialState()
        {
            var value = _config.ToSpecialStateDistance * _configState.ToSpecialStateDistanceMultiplier;
            //Debug.LogWarning($"Distance for special state is {value}");
            return value;
        }
    }
}