using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCB.Shared.Boots
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _bootObjects = new List<GameObject>();

        private void Start()
        {
            foreach (GameObject bootObject in _bootObjects)
            {
                Instantiate(bootObject);
            }
        }
    }
}

