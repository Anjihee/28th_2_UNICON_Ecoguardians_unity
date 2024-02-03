// ��ĥ �� ���

// �ڲ� ���� ���� �ִϸ��̼� ����


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
    public GameObject Life2;
    public GameObject WinPanel;
    public GameObject GameOverPanel;
    public GameObject ComboPanel; //�޺� �߰�
    public GameObject[] Combocol; //�޺� �߰�
    public int comboBlockIndex = -1; // �޺� �ε���
    public GameObject PausePanel;
    public GameObject yj_EndingPanel; // �����г�
    public AudioSource S_Break;
    public AudioSource S_Eat;
    public AudioSource S_Fail;
    public AudioSource S_Gun;
    public AudioSource S_HardBreak;
    public AudioSource S_Paddle;
    public AudioSource S_Victory;
    public AudioSource S_Combo;// �޺� �Ҹ�
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
        Debug.Log("_stage : " +_stage + "/ stage : " + stage);
        if (_stage == 0) stage++;
        else if(_stage != -1) stage = _stage;
        

        // ���������� 4�� �Ǹ� ���� ���� �г� ��
        if (stage >= (StageStr.Length)) {
            if (!yj_EndingPanel.activeSelf)
            {
                yj_EndingPanel.SetActive(true);
                
            }
            
            return;
        }

        Clear();
        BlockGenerator();
        StartCoroutine("BallReset");

        // ���� �ʱ�ȭ
        StageText.text = stage.ToString();
        /*score = 0;
        ScoreText.text = "0";*/

        PaddleSr.enabled = true;
        Life0.SetActive(true);
        Life1.SetActive(true);
        Life2.SetActive(true);
        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        

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

    // �� ��ġ �ʱ�ȭ�ϰ� 0.7�ʰ� �����̴� �ִϸ��̼� ���
    IEnumerator BallReset()
    {
        isStart = false;
        combo = 0;
        Ball[0].SetActive(true);
        Ball[1].SetActive(false);
        Ball[2].SetActive(false);
        // Ball[3].SetActive(false);
        BallAni[0].SetTrigger("Blink");
        BallTr[0].position = new Vector2(paddleX, -2.87f);

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
    
    // ���� �浹�� ��
    public IEnumerator BallCollisionEnter2D(Transform ThisBallTr, Rigidbody2D ThisBallRg, Ball ThisBallCs, GameObject Col, Transform ColTr, SpriteRenderer ColSr, Animator ColAni)
    {
        Physics2D.IgnoreLayerCollision(2, 2);
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

            // �ڼ��е鿡 �ε����� ���� �ڼ��� �پ�����
            /*
            case "MagnetPaddle":
                ThisBallCs.isMagnet = true;
                ThisBallRg.velocity = Vector2.zero;
                float gapX = transform.position.x - ThisBallTr.position.x;
                while (ThisBallCs.isMagnet)
                {
                    if (Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
                        ThisBallTr.position = new Vector2(transform.position.x + gapX, ThisBallTr.position.y);

                    if (gameObject.name == "Paddle" || (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
                    {
                        ThisBallRg.velocity = Vector2.zero;
                        ThisBallRg.AddForce((ThisBallTr.position - transform.position).normalized * ballSpeed);
                        ThisBallCs.isMagnet = false;
                    }
                    yield return new WaitForSeconds(0.01f);
                }
                break;
            */

            case "DeathZone":
                ThisBallTr.gameObject.SetActive(false);
                BallCheck(); // ��üũ
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

    // �е��� �����۰� �浹�� ��
    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
        S_Eat.Play();
        switch (col.name)
        {
            // �� 3�� ���� Ȱ��ȭ
            case "Item_TripleBall":
                GameObject OneBall = BallCheck();
                for (int i = 0; i < 3; i++)
                {
                    if (OneBall.name == Ball[i].name) continue;
                    BallTr[i].position = OneBall.transform.position;
                    Ball[i].SetActive(true);
                    BallRg[i].velocity = Vector2.zero;
                    BallRg[i].AddForce(Random.insideUnitCircle.normalized * ballSpeed);
                }
                break;

            // 7.5�ʵ��� �е��� Ŀ��
            case "Item_Big":
                paddleSize = 2.42f;
                paddleBorder = 9.5f; // �ٲ���ߵ�
                StopCoroutine("Item_BigOrSmall");
                StartCoroutine("Item_BigOrSmall", false);
                break;

            // 7.5�ʵ��� �е��� �۾���
            case "Item_Small":
                paddleSize = 0.82f;
                paddleBorder = 10f;  // �ٲ���ߵ�
                StopCoroutine("Item_BigOrSmall");
                StartCoroutine("Item_BigOrSmall", false);
                break;

        /*
            // 7.5�ʵ��� ���� �ӵ��� ������(�����̶� �������) 
            case "Item_SlowBall":
                StopCoroutine("Item_SlowBall");
                StartCoroutine("Item_SlowBall", false);
                break;

            // 4�ʵ��� �Ұ��� ��(�����̶� �������) 
            case "Item_FireBall":
                StopCoroutine("Item_FireBall");
                StartCoroutine("Item_FireBall", false);
                break;

            // 7.5�ʵ��� �ڼ� Ȱ��ȭ
            case "Item_Magnet":
                StopCoroutine("Item_Magnet");
                StartCoroutine("Item_Magnet", false);
                break;
            */

            // 4�ʵ��� 24���� �Ѿ��� �߻���
            case "Item_Gun":
                StopCoroutine("Item_Gun");
                StartCoroutine("Item_Gun", false);
                break;
        }
    }

    IEnumerator Item_BigOrSmall(bool skip)
    {
        if (!skip)
        {
            PaddleSr.size = new Vector2(paddleSize, PaddleSr.size.y);
            PaddleCol.size = new Vector2(paddleSize, PaddleCol.size.y);
            yield return new WaitForSeconds(7.5f);
        }
        paddleSize = 1.58f;
        paddleBorder = 9.5f;
        PaddleSr.size = new Vector2(paddleSize, PaddleSr.size.y);
        PaddleCol.size = new Vector2(paddleSize, PaddleCol.size.y);
    }

    /*
    IEnumerator Item_SlowBall(bool skip) // �� �����̶� �ȳ��͵� �ɼ����� 
    {
        if (!skip)
        {
            for (int i = 0; i < 3; i++)
            {
                ballSpeed = 250;
                BallAddForce(BallRg[i]);
            }
            yield return new WaitForSeconds(7.5f);
        }
        for (int i = 0; i < 3; i++)
        {
            ballSpeed = oldBallSpeed;
            BallAddForce(BallRg[i]);
        }
    }
    
    //�Ұ� ������ ���� �ٵ� �����̶� ������� ���� 
    IEnumerator Item_FireBall(bool skip)
    {
        if (!skip)
        {
            for (int i = 0; i < 3; i++)
            {
                BallSr[i].sprite = B[23];
                ParticleSystem.MainModule PS = BallTr[i].GetChild(0).GetComponent<ParticleSystem>().main;
                PS.startColor = Color.red;
            }
            for (int i = 0; i < BlockCol.Length; i++)
            {
                BlockCol[i].tag = "TriggerBlock";
                BlockCol[i].isTrigger = true;
            }
            yield return new WaitForSeconds(4);
        }
        for (int i = 0; i < 3; i++)
        {
            BallSr[i].sprite = B[22];
            ParticleSystem.MainModule PS = BallTr[i].GetChild(0).GetComponent<ParticleSystem>().main;
            PS.startColor = Color.white;
        }
        for (int i = 0; i < BlockCol.Length; i++)
        {
            BlockCol[i].tag = "Untagged";
            BlockCol[i].isTrigger = false;
        }
    }

    // �ڼ� ������ ����
    IEnumerator Item_Magnet(bool skip)
    {
        if (!skip)
        {
            gameObject.name = "MagnetPaddle";
            Magnet.SetActive(true);
            yield return new WaitForSeconds(5.5f);
            Magnet.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Magnet.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            Magnet.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            Magnet.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            Magnet.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            Magnet.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
        gameObject.name = "Paddle";
        Magnet.SetActive(false);
    }
    */

    IEnumerator Item_Gun(bool skip)
    {
        if (!skip)
        {
            Gun.SetActive(true);
            for (int i = 0; i < 12; i++)
            {
                Bullet[i * 2].SetActive(true);
                Bullet[i * 2 + 1].SetActive(true);
                S_Gun.Play();
                yield return new WaitForSeconds(0.34f);
            }
        }
        Gun.SetActive(false);
    }

    public void BlockBreak(GameObject Col, Transform ColTr, Animator ColAni)
    {
        // ������ ����
        ItemGenerator(ColTr.position);

        // �߰��� ��
        comboBlockIndex = -1;
        for (int i = 0; i < BlocksTr.childCount; i++)
        { 
            if(BlocksTr.GetChild(i).gameObject == Col)
            {
                comboBlockIndex = i;
            }        
        }

       // ���ھ� ����, �޺��� 1��, 3�޺��̻��� 3��
        score += (++combo > 3) ? 3 : combo;
        ScoreText.text = score.ToString();

        // ���� �μ����� �ִϸ��̼�
        ColAni.SetTrigger("Break");
        S_Break.Play();
        StartCoroutine(ActiveFalse(Col));

        // �� üũ
        StopCoroutine("BlockCheck");
        StartCoroutine("BlockCheck",combo);
    }

    // %�� Ȯ���� �������� ����(�������� �� ���� ���� �ִ�)
    void ItemGenerator(Vector2 ColTr)
    {
        int rand = Random.Range(0, 10000);
        if (rand < 1200)
        {
            string currentName = "";

            // ���� �ڵ�
            switch (rand % 4)
            {
                case 0: currentName = "Item_TripleBall"; break; // ���� 3�� ��
                case 1: currentName = "Item_Big"; break;
                case 2: currentName = "Item_Small"; break;
                case 3: currentName = "Item_Gun"; break;
            }
            P_ItemSr.sprite = B[rand % 4 + 11];
            GameObject Item = Instantiate(P_Item, ColTr, Quaternion.identity);
            Item.name = currentName;
            Item.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 0.008f);
            Item.transform.SetParent(ItemsTr);
            Destroy(Item, 4);
        }
    }
    

    IEnumerator ActiveFalse(GameObject Col)
    {
        yield return new WaitForSeconds(0.2f);
        Col.SetActive(false);
    }

    GameObject BallCheck() // �״°� 3���ϱ� ��� �� �ؾߵ�... 
    {
        int ballCount = 0;
        GameObject ReturnBall = null;
        foreach (GameObject OneBall in GameObject.FindGameObjectsWithTag("Ball"))
        {
            ballCount++;
            ReturnBall = OneBall;
        }

        // ���� �ϳ��� ���� �� ������ ����
        if (ballCount == 0)
        {
            if (Life2.activeSelf)
            {
                Life2.SetActive(false);
                StartCoroutine("BallReset");  // ��� �ϳ� �����Ŵϱ� ��ü�����ϸ� �ȵȴ�
                S_Fail.Play();  // �׾��� �� ȿ����
            }
            else if (Life1.activeSelf)
            {
                Life1.SetActive(false);
                StartCoroutine("BallReset");
                S_Fail.Play();
            }
            else
            {
                Life0.SetActive(false);
                GameOverPanel.SetActive(true);
                S_Fail.Play();
                Clear();
            }
        }
        return ReturnBall;
    }

    // ball�� ���� �� 
    public void BallAddForce(Rigidbody2D ThisBallRg)
    {
        Vector2 dir = ThisBallRg.velocity.normalized;
        ThisBallRg.velocity = Vector2.zero;
        ThisBallRg.AddForce(dir * ballSpeed);
    }

    // �� üũ
    IEnumerator BlockCheck(int combo)
    {
        yield return new WaitForSeconds(0.5f);
        int blockCount = 0;

        if(comboBlockIndex != -1 && BlocksTr.childCount > comboBlockIndex && BlocksTr.GetChild(comboBlockIndex).gameObject.activeSelf)
        {
            blockCount++;

            if (combo > 1)
            {
                // Activate only the Combocol corresponding to the block where the combo occurred
                Combocol[comboBlockIndex].gameObject.SetActive(true);
                S_Combo.Play();
                StartCoroutine(DisableComboPanel(Combocol[comboBlockIndex].gameObject));
            }
        }

        // Check if all blocks are destroyed
        bool allBlocksDestroyed = true;
        for (int i = 0; i < BlocksTr.childCount; i++)
        {
            if (BlocksTr.GetChild(i).gameObject.activeSelf)
            {
                allBlocksDestroyed = false;
                break;
            }
        }

        // �¸�
        if (allBlocksDestroyed && blockCount == 0)
        {
            WinPanel.SetActive(true);
            S_Victory.Play();
            Clear();
        }
        else { 
        // ���� ������ �긲
            ItemGenerator(new Vector2(Random.Range(-9f, 9f), 4f));
            if (combo > 1)
            {
                Combocol[comboBlockIndex].gameObject.SetActive(true);
                S_Combo.Play();
                StartCoroutine(DisableComboPanel(Combocol[comboBlockIndex].gameObject));
            }
        }
    }

    IEnumerator DisableComboPanel(GameObject comboPanel)
    {
        yield return new WaitForSeconds(0.5f);
        comboPanel.SetActive(false);
    }

    // �¸� �Ǵ� ���ӿ����� ȣ��� 
    void Clear()
    {
        StopAllCoroutines();
        StartCoroutine("Item_BigOrSmall", true);
       // StartCoroutine("Item_SlowBall", true);
       // StartCoroutine("Item_FireBall", true);
       // StartCoroutine("Item_Magnet", true);
        StartCoroutine("Item_Gun", true);

        // ���� �ʱ�ȭ
       /* StageText.text = stage.ToString();
        score = 0;
        ScoreText.text = "0";
       */

        for (int i = 0; i < 3; i++) Ball[i].SetActive(false);
        PaddleSr.enabled = false;
    }

    
}
