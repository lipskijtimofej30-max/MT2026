using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class Database<T> : ScriptableObject
    {
        [field:SerializeField] public List<T> Databased { get; set; }
    }
}