using FishNet.Object;
using UnityEngine;

public class PlayerAvatarController : NetworkBehaviour
{
    private Transform mrtkCameraTransform;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // If this avatar is mine (the local player), find the main camera.
        if (base.IsOwner)
        {
            mrtkCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Only process for the local player.
        if (base.IsOwner && mrtkCameraTransform != null)
        {
            // 1. Update your own avatar's position to match the camera.
            //    This makes the local avatar move with keyboard input in the editor.
            this.transform.position = mrtkCameraTransform.position;
            this.transform.rotation = mrtkCameraTransform.rotation;

            // 2. Send the updated transform to the server to sync with others.
            SyncTransformServerRpc(mrtkCameraTransform.position, mrtkCameraTransform.rotation);
        }
    }

    // [Local Player -> Server] Send my transform values.
    [ServerRpc]
    private void SyncTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        // The server relays the received values to all other clients.
        BroadcastTransformObserversRpc(position, rotation);
    }

    // [Server -> All Clients] Update the avatar's transform.
    [ObserversRpc(BufferLast = true)]
    private void BroadcastTransformObserversRpc(Vector3 position, Quaternion rotation)
    {
        // Don't update my own avatar via the network since I'm controlling it directly.
        // Only update the avatars of other players.
        if (!base.IsOwner)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}