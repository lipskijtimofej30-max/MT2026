using System;
using Cinemachine;
using UnityEngine;

namespace Game.Scripts
{
    public class PhotoEvaluator
    {
        public PhotoScore CalculateScore(BaseAnimalAI animalAI, AnimalState state, CinemachineVirtualCamera camera)
        {
            PhotoScore photoScore = new PhotoScore();
            
            if (animalAI == null)
            {
                Debug.LogError("CalculateScore: animalAI равен null");
                return photoScore;
            }
            if (animalAI.StateMachine == null)
            {
                Debug.LogError("CalculateScore: StateMachine равен null");
                return photoScore;
            }
            if (animalAI.StateMachine.CurrentState == null)
            {
                Debug.LogWarning("CalculateScore: CurrentState равен null, возвращается нулевой счёт");
                return photoScore;
            }

            float multiplier;
            if (animalAI.StateMachine.CurrentAnimalState == state)
                multiplier = 1f;
            else
                multiplier = 0.2f;
            photoScore.PosePoints = Mathf.RoundToInt(100f * multiplier);
            
            float distance = Vector3.Distance(animalAI.transform.position, camera.transform.position);
            photoScore.SizePoints = Mathf.RoundToInt(Mathf.Clamp(100000f/(distance * camera.m_Lens.FieldOfView), 0.1f, 500f));
            
            var viewportPosition = Camera.main.WorldToViewportPoint(animalAI.transform.position);
            float distanceFromCenter = Vector2.Distance(new Vector2(0.5f, 0.5f), new  Vector2(viewportPosition.x, viewportPosition.y));
            photoScore.PlacementPoints = Mathf.RoundToInt(Mathf.Clamp(300f * (1f - distanceFromCenter), 0.1f, 300f));
            
            photoScore.TotalScore = photoScore.PosePoints + photoScore.PlacementPoints + photoScore.SizePoints;
            
            Debug.LogWarning($"Pose points: {photoScore.PosePoints}, Size points: {photoScore.SizePoints}, Placement points: {photoScore.PlacementPoints}(distance to center {distanceFromCenter}), Total points {photoScore.TotalScore}");
            return photoScore;
        }
    }

    public class PhotoScore
    {
        public int PosePoints;
        public int SizePoints;
        public int PlacementPoints;
        public int TotalScore;
    }
}
