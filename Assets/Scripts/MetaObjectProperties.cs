using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class MetaObjectProperties : MonoBehaviour
{
    [Header("Material Selection")]
    public MaterialDataSO MaterialData;

    [Header("Options")]
    public bool UseCalculatedMass = false;

    [Header("Debug View (ReadOnly at Start)")]
    public float Mass;
    public float DynamicFriction;
    public float StaticFriction;
    public float Bounciness;
    public string FrictionCombine = "Minimum";
    public string BounceCombine = "Minimum";

    public float ThermalConductivity;
    public float SurfaceRoughness;
    public float SoundReflection;
    public float SoundAbsorption;

    void Awake()
    {
        if (MaterialData == null)
        {
            Debug.LogWarning($"{name}: No material data assigned.");
            return;
        }

        ApplyMaterialSelf();

        if (UseCalculatedMass)
        {
            float childMass = CalculateChildMasses();
            Mass = childMass;
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Mass = rb != null ? rb.mass : 0f;
            DistributeMassToChildrenByVolume(Mass);  // 부모 질량 자식에 분배
        }

        EnsureComponents(gameObject); // Rigidbody 부착만, mass 설정은 안 함
    }

    void ApplyMaterialSelf()
    {
        StaticFriction = MaterialData.StaticFriction;
        DynamicFriction = MaterialData.DynamicFriction;
        Bounciness = MaterialData.Bounciness;
        ThermalConductivity = MaterialData.ThermalConductivity;
        SurfaceRoughness = MaterialData.SurfaceRoughness;
        SoundReflection = MaterialData.SoundReflection;
        SoundAbsorption = MaterialData.SoundAbsorption;

        float volume = GetColliderVolume(gameObject);
        float scaleFactor = 50f;
        float correctedVolume = volume * Mathf.Pow(scaleFactor, 3);
        Mass = MaterialData.Density * correctedVolume;
    }

    float CalculateChildMasses()
    {
        float totalMass = 0f;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("joint")) continue;

            var childProp = child.GetComponent<MetaObjectProperties>();
            if (childProp == null)
                childProp = child.gameObject.AddComponent<MetaObjectProperties>();

            childProp.MaterialData = this.MaterialData;
            childProp.ApplyMaterialSelf();

            totalMass += childProp.Mass;
        }

        return totalMass;
    }

    void DistributeMassToChildrenByVolume(float parentMass)
    {
        List<MetaObjectProperties> childrenProps = new List<MetaObjectProperties>();
        List<float> volumes = new List<float>();
        float totalVolume = 0f;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("joint")) continue;

            var childProp = child.GetComponent<MetaObjectProperties>();
            if (childProp == null)
                childProp = child.gameObject.AddComponent<MetaObjectProperties>();

            childProp.MaterialData = this.MaterialData;
            childProp.ApplyMaterialSelf();  // 속성만 업데이트

            float vol = GetColliderVolume(child.gameObject);
            volumes.Add(vol);
            childrenProps.Add(childProp);
            totalVolume += vol;
        }

        // 비율 계산 및 질량 분배
        for (int i = 0; i < childrenProps.Count; i++)
        {
            float ratio = totalVolume > 0f ? volumes[i] / totalVolume : 1f / childrenProps.Count;
            childrenProps[i].Mass = parentMass * ratio;
        }
    }

    float GetColliderVolume(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col == null) return 1f;

        if (col is BoxCollider box)
        {
            Vector3 size = Vector3.Scale(box.size, obj.transform.lossyScale);
            return size.x * size.y * size.z;
        }
        else if (col is SphereCollider sphere)
        {
            float radius = sphere.radius * MaxScaleComponent(obj.transform.lossyScale);
            return (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
        }
        else if (col is CapsuleCollider capsule)
        {
            float radius = capsule.radius * GetAxisScale(obj.transform.lossyScale, capsule.direction);
            float height = capsule.height * GetAxisScale(obj.transform.lossyScale, capsule.direction);
            float cylinderHeight = Mathf.Max(0f, height - 2f * radius);
            return Mathf.PI * Mathf.Pow(radius, 2) * cylinderHeight + (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
        }
        else if (col is MeshCollider meshCol)
        {
            Bounds bounds = meshCol.bounds;
            Vector3 size = bounds.size;
            return size.x * size.y * size.z;
        }

        return 1f;
    }

    float MaxScaleComponent(Vector3 v) => Mathf.Max(v.x, Mathf.Max(v.y, v.z));

    float GetAxisScale(Vector3 scale, int axis)
    {
        return axis == 0 ? scale.x : axis == 1 ? scale.y : scale.z;
    }

    void EnsureComponents(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();

        Collider col = obj.GetComponent<Collider>();
        if (col == null)
        {
            MeshCollider mc = obj.AddComponent<MeshCollider>();
            mc.convex = true;
            col = mc;
        }

        if (col.sharedMaterial == null)
        {
            PhysicMaterial mat = new PhysicMaterial("AutoMaterial");
            mat.staticFriction = StaticFriction;
            mat.dynamicFriction = DynamicFriction;
            mat.bounciness = Bounciness;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            mat.bounceCombine = PhysicMaterialCombine.Minimum;
            col.sharedMaterial = mat;
        }
    }
}
