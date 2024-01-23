using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeer : MonoBehaviour
{
    public float jump1 = 1f;
    public float jump2 = 3f;
    int jumpCount = 0;
    public int speed = 2;

    public void Jump()
    {
        //ó�� ������ �� 
        if (jumpCount == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, jump1, 0);
            jumpCount += 1;
        }
        else if (jumpCount == 1) {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, jump2, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.CompareTo("Plane") == 0)
        {
            jumpCount = 0;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

        if (Input.GetMouseButtonDown(0))
        {
            Jump();
        }

    }
}
