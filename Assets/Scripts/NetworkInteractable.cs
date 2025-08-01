using FishNet.Object;
using FishNet.Connection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkInteractable : NetworkBehaviour
{
    private ObjectManipulator objectManipulator;
    private Interactable interactable;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[NetworkInteractable] {gameObject.name} started. IsOwner: {IsOwner}");

        objectManipulator = GetComponent<ObjectManipulator>();
        interactable = GetComponent<Interactable>();

        if (objectManipulator != null)
        {
            objectManipulator.OnManipulationStarted.AddListener(HandleInteractionStarted);
        }
        if (interactable != null)
        {
            interactable.OnClick.AddListener(HandleInteractionStarted);
        }
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        Debug.Log($"[Ownership] {gameObject.name} ownership changed from {prevOwner.ClientId} to {Owner.ClientId}");
    }

    private void HandleInteractionStarted()
    {
        Debug.Log($"[Interaction] Interaction started with {gameObject.name}! (Current owner: {Owner.ClientId})");

        if (!base.IsOwner)
        {
            Debug.Log("[Interaction] Not the owner, requesting ownership from server.");
            RequestOwnershipServerRpc();
        }
    }

    private void HandleInteractionStarted(ManipulationEventData eventData)
    {
        HandleInteractionStarted();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(NetworkConnection sender = null)
    {
        Debug.Log($"[Server] Received ownership request for {gameObject.name} from {sender.ClientId}. Granting ownership.");
        base.GiveOwnership(sender);
    }

    void Update()
    {
        if (base.IsOwner)
        {
            SyncTransformServerRpc(transform.position, transform.rotation);
        }
    }

    [ServerRpc]
    private void SyncTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        BroadcastTransformObserversRpc(position, rotation);
    }

    [ObserversRpc(BufferLast = true)]
    private void BroadcastTransformObserversRpc(Vector3 position, Quaternion rotation)
    {
        if (!base.IsOwner)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}