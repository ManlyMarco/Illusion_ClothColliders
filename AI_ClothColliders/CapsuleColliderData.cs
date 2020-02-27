using UnityEngine;

namespace AI_ClothColliders
{
    internal class CapsuleColliderData
    {
        public string BoneName;
        public float ColliderRadius;
        public float CollierHeight;
        public Vector3 ColliderCenter;
        public int Direction;
        public string ColliderNamePostfix;

        public CapsuleColliderData(string boneName, float colliderRadius, float collierHeight, Vector3 colliderCenter, int direction, string colliderNamePostfix = "")
        {
            BoneName = boneName;
            ColliderRadius = colliderRadius;
            CollierHeight = collierHeight;
            ColliderCenter = colliderCenter;
            Direction = direction;
            ColliderNamePostfix = colliderNamePostfix;
        }
    }
}