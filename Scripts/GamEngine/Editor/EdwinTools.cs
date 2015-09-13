using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameEngine
{
	public class EdwinTools : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		[MenuItem("Edwin'Tools/ClearUp")]
		public static void ClearData()
		{
			PlayerPrefs.DeleteAll ();
		}
		// Update is called once per frame
		void Update () {

		}
	}
}
