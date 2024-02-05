using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Paddle paddle;

    void OnEnable()
    {
        // ó�� �Ѿ��� ��ġ�� ���� �ش�.
        transform.position = new Vector2(paddle.paddleX + (CompareTag("Odd") ? -0.904f : 0.904f), -2.867949f);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 0.05f);
        Invoke("ActiveFalse", 2); // delay�� �༭ �Լ��� �����Ŵ
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.name)
        {
            case "Block":
            case "HardBlock0":
            case "HardBlock1":
            case "HardBlock2":
                GameObject Col = col.gameObject;
                paddle.BlockBreak(Col, Col.transform, Col.GetComponent<Animator>());
                ActiveFalse();
                break;
            case "Background":
                ActiveFalse();
                break;
        }
    }

    void ActiveFalse() { gameObject.SetActive(false); }

}
