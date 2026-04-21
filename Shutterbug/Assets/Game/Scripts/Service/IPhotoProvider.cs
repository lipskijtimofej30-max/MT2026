using System;
using UnityEngine;

namespace Game.Scripts.Service
{
    public interface IPhotoProvider
    {
        Texture2D Photo { get; set; }
        PhotoScore Score { get; set; }

        event Action<Texture2D, PhotoScore> OnPhotoAndScoreReady;
    }
}