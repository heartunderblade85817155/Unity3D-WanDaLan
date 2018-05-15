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

    public string CellName = "Cell_Rhombus";

    public Vector3 InitPos = new Vector3(0.0f, -5.0f, 1.0f);

    private Vector3 CopyInitPos;

    private bool OpenOpration;

    private GameObject CircleBackGround;

    private bool BeginCreate;
    private bool BeginDelete;

    private int TheDeleteNumber;
    public float DeleteTime = 0.5f;
    private float DeleteTotalTime = 0.0f;

    // 这一关中，该类型细胞的最大半径
    public float CellMaxRadius;

    public void CreateCell(string CellName, bool flag = false)
    {
        if ((CurrentStage & 1) == 1)
        {
            return;
        }

        if (CellNum >= CellMaxNum)
        {
            return;
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
            ThisCell.transform.position = CopyInitPos;

            if (!flag)
                CircleBackGround.GetComponent<CopyCellDynamic>().SetNewCell(ThisCell);

            BeginCreate = false;
        }
        else
        {
            CellObject = Resources.Load(CellName) as GameObject;
            GameObject NewCell = Instantiate(CellObject);
            if (NewCell)
            {
                CellNum++;

                //更改主细胞名字
                NewCell.name = (CellNum * 2 - 2).ToString();
                NewCell.GetComponent<SpriteRenderer>().sortingOrder = CellNum;
                //更改子细胞名字
                GameObject ThisChild = NewCell.transform.Find("1").gameObject;
                ThisChild.name = (CellNum * 2 - 1).ToString();
                ThisChild.GetComponent<SpriteRenderer>().sortingOrder = CellNum;

                NewCell.transform.position = CopyInitPos;
                NewCell.tag = "TheCell";

                if (!flag)
                    CircleBackGround.GetComponent<CopyCellDynamic>().SetNewCell(NewCell);

                BeginCreate = false;

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
            return;
        }

        if (CellList.Count > 0)
        {
            for (int i = 0; i < CellList.Count; ++i)
            {
                if (CellList[i].Equals(DelCell))
                {
                    TheDeleteNumber = i;
                    BeginDelete = true;
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

    void Start()
    {
        CellNum = 0;

        OpenOpration = true;

        Moved = false;

        PreMousePos = Vector3.zero;

        CopyInitPos = InitPos;

        CreateCell(CellName, true);

        BeginCreate = false;

        BeginDelete = false;

        CircleBackGround = GameObject.Find("BG_Circle").gameObject;
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

        while (MoveScale * NumDiv < NumFloor)
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

    void Update()
    {
        // 复制/删除细胞的时候不进行任何其他有关细胞操作的逻辑
        if (BeginCreate || BeginDelete)
        {
            if (BeginDelete)
            {
                if (DeleteTotalTime < DeleteTime)
                {
                    DeleteTotalTime += Time.deltaTime;
                    CellList[TheDeleteNumber].GetComponent<SpriteRenderer>().material.SetFloat("_NoiseCoefficient", DeleteTotalTime / DeleteTime);
                }
                else
                {
                    CellList[TheDeleteNumber].GetComponent<MoveByMouse>().SetCanMove(true);
                    CellList[TheDeleteNumber].SetActive(false);
                    CellList[TheDeleteNumber].GetComponent<SpriteRenderer>().material.SetFloat("_NoiseCoefficient", 0.0f);
                    CellNum--;

                    BeginDelete = false;
                    DeleteTotalTime = 0.0f;
                }
            }
            return;
        }


        CurrentStage = this.GetComponent<GameController>().GetCurrentStage();

        if (!((CurrentStage & 1) != 1 || (CurrentStage >> 4 & 1) != 1))
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (!OpenOpration)
            {
                return;
            }

            if (!MoveCell)
            {
                //如果使用透视矩阵，需要深度值
                //Vector3 MousePos = Input.mousePosition;
                //MousePos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                //如果没发生碰撞
                if (!hit)
                {
                    return;
                }

                Debug.Log("catch + " + hit.collider.gameObject.name);

                //如果发生碰撞但是不是细胞
                if (hit.collider.gameObject.tag.Equals("TheCell") || hit.collider.gameObject.tag.Equals("DecIns"))
                {
                    MoveCell = hit.collider.gameObject;
                }
                else
                {
                    if (hit.collider.gameObject.tag.Equals("Copy"))
                    {
                        CopyRemote = hit.collider.transform.parent.gameObject;
                        CopyRemote.GetComponent<CopyController>().ShowOn();
                        OpenOpration = false;
                    }
                    else if (hit.collider.gameObject.tag.Equals("Delete"))
                    {
                        CopyRemote = hit.collider.transform.parent.gameObject;
                        CopyRemote.GetComponent<DeleteController>().ShowOn();
                        OpenOpration = false;
                    }
                    return;
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
                    MoveCell.GetComponent<MoveByMouse>().SetCanMove(false);
                }
            }

            if (CopyRemote)
            {
                if (CopyRemote.GetComponent<CopyController>())
                {
                    CopyInitPos = CopyRemote.transform.parent.parent.parent.position;

                    BeginCreate = true;

                    Vector3 Direction = new Vector3(CopyInitPos.x - InitPos.x, CopyInitPos.y - InitPos.y, 0.0f);

                    Direction = -Direction;

                    if (Mathf.Approximately(Direction.magnitude, 0.0f))
                    {
                        Direction = new Vector2(1.0f, 0.0f);
                    }

                    RaycastHit2D[] hit = Physics2D.RaycastAll(CopyInitPos, Direction);

                    float CopyDis = CellMaxRadius * 2.0f;
                    if (hit.Length > 0)
                    {
                        for (int i = 0; i < hit.Length; ++i)
                        {
                            if (hit[i].collider.gameObject.name.Equals("NoCenterCircle"))
                            {
                                CopyDis = (hit[i].distance - CellMaxRadius * 3.0f) > 0.0f ? CellMaxRadius * 2 : hit[i].distance - 0.5f - CellMaxRadius;
                                break;
                            }
                        }
                    }

                    CircleBackGround.GetComponent<CopyCellDynamic>().SetUseMetaBall(true, CopyInitPos, CopyInitPos + Direction.normalized * CopyDis, CopyRemote);

                    CopyInitPos += Direction.normalized * CopyDis;
                }
                else if (CopyRemote.GetComponent<DeleteController>())
                {
                    BeginDelete = true;

                    CopyRemote.GetComponent<DeleteController>().DeleteIt();
                }
            }

            OpenOpration = true;

            //稍微移动使其坐标为整数
            if (MoveCell && Moved)
            {
                if (!MoveCell.tag.Equals("DecIns"))
                {
                    Vector3 CurrentPos = MoveCell.transform.position;
                    Vector3 SmallOffset = new Vector3(GetRound(CurrentPos.x), GetRound(CurrentPos.y), CurrentPos.z) - CurrentPos;
                    MoveCell.GetComponent<MoveByMouse>().SetMouseMove(SmallOffset);
                }
            }

            Moved = false;
            PreMousePos = Vector3.zero;
            MoveCell = null;
            CopyRemote = null;
        }
    }
}
