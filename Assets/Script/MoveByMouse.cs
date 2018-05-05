using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByMouse : MonoBehaviour 
{
	private bool CanMove;

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
