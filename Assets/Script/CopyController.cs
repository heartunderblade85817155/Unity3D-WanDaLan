using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyController : MonoBehaviour 
{
	private GameObject SceneController;

	private GameObject normal;
	private GameObject on;
	private GameObject disable;

	private string TheCellName;

	void Start () 
	{
		normal = this.transform.Find("copy").gameObject;
		normal.SetActive(true);
		on = this.transform.Find("copyon").gameObject;
		on.SetActive(false);
		disable = this.transform.Find("copyx").gameObject;
		disable.SetActive(false);

		SceneController = GameObject.Find("SceneController");

		TheCellName = this.transform.parent.transform.parent.name;
	}

	public void ShowOn()
	{
        normal.SetActive(false);
        on.SetActive(true);
        disable.SetActive(false);
    }

    public void CreateIt()
    {
        SceneController.GetComponent<CellController>().CreateCell(TheCellName);

        this.transform.parent.transform.parent.transform.parent.GetComponent<MoveByMouse>().SetCanMove(true);
    }
	
	void Update () 
	{
		if (!SceneController.GetComponent<CellController>().CanCopy())
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
