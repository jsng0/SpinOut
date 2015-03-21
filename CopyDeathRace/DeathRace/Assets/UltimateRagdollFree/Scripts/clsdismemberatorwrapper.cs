using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
/// <summary>
/// 2013-07-08
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// Simple wrapper class to overwrite a target Unity4 skinned mesh renderer triangles and maintain compatibility with Unity 3.5
/// </summary>
public class clsdismemberatorwrapper: MonoBehaviour {
	public SkinnedMeshRenderer vargskm;
	public bool vargforcewrap;
	
	[HideInInspector]
	public Vector3[] propvertices;
	[HideInInspector]
	public Vector3[] propnormals;
	[HideInInspector]
	public Vector4[] proptangents;
	[HideInInspector]
	public Vector2[] propuvs;
	[HideInInspector]
	public BoneWeight[] propboneweights;
	[HideInInspector]
	public int[] proptriangles;
	[HideInInspector]
	public clssubmesher[] propsubmeshes;
	[HideInInspector]
	public Matrix4x4[] propbindposes;
	
	private Mesh varoriginalmesh;
	
	void Awake() {
		if (vargskm != null) {
			varoriginalmesh = vargskm.sharedMesh;
			if (varoriginalmesh != null) {
				if ((varoriginalmesh.triangles.Length != proptriangles.Length) || (vargforcewrap == true)) {
					metrestorewrapper();
				}
			}
		}
	}
	
	private void metrestorewrapper() {
		for (int varcounter = 0; varcounter < propsubmeshes.Length; varcounter++) {
			vargskm.sharedMesh.SetTriangles(propsubmeshes[varcounter].propsubmesh, varcounter);
		}
	}
}

[System.Serializable]
public class clssubmesher {
	public int[] propsubmesh;
}
