using UnityEngine;

namespace AI_ClothColliders
{
    internal class SphereColliderData
    {
        public string BoneName;
        public float ColliderRadius;
        public Vector3 ColliderCenter;
        public string UniqueId;

        public SphereColliderData(string boneName, float colliderRadius, Vector3 colliderCenter, string uniqueId)
        {
            BoneName = boneName;
            ColliderRadius = colliderRadius;
            ColliderCenter = colliderCenter;
            UniqueId = uniqueId;
        }
    }
}
