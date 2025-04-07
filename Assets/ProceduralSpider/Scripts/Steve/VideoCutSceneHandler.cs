using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoCutScene : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public GameObject playerController;  // to disable player controller during cutscene


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished; // Subscribe to the loop point reached event
    }

    private void OggerEnter(Collider other)
    {
        
        
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
