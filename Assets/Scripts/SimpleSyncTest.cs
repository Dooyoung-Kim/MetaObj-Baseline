using FishNet.Object;
using UnityEngine;

public class SimpleSyncTest : NetworkBehaviour
{
    // 이 함수를 PC에서 UI 버튼으로 호출할 것입니다.
    public void TestBroadcast()
    {
        // 서버에서만 이 함수가 실행되도록 합니다.
        if (base.IsServer)
        {
            Debug.Log("[Server] 'Change Color' 방송을 모든 클라이언트에게 보냅니다.");
            ChangeColorObserversRpc(Color.green);
        }
        else
        {
            Debug.LogWarning("[Client] 서버만 이 기능을 호출할 수 있습니다.");
        }
    }

    // [서버 -> 모든 클라이언트] 색상 변경 명령
    [ObserversRpc]
    private void ChangeColorObserversRpc(Color newColor)
    {
        // 이 로그는 PC와 홀로렌즈 양쪽 모두에서 보여야 합니다.
        Debug.Log($"[All Clients] 서버의 방송을 수신! 색상을 '{newColor}'로 변경합니다.");
        GetComponent<Renderer>().material.color = newColor;
    }
}