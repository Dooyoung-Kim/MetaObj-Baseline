# Project Title

This is a brief overview of the project.

## Getting Started

To get started with this project, please follow these steps:

1. Check the **`BaseScene`** for the main scene file.
2. Change the **Build Setting** to **Universal Windows Platform** to ensure the project builds correctly in Hololens2.

## Material Property Overview

| Material Name          | Static Friction | Dynamic Friction | Bounciness | Density (g/cm³) | Thermal Conductivity (W/m·K) | Surface Roughness | Sound Reflection | Sound Absorption |
| ---------------------- | --------------- | ---------------- | ---------- | --------------- | ---------------------------- | ----------------- | ---------------- | ---------------- |
| AluminumInsulated      | 0.40            | 0.30             | 0.10       | 2.70            | 180.0                        | 0.40              | 0.70             | 0.30             |
| CryoTank\_Aluminum     | 0.35            | 0.25             | 0.10       | 2.70            | 205.0                        | 0.35              | 0.65             | 0.35             |
| Engine\_Inconel        | 0.50            | 0.40             | 0.05       | 8.40            | 11.0                         | 0.60              | 0.85             | 0.15             |
| AluminumHoneycomb      | 0.30            | 0.20             | 0.05       | 1.80            | 90.0                         | 0.30              | 0.60             | 0.40             |
| AluminumComposite      | 0.40            | 0.30             | 0.10       | 2.20            | 100.0                        | 0.50              | 0.60             | 0.40             |
| Service\_AluminumPanel | 0.35            | 0.25             | 0.10       | 2.70            | 150.0                        | 0.40              | 0.65             | 0.35             |
| TitaniumShell          | 0.40            | 0.30             | 0.15       | 4.50            | 16.0                         | 0.30              | 0.90             | 0.10             |
| Wood                   | 0.50            | 0.40             | 0.25       | 0.70            | 0.2                          | 0.60              | 0.40             | 0.60             |
| Concrete               | 0.70            | 0.60             | 0.10       | 2.40            | 1.5                          | 0.70              | 0.80             | 0.20             |
| Rubber                 | 1.00            | 0.90             | 0.70       | 1.15            | 0.2                          | 0.80              | 0.10             | 0.90             |

## Material Property Descriptions and Sensory Mapping

| Property Name           | Description                                                                 | Sensory Domain(s)             |
|-------------------------|-----------------------------------------------------------------------------|-------------------------------|
| **Static Friction**     | Resistance before movement begins.                                          | **Haptic**, **Visual**        |
| **Dynamic Friction**    | Resistance during movement.                                                 | **Haptic**, **Visual**        |
| **Bounciness**          | How much the material rebounds after collision (0–1).                       | **Haptic**, **Visual**        |
| **Density**             | Mass per volume (g/cm³), affects perceived weight.                          | **Haptic**, **Visual**        |
| **Thermal Conductivity**| How well the material conducts heat (W/m·K).                                | **Haptic**                    |
| **Surface Roughness**   | Surface texture (0=smooth, 1=rough).                                        | **Haptic**                    |
| **Sound Reflection**    | How much sound is reflected (0=absorbed, 1=reflected).                      | **Auditory**                  |
| **Sound Absorption**    | How much sound is absorbed (0=reflected, 1=absorbed).                       | **Auditory**                  |

## Scene Graph Structure & Material Mass Calculation
### Node & Edge Structure

This project uses a hierarchical **Scene Graph** to represent the structure of the Saturn V rocket.

```
node_1 (Spaceship_SaturnV_SceneGraph)
├── node_2 ("1")
│   ├── node_3 (S-IC_00)
│   └── node_4 (S-IC_01)
├── node_5 ("2")
│   ├── node_6 (S-IC_02_Tank)
│   └── node_7 (S-IC_05_Tank)
└── ...
```
  
- **Nodes** (`"nodes"`):
  Each node represents a 3D component and contains:
  - `id`: Unique identifier
  - `name`: Component name (e.g., `"S-IVB_00"`)
  - `position`, `rotation`, `scale`: Local transform relative to the parent
  - `MaterialName`: Reference to material preset (e.g., `AluminumInsulated`)
  - `Mass`: Physical mass of the component (in kg)
  - Other physical & acoustic properties:  
    `StaticFriction`, `DynamicFriction`, `Bounciness`, `ThermalConductivity`, `SurfaceRoughness`, `SoundReflection`, `SoundAbsorption`, etc.

- **Edges** (`"edges"`):
  Edges describe parent-child relationships between nodes.
  All edges are of type `"vertical"` and form a tree structure starting from the root:
  
### Mass Assignment Principle

Each node’s `Mass` value in the JSON is **automatically calculated** using the following:

> **Mass = Material Density × Estimated Volume**  
> → Volume is approximated based on the node's `scale` (bounding box size).

- `MaterialName` refers to a material definition with known **density** (e.g., `AluminumInsulated = 2.7 g/cm³`).
- Volume is inferred from the node's scale values.
- The computed mass is stored in each node's `Mass` property for documentation and potential analysis.

⚠️ **Note**:  
The `Mass` value in the node is *not directly used* in the Unity `Rigidbody` component.  
Instead, **Rigidbody mass is assigned using pre-defined simulation values** for stability and control, independent of these estimates.

This allows:
- Realistic approximation of part-wise mass distribution in the graph
- Controlled and balanced physical simulation in Unity using fine-tuned Rigidbody settings

### How to Generate Scene Graph

Press '**E**' Key to generate **SceneGraph.json** in **/MetaObj-Baseline-main/Assets\SceneGraph.json**

## Additional Information

More detailed instructions and documentation will be updated soon.
