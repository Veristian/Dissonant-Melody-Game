using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject followCam;
    public GameObject levelCam;
    public CinemachineTargetGroup targetGroup;
    private bool wasFollowingPlayer;
    private CinemachineVirtualCamera followCamSetting;

    private GameObject playerToFollow;
    private void Awake()
    {
        followCamSetting = followCam.GetComponent<CinemachineVirtualCamera>();
        ChangeCamera();
    }

    private void LateUpdate()
    {
        if (InputManager.SwitchCameraPressed)
        {
            wasFollowingPlayer = !wasFollowingPlayer;
            ChangeCamera();
        }
        UpdateCameraLook();
    }
    private void ChangeCamera()
    {
        if (wasFollowingPlayer)
        {
            followCam.SetActive(false);
            levelCam.SetActive(true);
        }
        else
        {
            levelCam.SetActive(false);
            followCam.SetActive(true);

        }
    }

    private void UpdateCameraLook()
    {
        //update virtual cam
        if (followCamSetting.Follow == null)
        {
            if (playerToFollow != null)
            {
                followCamSetting.Follow = playerToFollow.transform;
            }
            else
            {
                playerToFollow = FindObjectOfType<PlayerMovement>().gameObject;
                followCamSetting.Follow = playerToFollow.transform;

            }

        }

        //update target group
        targetGroup.m_Targets[0].target = LevelManager.Instance.currentCheckpoint.gameObject.transform;
        if (LevelManager.Instance.nextCheckpoint == null)
        {
            if (playerToFollow != null)
            {
                targetGroup.m_Targets[1].target = playerToFollow.transform;
            }
            else
            {
                playerToFollow = FindObjectOfType<PlayerMovement>().gameObject;
                targetGroup.m_Targets[1].target = playerToFollow.transform;
            }            

            
        }
        else
        {
            targetGroup.m_Targets[1].target = LevelManager.Instance.nextCheckpoint.gameObject.transform;
        }
    }



}
