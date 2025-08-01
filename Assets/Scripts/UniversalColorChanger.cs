using FishNet.Object;
using UnityEngine;

public class UniversalColorChanger : NetworkBehaviour
{
    private Renderer _renderer;

    // ��ü�� ��Ʈ��ũ�� ������ �� ������ ������Ʈ�� �̸� ã�ƵӴϴ�.
    public override void OnStartClient()
    {
        base.OnStartClient();
        _renderer = GetComponent<Renderer>();
    }

    // �� �Լ��� UI ��ư�� OnClick() �̺�Ʈ�� �����ϸ� �˴ϴ�.
    public void ChangeColorOnClick()
    {
        // �������� ������ �����ش޶�� ��û�մϴ�.
        // Ŭ���̾�Ʈ�� �� �Լ��� ȣ���ϸ�, FishNet�� �ڵ����� ������ �ִ� CmdChangeColor �Լ��� ������� �ݴϴ�.
        CmdChangeColor();
    }

    // [ServerRpc]�� Ŭ���̾�Ʈ�� ������ �ִ� �Լ��� ȣ���� �� �ְ� �մϴ�.
    // RequireOwnership = false: �� ��ü�� �����ڰ� �ƴϾ �� �Լ��� ȣ���� �� �ִٴ� �ǹ��Դϴ�. (�߿�!)
    [ServerRpc(RequireOwnership = false)]
    private void CmdChangeColor()
    {
        // �� �ڵ�� ���������� ����˴ϴ�.
        Color newColor = new Color(Random.value, Random.value, Random.value);

        // ������ ��� Ŭ���̾�Ʈ���� RpcChangeColor �Լ��� �����϶�� ����մϴ�.
        RpcChangeColor(newColor);
    }

    // [ObserversRpc]�� ������ ��� Ŭ���̾�Ʈ�� �Լ��� ȣ���� �� �ְ� �մϴ�.
    [ObserversRpc]
    private void RpcChangeColor(Color newColor)
    {
        // �� �ڵ�� ��� Ŭ���̾�Ʈ(���� ����)���� ����˴ϴ�.
        if (_renderer != null)
        {
            _renderer.material.color = newColor;
        }
    }
}