using UnityEngine;

public class ZOffsetChildren : MonoBehaviour
{
    public float startZ = 0f;
    public float stepZ = 10f;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 pos = child.localPosition;
            pos.z = startZ + stepZ * i;
            child.localPosition = pos;
        }
    }
}
