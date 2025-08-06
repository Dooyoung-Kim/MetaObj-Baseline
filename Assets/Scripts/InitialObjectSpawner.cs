using FishNet.Object;
using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가
using System.Collections.Generic;

public class InitialObjectSpawner : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToSpawn;

    public override void OnStartServer()
    {
        base.OnStartServer();
        // 바로 스폰하지 않고, 딜레이를 주는 코루틴을 실행합니다.
        StartCoroutine(SpawnObjectsAfterDelay());
    }

    private IEnumerator SpawnObjectsAfterDelay()
    {
        // 네트워크 상태가 안정화될 시간을 벌기 위해 아주 잠깐 기다립니다.
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