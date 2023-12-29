using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCB.Shared.Boots
{
    public class ActiveSceneEnabler : MonoBehaviour
    {
        public bool SetupEventSystemOnStart = true;

        private void Awake()
        {
            if (SceneManager.GetActiveScene() == gameObject.scene)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (SetupEventSystemOnStart)
            {
                if (GameObject.Find("EventSystem") == null)
                {
                    var eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
                }
            }   
        }
    }
}
