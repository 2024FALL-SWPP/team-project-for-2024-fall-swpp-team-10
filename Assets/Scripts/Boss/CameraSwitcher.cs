using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera[] subCameras; // 3�� �Ҵ�

    private int currentCamIndex = -1;
    // currentCamIndex = -1�̸� ���� ī�޶�, 0/1/2�̸� �ش� Sub Camera

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
            // ���� ��ȣ�� �� �������Ƿ� ���� ī�޶�� ����
            EnableMainCamera();
        }
        else
        {
            // �ٸ� ī�޶�� ��ȯ
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
