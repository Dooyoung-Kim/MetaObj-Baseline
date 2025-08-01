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
            Debug.LogError("NetworkManager�� ã�� �� �����ϴ�. �� ��ũ��Ʈ�� NetworkManager�� �ִ� ������Ʈ�� �־�� �մϴ�.");
        }
    }

    public void StartServerOnClick()
    {
        if (_networkManager.ServerManager.StartConnection())
        {
            Debug.Log("������ ���������� ���۵Ǿ����ϴ�.");
            // _networkManager.ServerManager.AddPlayer(...);  <- �� ������ ������ �����մϴ�.
        }
        else
        {
            Debug.LogError("���� ���ۿ� �����߽��ϴ�.");
        }
    }

    public void StartClientOnClick()
    {
        if (_networkManager.ClientManager.StartConnection())
        {
            Debug.Log("Ŭ���̾�Ʈ�� ���������� ������ �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("Ŭ���̾�Ʈ ���� ���ۿ� �����߽��ϴ�.");
        }
    }
}