using FishNet.Object;
using UnityEngine;
using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;

public class PlayerAvatarController : NetworkBehaviour
{
    private Transform mrtkCameraTransform;
    private bool isInitialized = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[OnStartClient] Player avatar created for client {Owner.ClientId}. IsOwner: {IsOwner}");

        if (base.IsOwner)
        {
            StartCoroutine(FindCameraCoroutine());
        }
        else
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Debug.Log($"[OnStartClient] Remote player ({Owner.ClientId}) avatar's Renderer.enabled state: {renderer.enabled}");
            }
        }
    }

    private IEnumerator FindCameraCoroutine()
    {
        while (CameraCache.Main == null)
        {
            Debug.LogWarning("[FindCamera] Waiting for MRTK CameraCache...");
            yield return new WaitForSeconds(0.5f);
        }

        mrtkCameraTransform = CameraCache.Main.transform;
        isInitialized = true;
        Debug.Log($"[FindCamera] Success: Found camera via CameraCache! Position: {mrtkCameraTransform.position}");
    }

    void Update()
    {
        if (base.IsOwner && isInitialized)
        {
            // This log can be spammy, uncomment only when needed.
            // Debug.Log("[Update] Sending transform data...");

            transform.position = mrtkCameraTransform.position;
            transform.rotation = mrtkCameraTransform.rotation;

            SyncTransformServerRpc(transform.position, transform.rotation);
        }
    }

    [ServerRpc]
    private void SyncTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        // This log can be spammy, uncomment only when needed.
        // Debug.Log($"[ServerRpc] Received transform from {Owner.ClientId}: {position}");
        BroadcastTransformObserversRpc(position, rotation);
    }

    [ObserversRpc(BufferLast = true)]
    private void BroadcastTransformObserversRpc(Vector3 position, Quaternion rotation)
    {
        if (!base.IsOwner)
        {
            transform.position = position;
            transform.rotation = rotation;
            // This log can be spammy, uncomment only when needed.
            // Debug.Log($"[ObserversRpc] Updating remote player ({Owner.ClientId}) avatar's transform: {position}");
        }
    }
}