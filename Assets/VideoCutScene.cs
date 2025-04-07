using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]

public class VideoCutSceneHandler : MonoBehaviour
{
    public GameObject playerController;  // to disable player controller during cutscene
    private VideoPlayer videoPlayer; 

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>(); // Get the VideoPlayer component attached to this GameObject
        videoPlayer.playOnAwake = false; // Disable play on awake to control when the video starts
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoFinished; // Subscribe to the loop point reached event
        SpiderEvents.OnPlayCutScene += PlayCutScene; // Subscribe to the event to play the cutscene
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished; // Unsubscribe from the event
        SpiderEvents.OnPlayCutScene -= PlayCutScene; // Unsubscribe from the event
    }


    private void PlayCutScene()
    {
        playerController.SetActive(false); // Disable the player controller during cutscene
        videoPlayer.Play(); // Start playing the video
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        playerController.SetActive(true); // Enable the player controller after the video finishes
        gameObject.SetActive(false); // Disable this cutscene object
    }
}
