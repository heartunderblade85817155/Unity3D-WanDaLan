using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByMouse : MonoBehaviour 
{
	private bool CanMove;

	public Vector3 InitPos = new Vector3(0.0f, -5.0f, -1.0f);

	public float MaxDis = 4.5f;

	void Start () 
	{
		CanMove = true;
	}

	public bool GetCanMove()
	{
		return CanMove;
	}

	public void SetCanMove(bool flag)
	{
		CanMove = flag;
	}

    public void ChangeCanMove()
    {
        CanMove = CanMove ? false : true;
    }

    public void SetMouseMove(Vector3 Offset)
	{
        if (CanMove)
        {
			if (((this.transform.position + Offset) - InitPos).magnitude > MaxDis)
			{
				return;
			}
            this.transform.position += Offset;
        }
	}

    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			CanMove = true;
		}
    }
}
