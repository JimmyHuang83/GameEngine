using UnityEngine;
using System.Collections;

public class SymbolCheck : MonoBehaviour {
	private static SymbolCheck instance;
	public static SymbolCheck Instance()
	{
	    if (instance == null)
	        instance =(SymbolCheck)GameObject.FindObjectOfType(typeof(SymbolCheck));
	    return instance;
	}

	public string[] symbolStr;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
