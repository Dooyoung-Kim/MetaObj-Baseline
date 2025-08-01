using FishNet.Object;
using UnityEngine;
using System.Collections.Generic; // List ����� ���� �߰�

public class InitialObjectSpawner : NetworkBehaviour
{
    // Inspector â���� ������ �����յ��� ���⿡ �����մϴ�.
    [SerializeField]
    private List<GameObject> objectsToSpawn;

    // ������ ������ ���۵Ǿ��� �� �ڵ����� ȣ��˴ϴ�.
    public override void OnStartServer()
    {
        base.OnStartServer();

        foreach (GameObject prefab in objectsToSpawn)
        {
            if (prefab != null)
            {
                // 1. �������� ���� �����մϴ�.
                GameObject spawnedObject = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);

                // 2. ������ ��ü�� ��Ʈ��ũ�� �������� ��� Ŭ���̾�Ʈ���� ��� ���̰� �մϴ�.
                base.ServerManager.Spawn(spawnedObject);

                Debug.Log(prefab.name + " has been spawned by the server.");
            }
        }
    }
}