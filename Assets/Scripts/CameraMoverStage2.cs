using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoverStage2 : MonoBehaviour
{
    public float speed = 1f; // �̵� �ӵ� ����

    private void Start()
    {
        // ���� ��ġ ����
        transform.position = new Vector3(0, 7, 43);
    }

    private void Update()
    {
        // x�� ���� �������� ī�޶� ������ �̵�
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
