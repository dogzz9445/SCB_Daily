// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SCB.Cores.Extensions
{
    /// <summary>
    /// Extension methods for Unity's GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Export mesh data of current GameObject, and children if enabled, to file provided in OBJ format
        /// </summary>
        public static async Task ExportOBJAsync(this GameObject root, string filePath, bool includeChildren = true)
        {
            await OBJWriterUtility.ExportOBJAsync(root, filePath, includeChildren);
        }

        /// <summary>
        /// Set all GameObject children active or inactive based on argument
        /// </summary>
        /// <param name="root">GameObject parent to traverse from</param>
        /// <param name="isActive">Indicates whether children GameObjects should be active or not</param>
        /// <remarks>
        /// Does not call SetActive on the top level GameObject, only its children
        /// </remarks>
        public static void SetChildrenActive(this GameObject root, bool isActive)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                root.transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        public static void SetLayerRecursively(this GameObject root, int layer)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root), "Root transform can't be null.");
            }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Set the layer to the given object and the full hierarchy below it and cache the previous layers in the out parameter.
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="layer">The layer to apply</param>
        /// <param name="cache">The previously set layer for each object</param>
        public static void SetLayerRecursively(this GameObject root, int layer, out Dictionary<GameObject, int> cache)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }

            cache = new Dictionary<GameObject, int>();

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                cache[child.gameObject] = child.gameObject.layer;
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Reapplies previously cached hierarchy layers
        /// </summary>
        /// <param name="root">Start point of the traverse</param>
        /// <param name="cache">The previously set layer for each object</param>
        public static void ApplyLayerCacheRecursively(this GameObject root, Dictionary<GameObject, int> cache)
        {
            if (root == null) { throw new ArgumentNullException(nameof(root)); }
            if (cache == null) { throw new ArgumentNullException(nameof(cache)); }

            foreach (var child in root.transform.EnumerateHierarchy())
            {
                int layer;
                if (!cache.TryGetValue(child.gameObject, out layer)) { continue; }
                child.gameObject.layer = layer;
                cache.Remove(child.gameObject);
            }
        }

        /// <summary>
        /// Determines whether or not a game object's layer is included in the specified layer mask.
        /// </summary>
        /// <param name="gameObject">The game object whose layer to test.</param>
        /// <param name="layerMask">The layer mask to test against.</param>
        /// <returns>True if <paramref name="gameObject"/>'s layer is included in <paramref name="layerMask"/>, false otherwise.</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            LayerMask gameObjectMask = 1 << gameObject.layer;
            return (gameObjectMask & layerMask) == gameObjectMask;
        }

        /// <summary>
        /// Apply the specified delegate to all objects in the hierarchy under a specified game object.
        /// </summary>
        /// <param name="root">Root game object of the hierarchy.</param>
        /// <param name="action">Delegate to apply.</param>
        public static void ApplyToHierarchy(this GameObject root, Action<GameObject> action)
        {
            action(root);
            Transform[] items = root.GetComponentsInChildren<Transform>();

            for (var i = 0; i < items.Length; i++)
            {
                action(items[i].gameObject);
            }
        }

        /// <summary>
        /// Find the first component of type <typeparamref name="T"/> in the ancestors of the specified game object.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="gameObject">Game object for which ancestors must be considered.</param>
        /// <param name="includeSelf">Indicates whether the specified game object should be included.</param>
        /// <returns>The component of type <typeparamref name="T"/>. Null if it none was found.</returns>
        public static T FindAncestorComponent<T>(this GameObject gameObject, bool includeSelf = true) where T : Component
        {
            return gameObject.transform.FindAncestorComponent<T>(includeSelf);
        }

        /// <summary>
        /// Perform an action on every component of type T that is on this GameObject
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <param name="gameObject">this gameObject</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEachComponent<T>(this GameObject gameObject, Action<T> action) where T : Component
        {
            foreach (T i in gameObject.GetComponents<T>())
            {
                action(i);
            }
        }

        /// <summary>
        /// Destroys GameObject appropriately depending if in edit or playmode
        /// </summary>
        /// <param name="gameObject">GameObject to destroy</param>
        /// <param name="t">time in seconds at which to destroy GameObject if applicable</param>
        public static void DestroyGameObject(GameObject gameObject, float t = 0.0f)
        {
            UnityObjectExtensions.DestroyObject(gameObject, t);
        }

        /// <summary>
        /// Checks if any MonoBehaviour on the given GameObject is using the RequireComponentAttribute requiring type T
        /// </summary>
        /// <remarks>Only functions when called within a UNITY_EDITOR context. Outside of UNITY_EDITOR, always returns false</remarks>
        /// <typeparam name="T">The potentially required component</typeparam>
        /// <param name="gameObject">the GameObject requiring the component</param>
        /// <param name="requiringTypes">A list of types that do require the component in question</param>
        /// <returns>true if <typeparamref name="T"/> appears in any RequireComponentAttribute, otherwise false </returns>
        public static bool IsComponentRequired<T>(this GameObject gameObject, out List<Type> requiringTypes) where T : Component
        {
            var genericType = typeof(T);
            requiringTypes = null;

#if UNITY_EDITOR
            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null)
                {
                    continue;
                }

                var monoBehaviourType = monoBehaviour.GetType();
                var attributes = Attribute.GetCustomAttributes(monoBehaviourType);

                foreach (var attribute in attributes)
                {
                    if (attribute is RequireComponent requireComponentAttribute)
                    {
                        if (requireComponentAttribute.m_Type0 == genericType ||
                            requireComponentAttribute.m_Type1 == genericType ||
                            requireComponentAttribute.m_Type2 == genericType)
                        {
                            if (requiringTypes == null)
                            {
                                requiringTypes = new List<Type>();
                            }

                            requiringTypes.Add(monoBehaviourType);
                        }
                    }
                }
            }
#endif // UNITY_EDITOR

            return requiringTypes != null;
        }

        // public static void SetLayerRecursively(
        // this GameObject self,
        // int layer
        // )
        // {
        //     self.layer = layer;

        //     foreach (Transform n in self.transform)
        //     {
        //         SetLayerRecursively(n.gameObject, layer);
        //     }
        // }

        public static void SetLayerRecursivelyWithDisableStatic(
            this GameObject self,
            int layer
        )
        {
            if (!self.activeInHierarchy)
                return;
            self.isStatic = false;
            self.layer = layer;

            foreach (Transform n in self.transform)
            {
                SetLayerRecursively(n.gameObject, layer);
            }
        }

        public static void SetTagRecursively(
            this GameObject self,
            string tag
        )
        {
            self.tag = tag;

            foreach (Transform n in self.transform)
            {
                SetTagRecursively(n.gameObject, tag);
            }
        }


        public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
        {
            foreach (Transform child in self)
            {
                if (selector(child))
                {
                    return child;
                }

                var finding = child.FindRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }

            return null;
        }


        public static List<Transform> FindChildrenRecursive(this Transform self, string exactName)
        {
            return self.FindChildrenRecursive(child => child.name == exactName);
        }

    public static List<Transform> FindChildrenRecursive(this Transform self, Func<Transform, bool> selector, List<Transform> children = null)
    {
        if (children == null)
        {
            children = new List<Transform>();
        }
        foreach (Transform child in self)
        {
            if (selector(child))
            {
                children.Add(child);
            }

            child.FindChildrenRecursive(selector, children);
        }

        return children;
    }
    }
}
