using FishNet.Object;
using FishNet.Connection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

// 이 스크립트는 동기화할 객체에 NetworkObject와 함께 추가되어야 합니다.
[RequireComponent(typeof(NetworkObject))]
public class NetworkInteractable : NetworkBehaviour
{
    private ObjectManipulator objectManipulator;
    private Interactable interactable;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 이 객체에 있는 MRTK 컴포넌트들을 찾습니다.
        objectManipulator = GetComponent<ObjectManipulator>();
        interactable = GetComponent<Interactable>();

        // MRTK 이벤트에 소유권 요청 함수를 연결합니다.
        if (objectManipulator != null)
        {
            objectManipulator.OnManipulationStarted.AddListener(HandleInteractionStarted);
        }
        if (interactable != null)
        {
            interactable.OnClick.AddListener(HandleInteractionStarted);
        }

        // 소유권에 따라 물리 및 조작 가능 상태를 업데이트합니다.
        UpdateAuthority();
    }

    // 소유권이 변경될 때마다 호출됩니다.
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        UpdateAuthority();
    }

    // 사용자가 객체와 상호작용을 시작하면 (직접 잡거나, 레이로 클릭) 호출됩니다.
    private void HandleInteractionStarted()
    {
        // 내가 소유자가 아니라면, 서버에 소유권을 요청합니다.
        if (!base.IsOwner)
        {
            RequestOwnershipServerRpc();
        }
    }
    
    // 위 함수는 ManipulationEventData가 있는 버전도 필요합니다. (ObjectManipulator용)
    private void HandleInteractionStarted(ManipulationEventData eventData)
    {
        HandleInteractionStarted();
    }


    // [클라이언트 -> 서버] 소유권을 요청합니다.
    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(NetworkConnection sender = null) // << 수정된 부분
    {
        // 이 RPC를 호출한 클라이언트에게 소유권을 넘겨줍니다.
        base.GiveOwnership(sender); // << 수정된 부분
    }

    void Update()
    {
        // 소유자만이 객체의 위치/회전 정보를 서버로 보낼 수 있습니다.
        if (base.IsOwner)
        {
            SyncTransformServerRpc(transform.position, transform.rotation);
        }
    }

    // [소유자 -> 서버] Transform 정보를 동기화합니다.
    [ServerRpc]
    private void SyncTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        // 서버는 받은 정보를 다른 모든 클라이언트에게 전파합니다.
        BroadcastTransformObserversRpc(position, rotation);
    }

    // [서버 -> 모든 클라이언트] Transform 정보를 갱신합니다.
    [ObserversRpc(BufferLast = true)]
    private void BroadcastTransformObserversRpc(Vector3 position, Quaternion rotation)
    {
        // 소유자는 직접 조작 중이므로, 네트워크로 받은 정보로 덮어쓰지 않습니다.
        if (!base.IsOwner)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    // 소유권 상태에 따라 MRTK 컴포넌트의 활성화/비활성화를 제어합니다.
    private void UpdateAuthority()
    {
        // Rigidbody가 있다면, 소유자가 아닐 때 물리적 영향을 받지 않도록 isKinematic으로 만듭니다.
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = !base.IsOwner;
        }
    }
}