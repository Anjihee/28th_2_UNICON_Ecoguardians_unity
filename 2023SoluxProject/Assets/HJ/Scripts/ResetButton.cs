using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
   public void Retry()
    {
        // ���� ������ �����ϱ� ���� �ִ� �õ� Ƚ��
        const int maxTries = 1000;
        int currentTry = 0;

        do
        {
            // �ʱ� ���� ����
            Board.Instance.CreateInitialBoard();

            // �� �� �̻��� ���ӵ� ������ ������ Ȯ��
            if (!Board.Instance.HasConsecutiveBubbles())
            {
                // ������ ������ ������ �ٽ� �õ�
                currentTry++;
            }
            else
            {
                // ������ �����ϸ� ���� ����
                break;
            }
        } while (currentTry < maxTries);

        //���ھ� �ʱ�ȭ 
        Score.Instance.ResetScore();

    }
}
