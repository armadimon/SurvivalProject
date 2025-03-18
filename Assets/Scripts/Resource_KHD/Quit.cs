using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("게임 종료"); // Unity 에디터에서는 로그 출력
        Application.Quit(); // 게임 종료
    }
}
