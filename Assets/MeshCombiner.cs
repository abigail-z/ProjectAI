using UnityEngine;
using System.Collections.Generic;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        MeshCollider[] MeshColliders = GetComponentsInChildren<MeshCollider>();
        CombineInstance[] combine = new CombineInstance[MeshColliders.Length];
        List<CombineInstance> combiners = new List<CombineInstance>();

        int i = 0;
        while (i < MeshColliders.Length)
        {
            combine[i].mesh = MeshColliders[i].sharedMesh;
            combine[i].transform = MeshColliders[i].transform.localToWorldMatrix;
            MeshColliders[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshCollider>().sharedMesh = new Mesh();
        transform.GetComponent<MeshCollider>().sharedMesh.CombineMeshes(combine, true, true);
        transform.gameObject.SetActive(true);
    }
}