using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCellMove : MonoBehaviour 
{

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag.Equals("Copy") || other.gameObject.tag.Equals("Delete"))
		{
			return;
		}
		Vector3 Delta = this.transform.position - other.transform.position;
		Vector2 Dir = new Vector2(Delta.x, Delta.y).normalized * 0.5f;

		other.transform.position = new Vector3(other.transform.position.x + Dir.x, other.transform.position.y + Dir.y, other.transform.position.z);
	}

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
