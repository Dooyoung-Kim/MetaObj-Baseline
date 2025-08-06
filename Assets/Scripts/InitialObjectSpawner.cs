using FishNet.Object;
using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� �߰�
using System.Collections.Generic;

public class InitialObjectSpawner : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToSpawn;

    public override void OnStartServer()
    {
        base.OnStartServer();
        // �ٷ� �������� �ʰ�, �����̸� �ִ� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(SpawnObjectsAfterDelay());
    }

    private IEnumerator SpawnObjectsAfterDelay()
    {
        // ��Ʈ��ũ ���°� ����ȭ�� �ð��� ���� ���� ���� ��� ��ٸ��ϴ�.
        yield return new WaitForSeconds(3.0f);

        Debug.Log("Waited for 3.0s, now spawning objects.");

        foreach (GameObject prefab in objectsToSpawn)
        {
            if (prefab != null)
            {
                GameObject spawnedObject = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
                base.ServerManager.Spawn(spawnedObject);
                Debug.Log(prefab.name + " has been spawned by the server.");
            }
        }
    }
}