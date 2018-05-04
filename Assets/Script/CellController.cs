using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour 
{
	public int CellMaxNum = 4;
	private GameObject CellObject;

	private int CellNum;

	private List<GameObject> CellList = new List<GameObject>();

	private Vector3 PreMousePos;
	private GameObject MoveCell = null;
	public float CellSpeed = 1.0f;

    private bool Moved;

    private GameObject CopyRemote = null;

	public float MoveScale = 1.0f;

	private uint CurrentStage;

	public void CreateCell(string CellName)
	{
		if ((CurrentStage & 1) == 1)
		{
			return;
		}

		if (CellNum >= CellMaxNum)
		{
			return ;
		}

		//如果缓存池中有足够多的Cell则不需要创建
		if (CellList.Count >= CellNum + 1)
		{
			GameObject ThisCell = null;
			for (int i = 0; i < CellList.Count; ++i)
			{
				if (!CellList[i].activeInHierarchy)
				{
					CellList[i].SetActive(true);
					ThisCell = CellList[i];
                    
                    CellNum++;
					break;
				}
			}
			ThisCell.transform.position = new Vector3(0.0f, -5.0f, 1.0f);
		}
		else
		{
            CellObject = Resources.Load(CellName) as GameObject;
            GameObject NewCell = Instantiate(CellObject);
            if (NewCell)
            {
                CellNum++;

                NewCell.name = (CellNum * 2 - 2).ToString();
                NewCell.GetComponent<SpriteRenderer>().sortingOrder = CellNum;

                GameObject ThisChild = NewCell.transform.Find("1").gameObject;
                ThisChild.name = (CellNum * 2 - 1).ToString();
                ThisChild.GetComponent<SpriteRenderer>().sortingOrder = CellNum;

                NewCell.transform.position = new Vector3(0.0f, -5.0f, 1.0f);
                NewCell.tag = "TheCell";

				//加入到缓存池中
				CellList.Add(NewCell);
            }
		}
	}

	public void DeleteCell(GameObject DelCell)
	{
		if ((CurrentStage & 1) == 1)
		{
			return;
		}

		if (CellNum <= 1)
		{
			return ;
		}

		if (CellList.Count > 0)
		{
			for (int i = 0; i < CellList.Count; ++i)
			{
				if (CellList[i].Equals(DelCell))
				{
                    CellList[i].GetComponent<MoveByMouse>().SetCanMove(true);
                    CellList[i].SetActive(false);
                    CellNum--;
					break;
				}
			}
		}
	}

	public int GetCellNum()
	{
		return CellNum;
	}

	public bool CanCopy()
	{
		return CellNum < CellMaxNum;
	}

	public bool CanDelete()
	{
		return CellNum > 1;
	}

	void Start () 
	{
		CellNum = 0;

        Moved = false;

        PreMousePos = Vector3.zero;

		CreateCell("Cell_Rhombus");
	}
	
	private float GetRound(float Pos)
	{
		bool flag = false;

		if (Pos < 0.0f)
		{
			flag = true;
			Pos = Mathf.Abs(Pos);
		}
		
		float NumFloor = Pos - Mathf.Floor(Pos);

		float NumMod = Mathf.Repeat(NumFloor, MoveScale);

		float NumDiv = 0; 
		
		while(MoveScale * NumDiv <  NumFloor)
		{
			NumDiv++;
		}

		NumDiv--;

		if (NumMod < MoveScale / 2.0)
		{
			float final = Mathf.Floor(Pos) + NumDiv * MoveScale;
			if (flag)
			{
				final = -final;
			}
			return final;
		}
		else
		{
			float final = Mathf.Floor(Pos) + (NumDiv + 1) * MoveScale;
			if (flag)
			{
				final = -final;
			}
			return final;
		}
	}

	void Update () 
	{
		CurrentStage = this.GetComponent<GameController>().GetCurrentStage();

		if ((CurrentStage & 1) == 1)
		{
			return;
		}

		if (Input.GetMouseButton(0))
		{
			if (!MoveCell)
			{
				//如果使用透视矩阵，需要深度值
				//Vector3 MousePos = Input.mousePosition;
				//MousePos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				
				//如果没发生碰撞
				if (!hit)
				{
					return ;
				}

                Debug.Log("catch + " + hit.collider.gameObject.name);

				//如果发生碰撞但是不是细胞
                if (hit.collider.gameObject.tag.Equals("TheCell"))
                {
                    MoveCell = hit.collider.gameObject;
                }
				else
				{
                    if (hit.collider.gameObject.tag.Equals("Copy"))
                    {
                        CopyRemote = hit.collider.transform.parent.gameObject;
                        CopyRemote.GetComponent<CopyController>().ShowOn();
                    }
                    else if (hit.collider.gameObject.tag.Equals("Delete"))
                    {
                        CopyRemote = hit.collider.transform.parent.gameObject;
                        CopyRemote.GetComponent<DeleteController>().ShowOn();
                    }
                    return ;
				}
			}

            Vector3 Offset = Vector3.zero;
            if (PreMousePos.Equals(Vector3.zero))
            {
				PreMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return;
            }
            else
            {
                Offset = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - PreMousePos) * CellSpeed;
                if (!Mathf.Approximately(Offset.magnitude, 0.0f))
                {
                    Moved = true;
                }
				MoveCell.GetComponent<MoveByMouse>().SetMouseMove(Offset);
				PreMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
		}

        if (Input.GetMouseButtonUp(0))
		{
			if (!Moved)
			{
				if (MoveCell)
				{
                    MoveCell.GetComponent<MoveByMouse>().ChangeCanMove();
				}
			}

            if (CopyRemote)
            {
                if (CopyRemote.GetComponent<CopyController>())
                {
                    CopyRemote.GetComponent<CopyController>().CreateIt();
                }
                else if (CopyRemote.GetComponent<DeleteController>())
                {
                    CopyRemote.GetComponent<DeleteController>().DeleteIt();
                }
            }

            //稍微移动使其坐标为整数
            if (MoveCell && Moved)
            {
                Vector3 CurrentPos = MoveCell.transform.position;
                Vector3 SmallOffset = new Vector3(GetRound(CurrentPos.x), GetRound(CurrentPos.y), CurrentPos.z) - CurrentPos;
                MoveCell.GetComponent<MoveByMouse>().SetMouseMove(SmallOffset);
            }

            Moved = false;
			PreMousePos = Vector3.zero;
			MoveCell = null;
            CopyRemote = null;
        }
	}
}
