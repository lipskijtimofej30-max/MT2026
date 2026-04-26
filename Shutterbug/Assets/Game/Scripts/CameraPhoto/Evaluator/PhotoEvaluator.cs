using Cinemachine;
using UnityEngine;

namespace Game.Scripts
{
    public class PhotoEvaluator
    {
        public PhotoScore CalculateScore(BaseAnimalAI animalAI, CinemachineVirtualCamera camera)
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

            photoScore.PoseMultiplier = animalAI.StateMachine.CurrentState.GetStateMultiplier();
            
            float distance = Vector3.Distance(animalAI.transform.position, camera.transform.position);
            photoScore.SizePoints = Mathf.RoundToInt(Mathf.Clamp(100000f/(distance * camera.m_Lens.FieldOfView), 0.1f, 500f));
            
            var viewportPosition = Camera.main.WorldToViewportPoint(animalAI.transform.position);
            float distanceFromCenter = Vector2.Distance(new Vector2(0.5f, 0.5f), new  Vector2(viewportPosition.x, viewportPosition.y));
            photoScore.PlacementPoints = Mathf.RoundToInt(Mathf.Clamp(300f * (1f - distanceFromCenter), 0.1f, 300f));
            
            photoScore.TotalScore = (int)(photoScore.PlacementPoints + photoScore.SizePoints * photoScore.PoseMultiplier);
            
            Debug.LogWarning($"Pose points: {photoScore.PoseMultiplier}, Size points: {photoScore.SizePoints}, Placement points: {photoScore.PlacementPoints}(distance to center {distanceFromCenter}), Total points {photoScore.TotalScore}");
            return photoScore;
        }
    }

    public class PhotoScore
    {
        public float PoseMultiplier;
        public float SizePoints;
        public float PlacementPoints;
        public int TotalScore;
    }
}
