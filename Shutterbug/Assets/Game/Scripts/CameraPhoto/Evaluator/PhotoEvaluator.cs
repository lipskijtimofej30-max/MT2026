using System;
using Cinemachine;
using UnityEngine;

namespace Game.Scripts
{
    public class PhotoEvaluator
    {
        public PhotoScore CalculateScore(BaseAnimalAI animalAI,Type stateForCheck, CinemachineVirtualCamera camera)
        {
            PhotoScore photoScore = new PhotoScore();
            if (!IsAnimalInSight(animalAI.transform))
            {
                return photoScore;
            }
            
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
            if (animalAI.StateMachine.CurrentState.GetType() == stateForCheck)
                multiplier = 1f;
            else
                multiplier = 0.2f;
            photoScore.PosePoints = Mathf.RoundToInt(100f * multiplier);
            
            float distance = Vector3.Distance(animalAI.transform.position, camera.transform.position);
            photoScore.SizePoints = Mathf.RoundToInt(Mathf.Clamp(100000f/(distance * camera.m_Lens.FieldOfView), 0.1f, 500f));
            
            var viewportPosition = Camera.main.WorldToViewportPoint(animalAI.transform.position);
            float distanceFromCenter = Vector2.Distance(new Vector2(0.5f, 0.5f), new  Vector2(viewportPosition.x, viewportPosition.y));
            photoScore.PlacementPoints = Mathf.RoundToInt(Mathf.Clamp(10000f * distanceFromCenter,0.1f, 300f));
            
            photoScore.TotalScore = photoScore.PosePoints + photoScore.PlacementPoints + photoScore.SizePoints;
            
            Debug.LogWarning($"Pose points: {photoScore.PosePoints}, Size points: {photoScore.SizePoints}, Placement points: {photoScore.PlacementPoints}(distance to center {distanceFromCenter}), Total points {photoScore.TotalScore}");
            return photoScore;
        }
        
        private bool IsAnimalInSight(Transform animal, float tolerancePixels = 200f)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(animal.position);
            Vector2 center = new Vector2(Screen.width/2, Screen.height/2);
            float distance = Vector2.Distance(screenPoint, center);
            return distance < tolerancePixels && screenPoint.z > 0;
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
