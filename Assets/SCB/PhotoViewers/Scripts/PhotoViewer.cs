using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCB.PhotoViewers
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PhotoViewer))]
    public class PhotoViewerEditor : Editor
    {
        [MenuItem("GameObject/SCB/PhotoViewer/PhotoViewer", false, 10)]
        public static void CreatePhotoViewer(MenuCommand menuCommand)
        {
            var photoViewer = new GameObject("PhotoViewer");
            photoViewer.AddComponent<PhotoViewer>();
            GameObjectUtility.SetParentAndAlign(photoViewer, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(photoViewer, "Create " + photoViewer.name);
            Selection.activeObject = photoViewer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var photoViewer = target as PhotoViewer;
            if (photoViewer == null) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Photos", EditorStyles.boldLabel);
            photoViewer.SavedPath = EditorGUILayout.TextField("Saved Path", photoViewer.SavedPath);
            if (GUILayout.Button("Select Photo Folder"))
            {
                var newPath = EditorUtility.OpenFolderPanel("Select Photo Folder", photoViewer.SavedPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    photoViewer.SavedPath = GetRelativePath(newPath, Application.dataPath);
                }
            }
            else if (GUILayout.Button("Add Photo"))
            {
                var photo = CreateInstance<Photo>();
                var newPhotoName = AssetDatabase.GenerateUniqueAssetPath($"{photoViewer.SavedPath}/New Photo.asset");
                AssetDatabase.CreateAsset(photo, newPhotoName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                photoViewer.photos.Add(photo);
            }
            else if (GUILayout.Button("Remove Photo"))
            {
                var photo = photoViewer.photos[photoViewer.photos.Count - 1];
                var toDeleteAsset = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(photo));
                AssetDatabase.DeleteAsset(toDeleteAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                photoViewer.photos.Remove(photo);
                EditorUtility.FocusProjectWindow();
            }
        }

        // private string GetRelativePath(string path)
        // {
            // https://forum.unity.com/threads/editorscript-getting-a-path-that-works-for-prefab-saving.63603/
            // string relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
            
        //     var projectPath = Application.dataPath;
        // }

        private string GetRelativePath(string path, string absolutePath)
        {
            var pathUri = new Uri(path);
            var absolutePathUri = new Uri(absolutePath);
            return absolutePathUri.MakeRelativeUri(pathUri).ToString();
        }
    }
#endif

    public class PhotoViewer : MonoBehaviour
    {
        [HideInInspector]
        public string SavedPath = "Assets/SCB/PhotoViewers/Photos";

        public List<Photo> photos;

        private void Start()
        {

        }
    }
}
