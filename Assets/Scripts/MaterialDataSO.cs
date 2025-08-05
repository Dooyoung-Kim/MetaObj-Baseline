using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "MetaObject/Material Data")]
public class MaterialDataSO : ScriptableObject
{
    public string MaterialName;

    [Header("Physics Properties")]
    public float StaticFriction;
    public float DynamicFriction;
    public float Bounciness;
    public float Density;

    [Header("Haptic & Audio")]
    public float ThermalConductivity;
    public float SurfaceRoughness;
    public float SoundReflection;
    public float SoundAbsorption;

    [Header("Combine Settings")]
    public PhysicMaterialCombine FrictionCombine = PhysicMaterialCombine.Minimum;
    public PhysicMaterialCombine BounceCombine = PhysicMaterialCombine.Minimum;
}
