using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteController : MonoBehaviour 
{
	private GameObject SceneController;

	private GameObject normal;
	private GameObject on;
	private GameObject disable;

	void Start () 
	{
		normal = this.transform.Find("delete").gameObject;
		normal.SetActive(true);
		on = this.transform.Find("deleteon").gameObject;
		on.SetActive(false);
		disable = this.transform.Find("deletex").gameObject;
		disable.SetActive(false);

		SceneController = GameObject.Find("SceneController");
	}
	
	public void ShowOn()
	{
        normal.SetActive(false);
        on.SetActive(true);
        disable.SetActive(false);
    }

    public void DeleteIt()
    {
        SceneController.GetComponent<CellController>().DeleteCell(this.transform.parent.transform.parent.transform.parent.gameObject);
    }

	void Update () 
	{
		if (!SceneController.GetComponent<CellController>().CanDelete())
		{
			normal.SetActive(false);
			on.SetActive(false);
			disable.SetActive(true);
			return;
		}
        else
        {
            normal.SetActive(true);
            on.SetActive(false);
            disable.SetActive(false);
        }

    }
}
