using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[Serializable]
public class MaterialAttributes
{
    public float StaticFriction;
    public float DynamicFriction;
    public float Bounciness;
    public float Density;
    public float ThermalConductivity;
    public float SurfaceRoughness;
    public float SoundReflection;
    public float SoundAbsorption;
}

[Serializable]
public class ExportNode
{
    public string id;
    public string name;

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public float Mass;
    public string MaterialName;

    public float StaticFriction;
    public float DynamicFriction;
    public float Bounciness;
    public string FrictionCombine;
    public string BounceCombine;

    public float ThermalConductivity;
    public float SurfaceRoughness;
    public float SoundReflection;
    public float SoundAbsorption;
}


[Serializable]
public class ExportEdge
{
    public string parentId;
    public string childId;
    public string type;  // "vertical" or "horizontal"
}


[Serializable]
public class ExportRoot
{
    public List<ExportNode> nodes = new List<ExportNode>();
    public List<ExportEdge> edges = new List<ExportEdge>();
}

public class SceneGraphGenerator : MonoBehaviour
{
    public KeyCode exportKey = KeyCode.E;
    public string exportFileName = "SceneGraph.json";

    private int idCounter = 0;

    void Update()
    {
        if (Input.GetKeyDown(exportKey))
        {
            ExportHierarchy();
        }
    }

    void ExportHierarchy()
    {
        ExportRoot root = new ExportRoot();

        string rootId = GenerateId();
        ExportNode rootNode = new ExportNode();
        rootNode.id = rootId;
        rootNode.name = gameObject.name;
        rootNode.position = transform.position;
        rootNode.rotation = transform.rotation;
        rootNode.scale = transform.lossyScale;
        rootNode.Mass = 0f;
        rootNode.MaterialName = "None";
        rootNode.FrictionCombine = "Average";
        rootNode.BounceCombine = "Average";
        root.nodes.Add(rootNode);

        foreach (Transform child in transform)
        {
            Traverse(child, rootId, root, null); // 최상위에서는 상속 없음
        }


        string json = JsonUtility.ToJson(root, true);
        string path = Path.Combine(Application.dataPath, exportFileName);
        File.WriteAllText(path, json);
        Debug.Log($"[MetaObjectExporter] JSON Exported to: {path}");
    }

    void Traverse(Transform current, string parentId, ExportRoot root, MetaObjectProperties inheritedProperties)
    {
        if (current.CompareTag("joint"))
        {
            foreach (Transform child in current)
            {
                Traverse(child, parentId, root, inheritedProperties);
            }
            return;
        }

        string thisId = GenerateId();
        ExportNode node = new ExportNode();
        node.id = thisId;
        node.name = current.gameObject.name;
        node.position = current.position;
        node.rotation = current.rotation;
        node.scale = current.lossyScale;

        MetaObjectProperties currentProps = current.GetComponent<MetaObjectProperties>();
        MetaObjectProperties propsToUse = currentProps != null ? currentProps : inheritedProperties;

        if (propsToUse != null && propsToUse.MaterialData != null)
        {
            node.MaterialName = propsToUse.MaterialData.MaterialName;

            node.Mass = propsToUse.Mass;
            node.StaticFriction = propsToUse.StaticFriction;
            node.DynamicFriction = propsToUse.DynamicFriction;
            node.Bounciness = propsToUse.Bounciness;
            node.FrictionCombine = propsToUse.FrictionCombine;
            node.BounceCombine = propsToUse.BounceCombine;

            node.ThermalConductivity = propsToUse.ThermalConductivity;
            node.SurfaceRoughness = propsToUse.SurfaceRoughness;
            node.SoundReflection = propsToUse.SoundReflection;
            node.SoundAbsorption = propsToUse.SoundAbsorption;
        }
        else
        {
            node.MaterialName = "None";
            node.Mass = 0f;
            node.StaticFriction = 0f;
            node.DynamicFriction = 0f;
            node.Bounciness = 0f;
            node.FrictionCombine = "Average";
            node.BounceCombine = "Average";
            node.ThermalConductivity = 0f;
            node.SurfaceRoughness = 0f;
            node.SoundReflection = 0f;
            node.SoundAbsorption = 0f;
        }

        root.nodes.Add(node);

        ExportEdge edge = new ExportEdge();
        edge.parentId = parentId;
        edge.childId = thisId;
        edge.type = "vertical";
        root.edges.Add(edge);

        foreach (Transform child in current)
        {
            Traverse(child, thisId, root, propsToUse); // 상속된 속성 전달
        }

        FixedJoint joint = current.GetComponent<FixedJoint>();
        if (joint != null && joint.connectedBody != null)
        {
            Transform connectedTransform = joint.connectedBody.transform;
            ExportNode connectedNode = root.nodes.Find(n => n.name == connectedTransform.name &&
                                                             Vector3.Distance(n.position, connectedTransform.position) < 0.001f);
            if (connectedNode != null)
            {
                ExportEdge jointEdge = new ExportEdge();
                jointEdge.parentId = thisId;
                jointEdge.childId = connectedNode.id;
                jointEdge.type = "horizontal";
                root.edges.Add(jointEdge);
            }
        }
    }



    string GenerateId()
    {
        idCounter++;
        return $"node_{idCounter}";
    }
}
