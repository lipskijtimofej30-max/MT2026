using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/UpgradeSystem/CameraUpgradesConfig")]
public class CameraUpgradesConfig : ScriptableObject
{
    [SerializeField] private List<UpgradeLevel> _levels = new List<UpgradeLevel>();
    public IReadOnlyList<UpgradeLevel> Levels => _levels;
}

[Serializable]
public class UpgradeLevel
{
    public int level = 0;
    public float cooldownDuration = 10f;
    public float zoomSpeed = 10f;
    public float maxFOV = 60f;
    public float minFOV = 30f;
    public float captureDistance = 50f;
}
