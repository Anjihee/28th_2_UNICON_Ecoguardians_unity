using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;  // �÷��̾��� Transform�� ������ ����
    public float smoothSpeed = 0.125f;  // ī�޶� �̵��� �ε巴�� �ϱ� ���� ������ ���

    void LateUpdate()
    {
        if (player != null)
        {
            // �÷��̾��� ���� X ��ġ�� ��������
            float targetX = player.position.x;

            // ���� ī�޶��� ��ġ�� �÷��̾��� X ��ġ�� �����Ͽ� �ε巴�� �̵�
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), smoothSpeed);

            // ī�޶� ��ġ�� ������Ʈ
            transform.position = smoothedPosition;
        }
    }
}
