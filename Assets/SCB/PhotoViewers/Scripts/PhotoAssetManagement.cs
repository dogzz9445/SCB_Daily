using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCB.PhotoViewers;
using UnityEngine;

namespace SCB.Shared.Components
{
    public class PhotoAssetManagement : AbstractAssetManagementMonoBehaviour<Photo>
    {

        [SerializeField]
        public override string BasePath { get; set; } = "Assets/SCB/PhotoViewers/Photos";
        [SerializeField]
        public override string AssetName { get; set; } = "New Photo";
    }
}