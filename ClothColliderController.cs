using System.Collections.Generic;
using System.Linq;
using AIChara;
using IllusionUtility.GetUtility;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;

namespace AI_ClothColliders
{
    public class ClothColliderController : CharaCustomFunctionController
    {
        private readonly List<ClothSphereColliderPair> _sphereColliders = new List<ClothSphereColliderPair>();
        private readonly List<CapsuleCollider> _capsuleColliders = new List<CapsuleCollider>();

        public void ApplyColliders() => _applyColliders = true;
        private bool _applyColliders;

        protected override void OnCardBeingSaved(GameMode currentGameMode) { }
        protected override void OnReload(GameMode currentGameMode, bool maintainState) => ApplyColliders();
        protected override void OnCoordinateBeingLoaded(ChaFileCoordinate coordinate) => ApplyColliders();

        protected override void Start()
        {
            base.Start();
            foreach (var colliderData in ClothCollidersPlugin.LegSphereColliderData)
                _sphereColliders.Add(new ClothSphereColliderPair(AddSphereCollider(colliderData.Key), AddSphereCollider(colliderData.Value)));
            foreach (var colliderData in ClothCollidersPlugin.LegCapsuleColliderData)
                _capsuleColliders.Add(AddCapsuleCollider(colliderData));
        }

        private void LateUpdate()
        {
            if (_applyColliders)
            {
                foreach (Cloth cloth in GetComponentsInChildren<Cloth>())
                {
                    // Add colliders if they aren't already added while perserving any other clliders
                    cloth.sphereColliders = cloth.sphereColliders.Except(_sphereColliders).Concat(_sphereColliders).ToArray();
                    cloth.capsuleColliders = cloth.capsuleColliders.Except(_capsuleColliders).Concat(_capsuleColliders).ToArray();
                }

                _applyColliders = false;
            }
        }

        private SphereCollider AddSphereCollider(SphereColliderData sphereColliderData)
        {
            if (sphereColliderData == null)
                return null;
            string colliderName = $"{ClothCollidersPlugin.PluginName}_{sphereColliderData.BoneName}";
            if (!sphereColliderData.ColliderNamePostfix.IsNullOrEmpty())
                colliderName += $"_{sphereColliderData.ColliderNamePostfix}";
            return AddSphereCollider(sphereColliderData.BoneName, colliderName, sphereColliderData.ColliderRadius, sphereColliderData.ColliderCenter);
        }

        private SphereCollider AddSphereCollider(string boneName, string colliderName, float colliderRadius = 0.5f, Vector3 colliderCenter = new Vector3())
        {
            //Check if the bone exists
            var bone = ChaControl.transform.FindLoop(boneName);
            if (bone == null)
                return null;

            var existing = bone.transform.Find(colliderName);
            if (existing)
                return bone.GetComponent<SphereCollider>();

            //Build the collider
            var colliderObject = new GameObject(colliderName);
            var collider = colliderObject.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;
            collider.center = colliderCenter;
            colliderObject.transform.SetParent(bone.transform, false);
            return collider;
        }

        private CapsuleCollider AddCapsuleCollider(CapsuleColliderData sphereColliderData)
        {
            if (sphereColliderData == null)
                return null;
            string colliderName = $"{ClothCollidersPlugin.PluginName}_{sphereColliderData.BoneName}";
            if (!sphereColliderData.ColliderNamePostfix.IsNullOrEmpty())
                colliderName += $"_{sphereColliderData.ColliderNamePostfix}";
            return AddCapsuleCollider(sphereColliderData.BoneName, colliderName, sphereColliderData.ColliderRadius, sphereColliderData.CollierHeight, sphereColliderData.ColliderCenter, sphereColliderData.Direction);
        }

        private CapsuleCollider AddCapsuleCollider(string boneName, string colliderName, float colliderRadius = 0.5f, float collierHeight = 0f, Vector3 colliderCenter = new Vector3(), int colliderDirection = 0)
        {
            //Check if the bone exists
            var bone = ChaControl.transform.FindLoop(boneName);
            if (bone == null)
                return null;

            var existing = bone.transform.Find(colliderName);
            if (existing)
                return bone.GetComponent<CapsuleCollider>();

            //Build the collider
            var colliderObject = new GameObject(colliderName);
            var collider = colliderObject.AddComponent<CapsuleCollider>();
            collider.radius = colliderRadius;
            collider.center = colliderCenter;
            collider.height = collierHeight;
            collider.direction = colliderDirection;
            colliderObject.transform.SetParent(bone.transform, false);
            return collider;
        }
    }
}