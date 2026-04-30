using Game.Scripts;
using Game.Signals;
using Zenject;

namespace Game.Data
{
    public class DataHandler
    {
        private AnimalDataRegistry animalDataRegistry;

        public DataHandler(SignalBus signalBus, AnimalDataRegistry animalDataRegistry)
        {
            this.animalDataRegistry = animalDataRegistry;
            signalBus.Subscribe<PlayerCrouchedSignal>(PlayerCrouched);
        }
        
        private void PlayerCrouched(PlayerCrouchedSignal signal)
        {
            if (signal.IsCrouched)
                SetAllMultiplier(0.75f);
            else
                SetAllMultiplier(1f);
        }

        private void SetAllMultiplier(float multiplier)
        {
            animalDataRegistry.SetToSpecialStateDistanceMultiplier(AnimalType.Rabbit, multiplier);
            animalDataRegistry.SetViewAngleMultiplier(AnimalType.Rabbit, multiplier);
            animalDataRegistry.SetViewDistanceMultiplier(AnimalType.Rabbit, multiplier);
        }
    }
}