using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyUIController : MonoBehaviour 
{
	private GameObject ControlUI;
	private GameObject Father;

	void Start () 
	{
		ControlUI = this.transform.Find("kuang").gameObject;

		ControlUI.SetActive(false);

		Father = this.transform.parent.gameObject;
	}
	
	void Update () 
	{
		bool DoNotShowCopyUI = Father.GetComponent<MoveByMouse>().GetCanMove();
		if (!DoNotShowCopyUI)
		{
			ControlUI.SetActive(true);
		}
		else
		{
			ControlUI.SetActive(false);
		}
	}
}
