using System.Linq;
using IllusionUtility.GetUtility;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;

namespace ClothColliders
{
    public class ClothColliderController : CharaCustomFunctionController
    {
        protected override void OnCardBeingSaved(GameMode currentGameMode) { }

        public void UpdateColliders(int kind)
        {
            var clothPart = ChaControl.nowCoordinate.clothes.parts[kind];
            var clothObj = ChaControl.objClothes[kind];
            if (clothPart == null || clothObj == null) return;

            ClothCollidersPlugin.SphereColliders.TryGetValue(ClothCollidersPlugin.GetDictKey(kind, clothPart.id), out var colliders);
            ClothCollidersPlugin.CapsuleColliders.TryGetValue(ClothCollidersPlugin.GetDictKey(kind, clothPart.id), out var capsuleColliders);

            if ((colliders == null || colliders.Count == 0) && (capsuleColliders == null || capsuleColliders.Count == 0)) return;

            var targets = clothObj.GetComponentsInChildren<Cloth>(true);
            foreach (var target in targets)
            {
                var sphereTargets = colliders?.Where(x => x.ClothName == target.name).ToList();
                var sphereResults = new ClothSphereColliderPair[sphereTargets?.Count ?? 0];
                if (sphereTargets != null && sphereTargets.Count > 0)
                {
                    for (var index = 0; index < sphereTargets.Count; index++)
                    {
                        var colliderPair = sphereTargets[index];
                        var c1 = AddSphereCollider(colliderPair.first);
                        var c2 = AddSphereCollider(colliderPair.second);
                        sphereResults[index] = new ClothSphereColliderPair(c1, c2);
                    }

                    if (sphereResults.Length > 0)
                    {
                        ClothCollidersPlugin.Logger.LogDebug("Added sphere colliders to bone " + target.name + ": " +
                                                             string.Join(", ",
                                                                 sphereResults
                                                                     .SelectMany(pair => new[] { pair.second, pair.first })
                                                                     .Where(x => x != null)
                                                                     .Select(x => x.transform.parent.name)
                                                                     .ToArray()));
                    }
                }

                var capsuleTargets = capsuleColliders?.Where(x => x.ClothName == target.name).ToList();
                var capsuleResults = new CapsuleCollider[capsuleColliders?.Count ?? 0];
                if (capsuleTargets != null && capsuleTargets.Count > 0)
                {
                    for (var index = 0; index < capsuleTargets.Count; index++)
                    {
                        var capsuleCollider = capsuleTargets[index];
                        capsuleResults[index] = AddCapsuleCollider(capsuleCollider);
                    }

                    if (capsuleResults.Length > 0)
                    {
                        ClothCollidersPlugin.Logger.LogDebug("Added capsule colliders to bone " + target.name + ": " +
                                                             string.Join(", ",
                                                                 capsuleResults.Where(x => x != null)
                                                                     .Select(x => x.transform.parent.name)
                                                                     .ToArray()));
                    }
                }

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

            // Debug logging ---
            var targetBonesNames = (colliders ?? Enumerable.Empty<SphereColliderPair>()).Select(x => x.ClothName)
                .Concat((capsuleColliders ?? Enumerable.Empty<CapsuleColliderData>()).Select(x => x.ClothName)).Distinct().ToList();
            var clothNames = targets.Where(x => x != null).Select(cloth => cloth.name).ToList();

            ClothCollidersPlugin.Logger.LogDebug("Cleared old colliders and applied new colliders to cloths: " + string.Join(", ", targetBonesNames.Intersect(clothNames).ToArray()));

            var missing = targetBonesNames.Except(clothNames).ToList();
            if (missing.Count > 0)
                ClothCollidersPlugin.Logger.LogWarning("Could not find following bones to apply colliders: " + string.Join(", ", missing.OrderBy(s => s).ToArray()));
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
                colliderObject.transform.localScale = Vector3.one;
                colliderObject.transform.localPosition = Vector3.zero;
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