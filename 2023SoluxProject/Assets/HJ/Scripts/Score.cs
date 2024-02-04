/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }

    private int _score;

    public int Score_
    {
        get => _score;

        set
        {
            if (_score == value) return;

            _score = value;

            scoreText.SetText($"{_score} / 50");
                
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => Instance = this;
}*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }

    private Dictionary<Item, int> colorScores = new Dictionary<Item, int>(); // �� ������ ������ ������ ��ųʸ�

    public Button btn;

    public int RedScore => GetColorScore(ItemDatabase.Items[3]); // ������ ����
    public int YellowScore => GetColorScore(ItemDatabase.Items[4]); // ����� ����
    public int GreenScore => GetColorScore(ItemDatabase.Items[1]); // �ʷϻ� ����
    public int BlueScore => GetColorScore(ItemDatabase.Items[0]); // �Ķ��� ����
    public int PurpleScore => GetColorScore(ItemDatabase.Items[2]); // ����� ����

    [SerializeField] public TextMeshProUGUI redScoreText;
    [SerializeField] public TextMeshProUGUI yellowScoreText; 
    [SerializeField] public TextMeshProUGUI greenScoreText; 
    [SerializeField] public TextMeshProUGUI blueScoreText; 
    [SerializeField] public TextMeshProUGUI purpleScoreText;

    private void Awake() {
        Instance = this;
    } 

    private void Start()
    {
        // �ʱ�ȭ: �� ������ ������ 0���� ����
        ResetScore();
        //��ư�Ⱥ��̰� 
        btn.gameObject.SetActive(false);
    }
    public void ResetScore()
    {
        foreach (var item in ItemDatabase.Items)
        {
            colorScores[item] = 0;
        }
        UpdateUI();
    }

    public void Update()
    {
        isClear();

    }

    public void AddScore(Item color, int score)
    {
        // ���� �� ���� �߰�
        colorScores[color] += score;

        // UI ����
        UpdateUI();
    }

    

    private int GetColorScore(Item color)
    {
        // ���� �� ���� ��ȯ
        return colorScores[color];
    }

    private void UpdateUI()
    {
        // UI ����: �� ������ ������ ǥ��
        redScoreText.SetText($"{RedScore} / 10");
        yellowScoreText.SetText($"{YellowScore} / 10");
        greenScoreText.SetText($"{GreenScore} / 10");
        blueScoreText.SetText($"{BlueScore} / 10");
        purpleScoreText.SetText($"{PurpleScore} / 10");
    }

    private void isClear()
    {
        if (RedScore >= 10 && YellowScore >= 10 && GreenScore >= 10 && BlueScore >= 10 && PurpleScore >= 10)
        {
            //����!! �������� �Ѿ�� ��ư
            btn.gameObject.SetActive(true);
        }
    }



}
