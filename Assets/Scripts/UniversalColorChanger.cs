using FishNet.Object;
using UnityEngine;

public class UniversalColorChanger : NetworkBehaviour
{
    private Renderer _renderer;

    // 객체가 네트워크에 등장할 때 렌더러 컴포넌트를 미리 찾아둡니다.
    public override void OnStartClient()
    {
        base.OnStartClient();
        _renderer = GetComponent<Renderer>();
    }

    // 이 함수를 UI 버튼의 OnClick() 이벤트에 연결하면 됩니다.
    public void ChangeColorOnClick()
    {
        // 서버에게 색상을 변경해달라고 요청합니다.
        // 클라이언트가 이 함수를 호출하면, FishNet이 자동으로 서버에 있는 CmdChangeColor 함수를 실행시켜 줍니다.
        CmdChangeColor();
    }

    // [ServerRpc]는 클라이언트가 서버에 있는 함수를 호출할 수 있게 합니다.
    // RequireOwnership = false: 이 객체의 소유자가 아니어도 이 함수를 호출할 수 있다는 의미입니다. (중요!)
    [ServerRpc(RequireOwnership = false)]
    private void CmdChangeColor()
    {
        // 이 코드는 서버에서만 실행됩니다.
        Color newColor = new Color(Random.value, Random.value, Random.value);

        // 서버가 모든 클라이언트에게 RpcChangeColor 함수를 실행하라고 방송합니다.
        RpcChangeColor(newColor);
    }

    // [ObserversRpc]는 서버가 모든 클라이언트의 함수를 호출할 수 있게 합니다.
    [ObserversRpc]
    private void RpcChangeColor(Color newColor)
    {
        // 이 코드는 모든 클라이언트(서버 포함)에서 실행됩니다.
        if (_renderer != null)
        {
            _renderer.material.color = newColor;
        }
    }
}