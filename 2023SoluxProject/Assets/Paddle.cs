using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Paddle : MonoBehaviour
{
    [Multiline(12)]
    public string[] StageStr;
    public Sprite[] B;
    public GameObject P_Item;
    public SpriteRenderer P_ItemSr;
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI ScoreText;
    public GameObject Life0;
    public GameObject Life1;
    public GameObject WinPanel;
    public GameObject GameOverPanel;
    public GameObject PausePanel;
    public AudioSource S_Break;
    public AudioSource S_Eat;
    public AudioSource S_Fail;
    public AudioSource S_Gun;
    public AudioSource S_HardBreak;
    public AudioSource S_Paddle;
    public AudioSource S_Victory;
    public Transform ItemsTr;
    public Transform BlocksTr;
    public BoxCollider2D[] BlockCol;
    public GameObject[] Ball;
    public Animator[] BallAni;
    public Transform[] BallTr;
    public SpriteRenderer[] BallSr;
    public Rigidbody2D[] BallRg;
    public GameObject[] Bullet;
    public SpriteRenderer PaddleSr;
    public BoxCollider2D PaddleCol;
    public GameObject Magnet;
    public GameObject Gun;

    bool isStart;
    public float paddleX;
    public float ballSpeed;
    float oldBallSpeed = 300;
    float paddleBorder = 9.5f;    //�ٲ���ߵ�
    float paddleSize = 1.58f;
    int combo;
    int score;
    int stage;

#if (UNITY_ANDROID)
        void Awake() { Screen.SetResolution(1170, 540, false); }
#else
    void Awake() { Screen.SetResolution(2340, 1080, false); }
#endif
    // �ڷΰ��� Ű ������ �Ͻ�����
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausePanel.activeSelf) { PausePanel.SetActive(false); Time.timeScale = 1; }
            else { PausePanel.SetActive(true); Time.timeScale = 0; }
        }
    }

    // �������� �ʱ�ȭ (-1 �����, 0 ���� ��������, ���� ��������)
    public void AllReset(int _stage)
    {
        if (_stage == 0) stage++;
        else if(_stage != -1) stage = _stage;
        if (stage >= StageStr.Length) return;


        BlockGenerator();
        StartCoroutine("BallReset");
    }

    // �� ����
    // �ٵ� Ŀ������ �ʿ� ���� �� ���Ƽ� ���߿� ������� �̶�.. 
    void BlockGenerator()
    {
        string currentStr = StageStr[stage].Replace("\n", ""); // ���� ���������� �ҷ���
        currentStr = currentStr.Replace(" ", ""); // ���Ⱑ ������ ó����
        for (int i = 0; i < currentStr.Length; i++)
        {
            BlockCol[i].gameObject.SetActive(false); // ����� ������� �ҷ��� 
            char A = currentStr[i]; string currentName = "Block"; int currentB = 0;

            if (A == '*') continue;
            else if (A == '8') { currentB = 8; currentName = "HardBlock0"; }
            else if (A == '9') currentB = Random.Range(0, 8);
            else currentB = int.Parse(A.ToString());

            BlockCol[i].gameObject.name = currentName;
            BlockCol[i].gameObject.GetComponent<SpriteRenderer>().sprite = B[currentB];
            BlockCol[i].gameObject.SetActive(true);
        }
    }

    IEnumerator BallReset()
    {
        BallAni[0].SetTrigger("Blink");

        StopCoroutine("InfinityLoop");
        yield return new WaitForSeconds(0.7f);
        StartCoroutine("InfinityLoop");
    }

    IEnumerator InfinityLoop()
    {
        while (true)
        {
            if(Input.GetMouseButton(0) || (Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                paddleX = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position).x, -paddleBorder, paddleBorder);
                transform.position = new Vector2(paddleX, transform.position.y);
                if(!isStart) BallTr[0].position = new Vector2(paddleX, BallTr[0].position.y);
            }

            if(!isStart && (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            {
                isStart = true;
                ballSpeed = oldBallSpeed;
                BallRg[0].AddForce(new Vector2(0.1f, 0.9f).normalized*ballSpeed);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator BallCollisionEnter2D(Transform ThisBallTr, Rigidbody2D ThisBallRg, Ball ThisBallCs, GameObject Col, Transform ColTr, SpriteRenderer ColSr, Animator ColAni)
    {
        if (!isStart) yield break;

        switch (Col.name)
        {
            // �е鿡 �ε����� ���̰���ŭ �� ��
            case "Paddle":
                ThisBallRg.velocity = Vector2.zero;
                ThisBallRg.AddForce((ThisBallTr.position - transform.position).normalized * ballSpeed);
                S_Paddle.Play();
                combo = 0;
                break;

           case "DeathZone":
                ThisBallTr.gameObject.SetActive(false);
                //BallCheck(); // ��üũ
                break;

            // ��0�� �ε����� ��1�� ��
            case "HardBlock0":
                Col.name = "HardBlock1";
                ColSr.sprite = B[9];
                S_HardBreak.Play();
                break;

            // ��1�� �ε����� ��2�� ��
            case "HardBlock1":
                Col.name = "HardBlock2";
                ColSr.sprite = B[10];
                S_HardBreak.Play();
                break;

            // ���̳� ���� �ε����� �ν���
            case "HardBlock2":
            case "Block":
                BlockBreak(Col, ColTr, ColAni);
                break;
   
        }
    }

    void BlockBreak(GameObject Col, Transform ColTr, Animator ColAni)
    {
        // ������ ����
        // ���ھ� ����
        // ���� �μ����� �ִϸ��̼�
        ColAni.SetTrigger("Break");
        StartCoroutine(ActiveFalse(Col));

        // �� üũ
    }

    IEnumerator ActiveFalse(GameObject Col)
    {
        yield return new WaitForSeconds(0.2f);
        Col.SetActive(false);
    }
}
