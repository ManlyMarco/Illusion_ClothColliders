using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This is an Unity Editor script. To use it you need the correct version of Unity Editor for your game.
/// Place this file in Unity Editor project in Assets/Editor folder, then use the AI_ClothColliders menu.
/// How to use:
/// 1 - Add the character model to the scene
/// 2 - Add colliders to the character model's body, only add them to the left bones (ending with _L)
/// 3 - Use menu option "AI_ClothColliders/Copy L colliders to R" to copy colliders to the right side, adjust if necessary
/// 4 - Use menu option "AI_ClothColliders/Show cloth collider exporter window" and paste the result to your manifest.xml for use with the AI_ClothColliders plugim.
/// Each cloth component is identified by its name, so you need to export each cloth component separately and then combine the exports into your zipmod's manifest.xml file.
/// </summary>
public class ClothColliderInfoExportWindow : EditorWindow
{
    private Cloth _currentCloth;
    private CategoryNo _currentCategoryNo;
    private int _currentId;
    private string _export;

    [MenuItem("AI_ClothColliders/Show cloth collider exporter window")]
    public static void ShowWindow()
    {
        var w = GetWindow(typeof(ClothColliderInfoExportWindow));
        w.titleContent = new GUIContent("Collider Exporter");
    }

    private void OnGUI()
    {
        GUI.changed = false;
        _currentCloth = (Cloth)EditorGUILayout.ObjectField("Cloth with colliders", _currentCloth, typeof(Cloth), true);

        if (_currentCloth == null)
        {
            GUILayout.Label("Select a cloth component with colliders to be exported.");
            return;
        }

        GUILayout.Label("Item ID and Item category must be the same as values inside of your list file.");
        int.TryParse(EditorGUILayout.TextField("Item ID", _currentId.ToString(CultureInfo.InvariantCulture)), out _currentId);
        _currentCategoryNo = (CategoryNo)EditorGUILayout.EnumPopup("Item category", _currentCategoryNo);

        if (GUI.changed)
        {
            try
            {
                var doc = ExportCloth();
                _export = doc.ToString(SaveOptions.None);
            }
            catch (Exception ex)
            {
                _export = "Failed to export collider information!\n\n" + ex;
            }
        }

        GUILayout.Label("The exported information below has to be added to your zipmod's manifest file.");
        EditorGUILayout.TextArea(_export, GUILayout.ExpandHeight(true));
    }

    private XDocument ExportCloth()
    {
        var doc = new XDocument();
        var root = new XElement("manifest");
        doc.AddFirst(root);
        var root2 = new XElement("AI_ClothColliders");
        root.AddFirst(root2);
        var clothRoot = new XElement("cloth");
        clothRoot.SetAttributeValue("id", _currentId.ToString());
        clothRoot.SetAttributeValue("category", _currentCategoryNo.ToString());
        clothRoot.SetAttributeValue("clothName", _currentCloth.name);
        root2.AddFirst(clothRoot);

        foreach (var clothSphereColliderPair in _currentCloth.sphereColliders)
        {
            if (clothSphereColliderPair.first == null && clothSphereColliderPair.second == null) continue;

            var col = new XElement("SphereColliderPair");
            clothRoot.Add(col);

            var sphereCollider = clothSphereColliderPair.first;
            if (sphereCollider != null)
            {
                var colliderElement = new XElement("first",
                    new XAttribute("boneName", sphereCollider.transform.name),
                    new XAttribute("radius", sphereCollider.radius.ToString("F", CultureInfo.InvariantCulture)),
                    new XAttribute("center", ExportVector(sphereCollider.center)));
                col.Add(colliderElement);
            }

            sphereCollider = clothSphereColliderPair.second;
            if (sphereCollider != null)
            {
                var colliderElement = new XElement("second",
                    new XAttribute("boneName", sphereCollider.transform.name),
                    new XAttribute("radius", sphereCollider.radius.ToString("F", CultureInfo.InvariantCulture)),
                    new XAttribute("center", ExportVector(sphereCollider.center)));
                col.Add(colliderElement);
            }
        }

        foreach (var capsuleCollider in _currentCloth.capsuleColliders)
        {
            if (capsuleCollider != null)
            {
                var colliderElement = new XElement("CapsuleCollider",
                    new XAttribute("boneName", capsuleCollider.transform.name),
                    new XAttribute("radius", capsuleCollider.radius.ToString("F", CultureInfo.InvariantCulture)),
                    new XAttribute("center", ExportVector(capsuleCollider.center)),
                    new XAttribute("height", capsuleCollider.height.ToString("F", CultureInfo.InvariantCulture)),
                    new XAttribute("direction", capsuleCollider.direction.ToString("D", CultureInfo.InvariantCulture)));
                clothRoot.Add(colliderElement);
            }
        }

        return doc;
    }

    private static string ExportVector(Vector3 vector)
    {
        return $"{vector.x.ToString("F", CultureInfo.InvariantCulture)}, {vector.y.ToString("F", CultureInfo.InvariantCulture)}, {vector.z.ToString("F", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Names are important and can't be changed
    /// </summary>
    private enum CategoryNo
    {
        fo_top,// = 240,
        fo_bot,
        fo_inner_t,
        fo_inner_b,
        fo_gloves,
        fo_panst,
        fo_socks,
        fo_shoes,
    }

    [MenuItem("AI_ClothColliders/Copy L colliders to R")]
    private static void CopyLtoR()
    {
        foreach (var collider in GameObject.FindObjectsOfType<Collider>())
        {
            if (collider.transform.name.EndsWith("L"))
            {
                var transformName = collider.transform.name.ToCharArray();
                transformName[transformName.Length - 1] = 'R';
                var s = new string(transformName);
                var tr = GameObject.Find(s);
                DestroyImmediate(tr.GetComponent(collider.GetType()));
                foreach (var transform in tr.transform.Cast<Transform>().ToArray())
                {
                    if (transform.name == collider.transform.name + "(Clone)")
                        DestroyImmediate(transform.gameObject);
                }

                UnityEditorInternal.ComponentUtility.CopyComponent(collider);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(tr);
                var copy = tr.GetComponent(collider.GetType());

                var sp = copy as SphereCollider;
                if (sp != null)
                {
                    var center = sp.center;
                    sp.center = new Vector3(center.x * -1, center.y, center.z);
                }

                var cp = copy as CapsuleCollider;
                if (cp != null)
                {
                    var center = cp.center;
                    cp.center = new Vector3(center.x * -1, center.y, center.z);
                }

                Debug.Log("Added copy to " + tr.name);
            }
        }
    }
}
