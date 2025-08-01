using FishNet.Object;
using UnityEngine;
using System.Collections.Generic; // List 사용을 위해 추가

public class InitialObjectSpawner : NetworkBehaviour
{
    // Inspector 창에서 스폰할 프리팹들을 여기에 연결합니다.
    [SerializeField]
    private List<GameObject> objectsToSpawn;

    // 서버가 완전히 시작되었을 때 자동으로 호출됩니다.
    public override void OnStartServer()
    {
        base.OnStartServer();

        foreach (GameObject prefab in objectsToSpawn)
        {
            if (prefab != null)
            {
                // 1. 프리팹을 씬에 생성합니다.
                GameObject spawnedObject = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);

                // 2. 생성된 객체를 네트워크에 스폰시켜 모든 클라이언트에게 즉시 보이게 합니다.
                base.ServerManager.Spawn(spawnedObject);

                Debug.Log(prefab.name + " has been spawned by the server.");
            }
        }
    }
}