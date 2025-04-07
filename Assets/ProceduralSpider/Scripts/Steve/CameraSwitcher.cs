using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ProceduralSpider;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook groundCamera;      // while spider is on the ground
    [SerializeField] private CameraThirdPerson wallCamera;        // while spider is wall walking
    [SerializeField] private CinemachineFreeLook flightCamera;      // while spider is flying


    private void OnEnable()
    {
        SpiderEvents.OnSpiderStateChanged += OnSpiderStateChanged;
    }

    private void OnDisable()
    {
        SpiderEvents.OnSpiderStateChanged -= OnSpiderStateChanged;
    }

    private void Start()
    {
        ActivateStateCamera(Spider.SpiderState.Unknown); // Set default camera to ground camera
    }

    private void OnSpiderStateChanged(Spider.SpiderState oldState, Spider.SpiderState newState)
    {
        ActivateStateCamera(newState);
    }

    private void ActivateStateCamera(Spider.SpiderState state)
    {
        switch (state)
        {
            case Spider.SpiderState.OnGround:
            default:
                groundCamera.gameObject.SetActive(true);
                wallCamera.gameObject.SetActive(false);
                flightCamera.gameObject.SetActive(false);
                break;
            case Spider.SpiderState.OnWall:
                groundCamera.gameObject.SetActive(false);
                wallCamera.gameObject.SetActive(true);
                flightCamera.gameObject.SetActive(false);
                break;
            case Spider.SpiderState.Flying:
                groundCamera.gameObject.SetActive(false);
                wallCamera.gameObject.SetActive(false);
                flightCamera.gameObject.SetActive(true);
                break;
        }
    }
}

