using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.Shared.Components
{
    
#if UNITY_EDITOR
    [CustomEditor(typeof(AbstractAssetManagementMonoBehaviour<>), true)]
    public class AssetManagementAbstractMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = target as IAssetManagementMonoBehaviour;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);
            component.BasePath = EditorGUILayout.TextField("Saved Path", component.BasePath);
            if (GUILayout.Button("Select Folder"))
            {
                var newPath = EditorUtility.OpenFolderPanel("Select Folder", component.BasePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    component.BasePath = GetRelativePath(newPath, Application.dataPath);
                }
            }
            else if (GUILayout.Button("Add Asset"))
            {
                component.Add();
            }
            else if (GUILayout.Button("Remove Asset"))
            {
                component.Remove();
            }
        }

        private string GetRelativePath(string path, string absolutePath)
        {
            var pathUri = new Uri(path);
            var absolutePathUri = new Uri(absolutePath);
            return absolutePathUri.MakeRelativeUri(pathUri).ToString();
        }
    }
#endif
    public interface IAssetManagementMonoBehaviour
    {
        string BasePath { get; set; }
        string AssetName { get; set; }
        void Add();
        void Remove();
    }

    public abstract class AbstractAssetManagementMonoBehaviour<T> : MonoBehaviour, IAssetManagementMonoBehaviour where T : ScriptableObject
    {
        [SerializeField]
        public virtual string BasePath { get; set; } = "Assets/SCB";
        [SerializeField]
        public virtual string AssetName { get; set; } = "New Asset";
        [SerializeField]
        public virtual string AssetExtension { get; set; } = "asset";

        public List<T> Assets;
        public virtual void Add()
        {
            #if UNITY_EDITOR
            var asset = Editor.CreateInstance(typeof(T)) as T;
            var newPhotoName = AssetDatabase.GenerateUniqueAssetPath($"{BasePath}/{AssetName}.{AssetExtension}");
            AssetDatabase.CreateAsset(asset, newPhotoName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Assets.Add(asset);
            #endif
        }

        public virtual void Remove()
        {
            #if UNITY_EDITOR
            if (Assets.Count == 0)
                return;
            var asset = Assets[^1];
            // Remove asset from Unity asset database and list
            Assets.Remove(asset);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }
    }
}
