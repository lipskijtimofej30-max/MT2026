using System;
using System.Collections.Generic;
using Game.Scripts;
using Zenject;

namespace Game.Data
{
    public class AnimalDataRegistry: IInitializable
    {
        private Dictionary<AnimalType, AnimalConfigState> _configStates;
        public void Initialize()
        {
            _configStates = new();
            foreach (AnimalType type in Enum.GetValues(typeof(AnimalType)))
            {
                _configStates[type] = new AnimalConfigState();
            }
        }

        public AnimalConfigState GetConfigState(AnimalType type)
        {
            if (_configStates.TryGetValue(type, out var state))
                return state;
            
            var newState = new AnimalConfigState();
            _configStates[type] = newState;
            return newState;
        }

        public void SetViewAngleMultiplier(AnimalType type, float multiplier) =>
            GetConfigState(type).ViewAngleMultiplier = multiplier;

        public void SetViewDistanceMultiplier(AnimalType type, float multiplier) =>
            GetConfigState(type).ViewDistanceMultiplier = multiplier;

        public void SetToSpecialStateDistanceMultiplier(AnimalType type, float multiplier)=>
            GetConfigState(type).ToSpecialStateDistanceMultiplier = multiplier;
    }

    [Serializable]
    public class AnimalConfigState
    {
        public float ViewDistanceMultiplier;
        public float ViewAngleMultiplier;
        public float ToSpecialStateDistanceMultiplier;
    }
}