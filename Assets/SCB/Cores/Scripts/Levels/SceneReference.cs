using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCB.Cores
{
    [CreateAssetMenu(fileName = nameof(SceneReference), menuName = "SCB/" + nameof(SceneReference), order = SCBOrdering.Cores + 1)]
    public class SceneReference : AbstractLevelData
    {
        // Add your custom properties and methods here

        // Example property
        public string ScenePath { get; set; }

        // Example method
        public void LoadScene()
        {
            SceneManager.LoadScene(ScenePath);
        }
    }
}
