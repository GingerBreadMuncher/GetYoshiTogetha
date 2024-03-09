using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        gameObject.SetActive(false);
    }
}
