using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[InitializeOnLoad]
public static class FixClothPainting
{
    private static GameObject _savedSelection;
    private static float _restoreSelectionTimer;

    [MenuItem("AI_ClothColliders/Fix Collider", false, 100)]
    static void FixClothCollider()
    {
        if (Selection.activeGameObject != null)
        {
            SkinnedMeshRenderer meshRenderer = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                Cloth cloth = Selection.activeGameObject.GetComponent<Cloth>();
                if (cloth != null)
                {
                    FixClothCollider(meshRenderer, cloth);

                    // In some cases after fixing the collider the Inspector doesn't update
                    // The only way I've found to fix this is to switch select to another gameObject
                    // and then switch back
                    _savedSelection = Selection.activeGameObject;
                    Selection.activeGameObject = null;
                    _restoreSelectionTimer = Time.realtimeSinceStartup;
                    EditorApplication.update += OnEditorUpdate_RestoreSelection;
                }
            }
        }
    }

    static void OnEditorUpdate_RestoreSelection()
    {
        // Only do this once, then remove the delegate
        if (Time.realtimeSinceStartup > (_restoreSelectionTimer + 0.2f))
        {
            Selection.activeGameObject = _savedSelection;
            EditorApplication.update -= OnEditorUpdate_RestoreSelection;
        }
    }

    // This fix is for Unity 2018.4.2, where you can't paint unless there is a MeshCollider on the cloth, and it matches the current cloth vertex positions
    // In 2019.2.0 they seem to have fixed the issue and no longer raycast to a MeshCollider, instead using the cloth directly
    static void FixClothCollider(SkinnedMeshRenderer meshRenderer, Cloth cloth)
    {
        if (meshRenderer.transform.localScale != Vector3.one ||
            meshRenderer.transform.localRotation != Quaternion.identity ||
            meshRenderer.transform.localPosition != Vector3.zero)
        {
            Debug.LogError("The skin has a transform, this cause cloth weight painting to not work correctly - remove all translation/rotation/scaling from the mesh", meshRenderer.gameObject);
        }

        cloth.ClearTransformMotion();

        meshRenderer.updateWhenOffscreen = true;
        meshRenderer.sharedMesh.RecalculateBounds();

        // Create a MeshCollider if there isn't one
        MeshCollider collider = meshRenderer.gameObject.GetComponent<MeshCollider>();
        if (collider == null)
        {
            collider = meshRenderer.gameObject.AddComponent<MeshCollider>();

        }
        // Set it to disappear once we're done with it
        collider.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        //collider.hideFlags = 0;

        // Update the mesh to the current skinned mesh vertices
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }
}