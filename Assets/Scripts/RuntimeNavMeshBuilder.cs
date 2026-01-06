using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class RuntimeNavMeshBuilder : MonoBehaviour
{
    [Tooltip("Root GameObject containing geometry to include in NavMesh (eg: Plane)")]
    public Transform geometryRoot;

    [Tooltip("Layer mask to include when collecting sources (Default: Everything)")]
    public LayerMask collectLayerMask = ~0;

    [Tooltip("Agent type ID to use (0 is default)")]
    public int agentTypeID = 0;

    [Tooltip("Bounds padding added around the geometry root bounds to ensure coverage")]
    public Vector3 boundsPadding = new Vector3(1f, 1f, 1f);

    [Tooltip("Use physics colliders instead of render meshes")]
    public bool usePhysicsColliders = false;

    NavMeshDataInstance navMeshInstance;
    NavMeshData navMeshData;

    void Start()
    {
        if (geometryRoot == null)
        {
            Debug.LogWarning("[RuntimeNavMeshBuilder] geometryRoot is null. Using this GameObject as root.");
            geometryRoot = transform;
        }

        Build();
    }

    void OnDisable()
    {
        if (navMeshInstance.valid)
            NavMesh.RemoveNavMeshData(navMeshInstance);
    }

    public void Build()
    {
        // 1) compute bounds around geometry root renderers (or fallback)
        Bounds b = new Bounds(geometryRoot.position, Vector3.zero);
        bool any = false;

        var renderers = geometryRoot.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            foreach (var r in renderers)
            {
                // include only renderers on selected layers
                if (((1 << r.gameObject.layer) & collectLayerMask.value) == 0) continue;
                if (!any) { b = r.bounds; any = true; }
                else b.Encapsulate(r.bounds);
            }
        }

        if (!any)
        {
            // fallback bounds if nothing matched - use collider bounds if present
            var colliders = geometryRoot.GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var c in colliders)
                {
                    if (((1 << c.gameObject.layer) & collectLayerMask.value) == 0) continue;
                    if (!any) { b = c.bounds; any = true; }
                    else b.Encapsulate(c.bounds);
                }
            }
        }

        if (!any)
        {
            // final fallback to a small bounds around root position
            b = new Bounds(geometryRoot.position, Vector3.one * 10f);
            Debug.LogWarning("[RuntimeNavMeshBuilder] No matched renderers/colliders found for the selected layer mask. Using fallback bounds.");
        }
        else
        {
            b.Expand(boundsPadding);
        }

        // 2) collect sources into a list (use the Bounds overload)
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

        NavMeshCollectGeometry geometryMode = usePhysicsColliders ? NavMeshCollectGeometry.PhysicsColliders : NavMeshCollectGeometry.RenderMeshes;

        NavMeshBuilder.CollectSources(
            b,                               // includedWorldBounds
            collectLayerMask.value,          // includedLayerMask (int)
            geometryMode,                    // geometry type
            0,                                // default area
            new List<NavMeshBuildMarkup>(),   // markups
            sources                           // results
        );

        if (sources.Count == 0)
        {
            Debug.LogWarning("[RuntimeNavMeshBuilder] Collected 0 NavMeshBuildSource items. Ensure geometry has MeshFilters/MeshRenderers or Colliders and correct layer. You can set usePhysicsColliders=true if you only have Colliders.");
        }
        else
        {
            Debug.Log($"[RuntimeNavMeshBuilder] Collected {sources.Count} sources. Building navmesh...");
        }

        // 3) get build settings
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(agentTypeID);

        // 4) build NavMeshData
        navMeshData = NavMeshBuilder.BuildNavMeshData(
            buildSettings,
            sources,
            b,
            geometryRoot.position,
            Quaternion.identity
        );

        if (navMeshData == null)
        {
            Debug.LogError("[RuntimeNavMeshBuilder] BuildNavMeshData returned null.");
            return;
        }

        // 5) add it to runtime navmesh
        navMeshInstance = NavMesh.AddNavMeshData(navMeshData);
        Debug.Log("[RuntimeNavMeshBuilder] Runtime NavMesh built and added.");
    }
}
