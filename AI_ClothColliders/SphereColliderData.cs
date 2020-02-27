using UnityEngine;

namespace AI_ClothColliders
{
    internal class SphereColliderData
    {
        public string BoneName;
        public float ColliderRadius;
        public float CollierHeight;
        public Vector3 ColliderCenter;
        public string ColliderNamePostfix;

        public SphereColliderData(string boneName, float colliderRadius, Vector3 colliderCenter, string colliderNamePostfix = "")
        {
            BoneName = boneName;
            ColliderRadius = colliderRadius;
            ColliderCenter = colliderCenter;
            ColliderNamePostfix = colliderNamePostfix;
        }
    }
}
