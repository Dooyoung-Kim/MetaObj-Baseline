using FishNet.Managing;
using UnityEngine;

public class ServerConnectionManager : MonoBehaviour
{
    private NetworkManager _networkManager;

    void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogError("NetworkManager를 찾을 수 없습니다. 이 스크립트는 NetworkManager가 있는 오브젝트에 있어야 합니다.");
        }
    }

    public void StartServerOnClick()
    {
        if (_networkManager.ServerManager.StartConnection())
        {
            Debug.Log("서버가 성공적으로 시작되었습니다.");
            // _networkManager.ServerManager.AddPlayer(...);  <- 이 라인을 완전히 삭제합니다.
        }
        else
        {
            Debug.LogError("서버 시작에 실패했습니다.");
        }
    }

    public void StartClientOnClick()
    {
        if (_networkManager.ClientManager.StartConnection())
        {
            Debug.Log("클라이언트가 성공적으로 연결을 시작했습니다.");
        }
        else
        {
            Debug.LogError("클라이언트 연결 시작에 실패했습니다.");
        }
    }
}