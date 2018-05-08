using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCellMove : MonoBehaviour 
{

	void OnTriggerStay2D(Collider2D other)
	{
		other.transform.position += (this.transform.position - other.transform.position).normalized * 0.5f;
	}

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
