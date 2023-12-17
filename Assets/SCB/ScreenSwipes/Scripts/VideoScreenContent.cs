using UnityEngine;
using UnityEngine.Video;

namespace SCB.ScreenSwipes
{
    public class VideoScreenContent : ScreenContent
    {
        private VideoPlayer videoPlayer;

        private void Start()
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>();
            videoPlayer.Prepare();
        }

        public override void OnScreenEntered()
        {
            base.OnScreenEntered();
            videoPlayer.Play();
        }

        public override void OnScreenExited()
        {
            base.OnScreenExited();
            videoPlayer.Pause();
        }
    }
}
