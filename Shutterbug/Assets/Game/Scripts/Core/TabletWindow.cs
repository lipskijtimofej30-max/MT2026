using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class TabletWindow : MonoBehaviour
    {
        public abstract void ToggleWindow();
        public abstract void SetVisible(bool visible);
    }
}