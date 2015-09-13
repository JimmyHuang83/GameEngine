using UnityEngine;
using System.Collections;

namespace GameEngine
{
	public class AutoDestroy : MonoBehaviour {

		public float duration = 0f;
		void Start(){}
		void Update()
		{
			if (duration <= 0f)
			{
				GameObject.Destroy(this.gameObject);
			}
			duration -= Time.deltaTime;
		}
	}
}
