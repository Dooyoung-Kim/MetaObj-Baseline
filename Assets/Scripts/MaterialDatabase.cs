using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialDatabase", menuName = "MetaObject/Material Database")]
public class MaterialDatabase : ScriptableObject
{
    public List<MaterialDataSO> materials;

    public MaterialDataSO GetByName(string name)
    {
        return materials.Find(m => m.MaterialName == name);
    }
}
