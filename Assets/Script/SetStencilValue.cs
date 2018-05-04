using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStencilValue : MonoBehaviour 
{
	private SpriteRenderer SetColor;
	private int TheStencilValue;

	private bool Init;
	void Start () 
	{
		Init = false;
		SetColor = this.GetComponent<SpriteRenderer>();
	}
	
	void Update () 
	{
		if (!Init)
		{
			Init = true;
			TheStencilValue = int.Parse(this.name) / 2;
			SetColor.material.SetInt("_StencilValue", 20 - TheStencilValue);
		}
	}
}
