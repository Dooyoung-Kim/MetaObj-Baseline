using FishNet.Object;
using UnityEngine;

public class SimpleSyncTest : NetworkBehaviour
{
    // �� �Լ��� PC���� UI ��ư���� ȣ���� ���Դϴ�.
    public void TestBroadcast()
    {
        // ���������� �� �Լ��� ����ǵ��� �մϴ�.
        if (base.IsServer)
        {
            Debug.Log("[Server] 'Change Color' ����� ��� Ŭ���̾�Ʈ���� �����ϴ�.");
            ChangeColorObserversRpc(Color.green);
        }
        else
        {
            Debug.LogWarning("[Client] ������ �� ����� ȣ���� �� �ֽ��ϴ�.");
        }
    }

    // [���� -> ��� Ŭ���̾�Ʈ] ���� ���� ���
    [ObserversRpc]
    private void ChangeColorObserversRpc(Color newColor)
    {
        // �� �α״� PC�� Ȧ�η��� ���� ��ο��� ������ �մϴ�.
        Debug.Log($"[All Clients] ������ ����� ����! ������ '{newColor}'�� �����մϴ�.");
        GetComponent<Renderer>().material.color = newColor;
    }
}