

using UnityEngine;
using System.Collections.Generic;

using System;
using System.Collections;
using System.Reflection;

//[ExecuteInEditMode()] 
public class CollisionLine : MonoBehaviour
{
	
	public string pathName ="";
	public Color pathColor = Color.cyan;
	public List<Vector3> nodes = new List<Vector3>(){};
	public int nodeCount;

	public bool initialized = false;
	public string initialName = "";
	
	
	public int curNode;
	
	public GameObject prefabObj;
	public GameObject cubeObj;
	
	public Material mt;
	
	public float width=1.0f;
	
	public float height=20.0f;
	
	public bool bezier;
	
	
	void OnEnable(){
		
	}
	
	void OnDrawGizmos() 
	{
		if(enabled) 
		{
			if(nodes.Count >0)
			{
				if (bezier)
				{
					DrawBezierPathHelper(nodes.ToArray(),pathColor);
				}
				else
				{
					DrawPathHelper(nodes.ToArray(),pathColor);
				}
			}
		}
	}
	/*
	void OnDrawGizmosSelected(){
		
		if(enabled) { // dkoontz
			if(nodes.Count >0){
				DrawPathHelper(nodes.ToArray(),pathColor);
			}
			
		
		} // dkoontz
		
	}
	*/
	
	private static Vector3[] PathControlPointGenerator(Vector3[] path){
		Vector3[] suppliedPath;
		Vector3[] vector3s;
		
		//create and store path points:
		suppliedPath = path;

		//populate calculate path;
		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length+offset];
		Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}	
		
		return(vector3s);
	}
	
	private static Vector3 Interp(Vector3[] pts, float t){
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}
	
	public void ToBezier()
	{
		List<Vector3> newNodes = new List<Vector3>(){};
		Vector3[] vector3s = PathControlPointGenerator(nodes.ToArray());
		
		Vector3 prevPt = Interp(vector3s,0);
		
		int SmoothAmount = nodes.Count*3;
		for (int i = 1; i <= SmoothAmount; i++) {
			newNodes.Add(prevPt);
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			prevPt = currPt;
			
		}
		
		nodes=newNodes;
		
	}
	
	public void Clear()
	{
		nodes.Clear();
		
	}
	
	public void GenerateBlock()
	{
		if (nodes.Count<=3)
		{
			return;	
		}
			
		GameObject obj = new GameObject("BlockMesh");
		
		obj.AddComponent(typeof(MeshFilter));
		obj.AddComponent(typeof(MeshRenderer));
		
		Mesh m_Mesh = new Mesh();
		
		int[] tris = new int[(nodes.Count-2)*3];
		int j=0;
		for (int i=0;i<tris.Length-2;i+=3)
		{
			tris[i]=0;
			tris[i+1]=j+1;
			tris[i+2]=j+2;
			j=j+1;
		}
		
		Vector2[] uvs = new Vector2[nodes.Count];
        int k = 0;
        while (k < uvs.Length) {
            uvs[k] = new Vector2(nodes[k].x, nodes[k].y);
            k++;
        }
		
        
		m_Mesh.vertices = nodes.ToArray();
		m_Mesh.triangles = tris;
		m_Mesh.uv = uvs;
		m_Mesh.Optimize();
		MeshFilter filter=obj.GetComponent<MeshFilter>();
        filter.sharedMesh=m_Mesh;
		
		if (mt!=null)
		{
			filter.renderer.material=mt;
		}
		obj.AddComponent(typeof(MeshCollider));
		
	}
	
	public void GenerateObstacle()
	{
		GameObject newParent = new GameObject("_Obstacle");
		Vector3 prevPt= nodes[0];
		for (int i = 1; i <=nodes.Count-1; i++) 
		{
			Vector3 currPt=nodes[i];
			Vector3 averagePos=(currPt+prevPt)/2;
			Vector3 dist=prevPt-currPt;
			
			float angle=Vector3.Angle(dist,new Vector3(1,0,0));
			if (dist.y<0)
			{
				angle=-angle;
			}
			//Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
			
			
			GameObject block=(GameObject)Instantiate(cubeObj,averagePos,Quaternion.Euler(0,0,angle));
			
			block.transform.localScale=new Vector3(dist.magnitude,width,height);
			
			
			block.transform.parent=newParent.transform;
			
		}
				
	}
	
	public void GenerateCollider()
	{
		/*
		GameObject newParent = new GameObject("_Parent");
		if (prefabObj!=null)
		{
			Vector3 prevPt= nodes[0];
			for (int i = 1; i <=nodes.Count-1; i++) 
			{
				Vector3 currPt=nodes[i];
				Vector3 averagePos=(currPt+prevPt)/2;
				Vector3 dist=prevPt-currPt;
				
				float angle=Vector3.Angle(dist,new Vector3(1,0,0));
				if (dist.y<0)
				{
					angle=-angle;
				}
				//Gizmos.DrawLine(currPt, prevPt);
				prevPt = currPt;
				
				GameObject block=(GameObject)Instantiate(prefabObj,averagePos,Quaternion.Euler(0,0,angle));
				
				exSprite exsprite=(exSprite)block.GetComponent(typeof(exSprite));
				
				//block.transform.lossyScale.Set(dist.magnitude,width,1);
				
				block.transform.parent=newParent.transform;
				//block.transform.localScale=new Vector3(5,5,5);
				//exsprite.enabled=false;
				
				//block.transform.lossyScale=new vE(0.5f,0.5f,0.5f);
				
				exsprite.scale=new Vector2(dist.magnitude/2,width);
				exsprite.Commit();
				//EditorUtility.SetDirty(exsprite);
				//Debug.Log(dist.magnitude);
				
			}
		}
		*/
		
	}
	
	private static void DrawBezierPathHelper(Vector3[] path, Color color){
		Vector3[] vector3s = PathControlPointGenerator(path);
		
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		Gizmos.color=color;
		int SmoothAmount = path.Length*5;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
	}
	
	private static void DrawPathHelper(Vector3[] path, Color color){
		Vector3[] vector3s = path;
		Vector3 prevPt=vector3s[0];
		Gizmos.color=color;
		for (int i = 1; i <= path.Length-1; i++) {
			
			
			Vector3 currPt=vector3s[i];
			
			Gizmos.DrawLine(currPt, prevPt);
			
			prevPt = currPt;
		}
	}
	

	
	void Update()
	{
			
		

	}
	
}

