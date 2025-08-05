using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialSOGenerator
{
    [MenuItem("Tools/Generate Material ScriptableObjects + Database")]
    public static void GenerateMaterialsWithDatabase()
    {
        string folderPath = "Assets/Materials/Generated";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Materials", "Generated");

        List<MaterialDataSO> createdMaterials = new List<MaterialDataSO>();

        var templateData = new Dictionary<string, MaterialDataSO>()
        {
            { "AluminumInsulated", new MaterialDataSO {
                MaterialName = "AluminumInsulated", StaticFriction = 0.4f, DynamicFriction = 0.3f, Bounciness = 0.1f, Density = 2.7f, ThermalConductivity = 180f, SurfaceRoughness = 0.4f, SoundReflection = 0.7f, SoundAbsorption = 0.3f }},
            { "CryoTank_Aluminum", new MaterialDataSO {
                MaterialName = "CryoTank_Aluminum", StaticFriction = 0.35f, DynamicFriction = 0.25f, Bounciness = 0.1f, Density = 2.7f, ThermalConductivity = 205f, SurfaceRoughness = 0.35f, SoundReflection = 0.65f, SoundAbsorption = 0.35f }},
            { "Engine_Inconel", new MaterialDataSO {
                MaterialName = "Engine_Inconel", StaticFriction = 0.5f, DynamicFriction = 0.4f, Bounciness = 0.05f, Density = 8.4f, ThermalConductivity = 11f, SurfaceRoughness = 0.6f, SoundReflection = 0.85f, SoundAbsorption = 0.15f }},
            { "AluminumHoneycomb", new MaterialDataSO {
                MaterialName = "AluminumHoneycomb", StaticFriction = 0.3f, DynamicFriction = 0.2f, Bounciness = 0.05f, Density = 1.8f, ThermalConductivity = 90f, SurfaceRoughness = 0.3f, SoundReflection = 0.6f, SoundAbsorption = 0.4f }},
            { "AluminumComposite", new MaterialDataSO {
                MaterialName = "AluminumComposite", StaticFriction = 0.4f, DynamicFriction = 0.3f, Bounciness = 0.1f, Density = 2.2f, ThermalConductivity = 100f, SurfaceRoughness = 0.5f, SoundReflection = 0.6f, SoundAbsorption = 0.4f }},
            { "Service_AluminumPanel", new MaterialDataSO {
                MaterialName = "Service_AluminumPanel", StaticFriction = 0.35f, DynamicFriction = 0.25f, Bounciness = 0.1f, Density = 2.7f, ThermalConductivity = 150f, SurfaceRoughness = 0.4f, SoundReflection = 0.65f, SoundAbsorption = 0.35f }},
            { "TitaniumShell", new MaterialDataSO {
                MaterialName = "TitaniumShell", StaticFriction = 0.4f, DynamicFriction = 0.3f, Bounciness = 0.15f, Density = 4.5f, ThermalConductivity = 16f, SurfaceRoughness = 0.3f, SoundReflection = 0.9f, SoundAbsorption = 0.1f }},
            { "Wood", new MaterialDataSO {
                MaterialName = "Wood", StaticFriction = 0.5f, DynamicFriction = 0.4f, Bounciness = 0.25f, Density = 0.7f, ThermalConductivity = 0.2f, SurfaceRoughness = 0.6f, SoundReflection = 0.4f, SoundAbsorption = 0.6f }},
            { "Concrete", new MaterialDataSO {
                MaterialName = "Concrete", StaticFriction = 0.7f, DynamicFriction = 0.6f, Bounciness = 0.1f, Density = 2.4f, ThermalConductivity = 1.5f, SurfaceRoughness = 0.7f, SoundReflection = 0.8f, SoundAbsorption = 0.2f }},
            { "Rubber", new MaterialDataSO {
                MaterialName = "Rubber", StaticFriction = 1.0f, DynamicFriction = 0.9f, Bounciness = 0.7f, Density = 1.15f, ThermalConductivity = 0.2f, SurfaceRoughness = 0.8f, SoundReflection = 0.1f, SoundAbsorption = 0.9f }},
        };

        foreach (var entry in templateData)
        {
            var instance = ScriptableObject.CreateInstance<MaterialDataSO>();
            EditorUtility.CopySerialized(entry.Value, instance);

            string assetPath = $"{folderPath}/{entry.Key}.asset";
            AssetDatabase.CreateAsset(instance, assetPath);
            createdMaterials.Add(instance);
        }

        MaterialDatabase database = ScriptableObject.CreateInstance<MaterialDatabase>();
        database.materials = createdMaterials;

        string dbPath = $"{folderPath}/MaterialDatabase.asset";
        AssetDatabase.CreateAsset(database, dbPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Material ScriptableObjects + Database created successfully.");
    }
}
