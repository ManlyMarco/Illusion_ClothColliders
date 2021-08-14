using UnityEngine;

namespace ClothColliders
{
    internal class CapsuleColliderData
    {
        public string ClothName { get; }
        public string BoneName;
        public float ColliderRadius;
        public float CollierHeight;
        public Vector3 ColliderCenter;
        public int Direction;
        public string ColliderNamePostfix;

        public CapsuleColliderData(string boneName, float colliderRadius, float collierHeight, Vector3 colliderCenter,
            int direction, string colliderNamePostfix, string clothName)
        {
            ClothName = clothName;
            BoneName = boneName;
            ColliderRadius = colliderRadius;
            CollierHeight = collierHeight;
            ColliderCenter = colliderCenter;
            Direction = direction;
            ColliderNamePostfix = colliderNamePostfix;
        }
    }
}