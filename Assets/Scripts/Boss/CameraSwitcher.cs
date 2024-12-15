using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera[] subCameras; // 3개 할당

    private int currentCamIndex = -1;
    // currentCamIndex = -1이면 메인 카메라, 0/1/2이면 해당 Sub Camera

    void Start()
    {
        EnableMainCamera();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleCameraSwitch(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleCameraSwitch(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandleCameraSwitch(2);
        }
    }

    void HandleCameraSwitch(int targetIndex)
    {
        if (currentCamIndex == targetIndex)
        {
            // 같은 번호를 또 눌렀으므로 메인 카메라로 복귀
            EnableMainCamera();
        }
        else
        {
            // 다른 카메라로 전환
            EnableSubCamera(targetIndex);
        }
    }

    void EnableMainCamera()
    {
        mainCamera.enabled = true;
        for (int i = 0; i < subCameras.Length; i++)
        {
            subCameras[i].enabled = false;
        }
        currentCamIndex = -1;
    }

    void EnableSubCamera(int index)
    {
        mainCamera.enabled = false;
        for (int i = 0; i < subCameras.Length; i++)
        {
            subCameras[i].enabled = (i == index);
        }
        currentCamIndex = index;
    }
}
