using System.Linq;
using IllusionUtility.GetUtility;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;

namespace AI_ClothColliders
{
    public class ClothColliderController : CharaCustomFunctionController
    {
        protected override void OnCardBeingSaved(GameMode currentGameMode) { }

        public void UpdateColliders(int kind)
        {
            var clothPart = ChaControl.nowCoordinate.clothes.parts[kind];
            var clothObj = ChaControl.objClothes[kind];
            if (clothPart == null || clothObj == null) return;

            // Sphere colliders -------------
            ClothCollidersPlugin.SphereColliders.TryGetValue(ClothCollidersPlugin.GetDictKey(kind, clothPart.id), out var colliders);
            var sphereResults = new ClothSphereColliderPair[colliders?.Count ?? 0];
            if (colliders != null)
            {
                for (var index = 0; index < colliders.Count; index++)
                {
                    var colliderPair = colliders[index];
                    var c1 = AddSphereCollider(colliderPair.first);
                    var c2 = AddSphereCollider(colliderPair.second);
                    sphereResults[index] = new ClothSphereColliderPair(c1, c2);
                }
            }

            // Capsule colliders -------------
            ClothCollidersPlugin.CapsuleColliders.TryGetValue(ClothCollidersPlugin.GetDictKey(kind, clothPart.id), out var capsuleColliders);
            var capsuleResults = new CapsuleCollider[capsuleColliders?.Count ?? 0];
            if (capsuleColliders != null)
            {
                for (var index = 0; index < capsuleColliders.Count; index++)
                {
                    var capsuleCollider = capsuleColliders[index];
                    capsuleResults[index] = AddCapsuleCollider(capsuleCollider);
                }
            }

            // Applying and cleanup -------------
            var targets = clothObj.GetComponentsInChildren<Cloth>();
            foreach (var target in targets)
            {
                // Destroy old colliders and apply the newly created
                // todo a better way of doing this? would interfere with other plugins adding colliders
                foreach (var existing in target.sphereColliders)
                {
                    // Make sure to not remove colliders reused by AddSphereCollider
                    if (!sphereResults.Any(pair => pair.first == existing.first || pair.second == existing.first))
                        DestroyImmediate(existing.first);
                    if (!sphereResults.Any(pair => pair.first == existing.second || pair.second == existing.second))
                        DestroyImmediate(existing.second);
                }
                target.sphereColliders = sphereResults;

                foreach (var existing in target.capsuleColliders)
                {
                    if (capsuleResults.All(collider => collider != existing))
                        DestroyImmediate(existing);
                }
                target.capsuleColliders = capsuleResults;
            }
        }

        private SphereCollider AddSphereCollider(SphereColliderData sphereColliderData)
        {
            if (sphereColliderData == null)
                return null;
            string colliderName = $"{ClothCollidersPlugin.PluginName}_{sphereColliderData.BoneName}";
            if (!sphereColliderData.UniqueId.IsNullOrEmpty())
                colliderName += $"_{sphereColliderData.UniqueId}";
            return AddSphereCollider(sphereColliderData.BoneName, colliderName, sphereColliderData.ColliderRadius, sphereColliderData.ColliderCenter);
        }

        private SphereCollider AddSphereCollider(string boneName, string colliderName, float colliderRadius = 0.5f, Vector3 colliderCenter = new Vector3())
        {
            // todo find all bones and cache them for later finding to save time
            var bone = ChaControl.transform.FindLoop(boneName);
            if (bone == null)
                return null;

            var colliderObject = bone.transform.Find(colliderName);
            if (colliderObject == null)
            {
                colliderObject = new GameObject(colliderName).transform;
                colliderObject.transform.SetParent(bone.transform, false);
            }

            var collider = colliderObject.GetComponent<SphereCollider>();
            if (collider == null)
                collider = colliderObject.gameObject.AddComponent<SphereCollider>();

            collider.radius = colliderRadius;
            collider.center = colliderCenter;

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
            // todo find all bones and cache them for later finding to save time
            var bone = ChaControl.transform.FindLoop(boneName);
            if (bone == null)
                return null;

            var colliderObject = bone.transform.Find(colliderName);
            if (colliderObject == null)
            {
                colliderObject = new GameObject(colliderName).transform;
                colliderObject.transform.SetParent(bone.transform, false);
            }

            var collider = colliderObject.GetComponent<CapsuleCollider>();
            if (collider == null)
                collider = colliderObject.gameObject.AddComponent<CapsuleCollider>();

            collider.radius = colliderRadius;
            collider.center = colliderCenter;
            collider.height = collierHeight;
            collider.direction = colliderDirection;

            return collider;
        }
    }
}