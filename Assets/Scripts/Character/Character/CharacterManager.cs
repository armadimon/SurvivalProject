using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // 싱글톤 인스턴스 변수
    static CharacterManager _instance;

    // 싱글톤 인스턴스 접근 프로퍼티
    public static CharacterManager Instance
    {
        get
        {
            // 인스턴스가 없으면 새로운 GameObject를 생성하여 추가
            if (_instance == null)
            {
                _instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();
            }
            return _instance;
        }
    }

    // 플레이어 객체를 저장할 변수
    private Player _player;

    // 플레이어 객체 접근 프로퍼티
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    public void Awake()
    {
        // 이미 존재하는 인스턴스가 있으면 현재 인스턴스를 할당하고 씬 전환 시 유지
        if (_instance != null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 중복된 인스턴스가 존재하면 삭제
        else if (_player != this)
        {
            Destroy(gameObject);
        }
    }
}
