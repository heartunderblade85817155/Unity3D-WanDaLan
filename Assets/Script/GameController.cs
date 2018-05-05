using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	public int CellNumForWin = 2;

	private GameObject[] Cells = new GameObject[10];

	private List<GameObject> CellList = new List<GameObject>();

	public List<Vector2> HowToWin = new List<Vector2>();
	private List<bool> HowToWinFlag = new List<bool>();

	private uint CurrentStage;

	private GameObject GoalTarget;
	public string TargetName;

	private GameObject TheGoal = null;
	public float AppearTime = 1.0f;
	private float AppearTotalTime = 0.0f;

	public Vector2 WeiTiao = Vector2.zero;

	public int NextMission;

	public uint GetCurrentStage()
	{
		return CurrentStage;
	}

	void Start () 
	{
		CurrentStage = 0;

		GoalTarget = Resources.Load(TargetName) as GameObject;

		for (int i = 0; i < HowToWin.Count; ++i)
		{
			HowToWinFlag.Add(false);
		}
	}
	
	void Update () 
	{
		int CurrentCellNum = this.GetComponent<CellController>().GetCellNum();
		if (CellNumForWin.Equals(CurrentCellNum) && (CurrentStage & 1) == 0)
		{
			CellList.Clear();

			Cells = GameObject.FindGameObjectsWithTag("TheCell");

			int len = Cells.Length;
			for (int i = 0; i < len; ++i)
			{
				if (!Cells[i].GetComponent<MoveByMouse>().GetCanMove())
				{
					return;
				}
				CellList.Add(Cells[i]);
			}

			CellList.Sort(delegate(GameObject a, GameObject b)
			{
				if (a.transform.position.x.Equals(b.transform.position.x))
				{
					return a.transform.position.y.CompareTo(b.transform.position.y);
				}
				else
				{
					return a.transform.position.x.CompareTo(b.transform.position.x);
				}
			});

			for (int i = 0; i < HowToWinFlag.Count; ++i)
			{
				HowToWinFlag[i] = false;
			}

			for (int i = 1; i < CellList.Count; ++i)
			{
				Debug.Log(CellList[0].name + "asdasdasd" + CellList[i].name);

				Vector3 tmp = CellList[i].transform.position - CellList[0].transform.position;

				for (int j = 0; j < HowToWin.Count; ++j)
				{
					if (tmp.x.Equals(HowToWin[j].x) && tmp.y.Equals(HowToWin[j].y))
					{
						HowToWinFlag[j] = true;
					}
				}
			}

			for (int i = 0; i < HowToWinFlag.Count; ++i)
			{
				if (HowToWinFlag[i].Equals(false))
				{
					return;
				}
			}
			CurrentStage |= 1;
		}

		if ((CurrentStage & 1) == 1 && (CurrentStage >> 1 & 1) == 0)
		{
			Vector3 CenterPos = Vector3.zero;

			for (int i = 0; i < CellList.Count; ++i)
			{
				CenterPos += CellList[i].transform.position;
			}
			CenterPos /= CellList.Count;

			TheGoal = Instantiate(GoalTarget);

			TheGoal.transform.position = new Vector3(CenterPos.x + WeiTiao.x, CenterPos.y + WeiTiao.y, 0.0f);

			Color tmp = TheGoal.GetComponent<SpriteRenderer>().color;
			tmp.a = 0.0f;
			TheGoal.GetComponent<SpriteRenderer>().color = tmp;

			CurrentStage |= 1<<1;
		}

		if ((CurrentStage>>1 & 1) == 1 && (CurrentStage>>2 & 1) == 0)
		{
			Color tmp;
			if (AppearTotalTime > AppearTime)
			{
				tmp = TheGoal.GetComponent<SpriteRenderer>().color;
				tmp.a = 1.0f;
				TheGoal.GetComponent<SpriteRenderer>().color = tmp;

				for (int i = 0; i < CellList.Count; ++i)
				{
					CellList[i].SetActive(false);
				}

				CurrentStage |= 1<<2;

				SceneManager.LoadScene(NextMission);
			}

			tmp = TheGoal.GetComponent<SpriteRenderer>().color;
			tmp.a = AppearTotalTime / AppearTime;
			TheGoal.GetComponent<SpriteRenderer>().color = tmp;

			AppearTotalTime += Time.deltaTime;
		}


	}
}
