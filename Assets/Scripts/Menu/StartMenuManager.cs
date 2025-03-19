using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{

    public Button startButton;
    public Button exitButton;
    private void Start()
    {
        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExitGame);
    }
    public void OnStartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnExitGame()
    {
        Debug.Log("게임 종료"); // Unity 에디터에서는 로그 출력
        Application.Quit(); // 게임 종료
    }
}
