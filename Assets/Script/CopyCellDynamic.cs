using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCellDynamic : MonoBehaviour
{

    private bool UseDynamicMetaBall;

    private Vector2 OriginPos = new Vector2(0.0f, 0.0f);

    private Vector2 FinalPos = new Vector2(1.5f, 0.0f);

    private Vector2 NewCellCurrentPos;

    public float CopyTime = 0.8f;
    private float CopyTotalTime = 0.0f;

    public float HideOrShowTime = 0.5f;
    private float HideOrShowTotalTime = 0.0f;

    private Material CircleMaterial;

    public float Threshold;
    public Color CellMetaColor;

    private GameObject CopyRemote;

    private int Stage;

    private GameObject OldCell;
    private GameObject NewCell;
    public float BiggerXiShu;

    public List<float> SmallCircleShape = new List<float>();
    public List<Vector2> SmallCirclesPos = new List<Vector2>();
    public List<float> SmallCircleRadius = new List<float>();

    private List<Vector4> CurrentSmallCirclePos = new List<Vector4>();

    //调用MetaBall效果
    public void SetUseMetaBall(bool flag, Vector2 Origin, Vector2 Final, GameObject Remote)
    {
        UseDynamicMetaBall = flag;
        OriginPos = Origin;
        FinalPos = Final;
        CopyRemote = Remote;
        OldCell = Remote.transform.parent.parent.parent.gameObject;

        CircleMaterial.SetFloat("_Threshold", Threshold);
        CircleMaterial.SetColor("_MetaBallColor", CellMetaColor);
        CircleMaterial.SetInt("_SmallCircleNum", SmallCirclesPos.Count);
        CircleMaterial.SetFloatArray("_CellRadius" ,SmallCircleRadius);
        CircleMaterial.SetFloatArray("_CellShapes" ,SmallCircleShape);
    }

    public void SetNewCell(GameObject TheCell)
    {
        NewCell = TheCell;

        NewCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", 0.0f);
    }

    void Start()
    {
        Stage = 0;

        CircleMaterial = this.gameObject.GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        if (UseDynamicMetaBall)
        {
            if ((Stage & 1) == 0)
            {
                if (HideOrShowTotalTime.Equals(0.0f))
                {
                    CurrentSmallCirclePos.Clear();
                    for (int i = 0; i < SmallCirclesPos.Count; ++i)
                    {
                        CurrentSmallCirclePos.Add(new Vector4(OriginPos.x + SmallCirclesPos[i].x, OriginPos.y + SmallCirclesPos[i].y, 
                                                                OriginPos.x + SmallCirclesPos[i].x, OriginPos.y + SmallCirclesPos[i].y));
                    }
                    CircleMaterial.SetVectorArray("_OldAndNewCellsPos", CurrentSmallCirclePos);
                    CircleMaterial.SetFloat("_Bigger", 0.0f);
                }

                if (HideOrShowTotalTime < HideOrShowTime)
                {
                    HideOrShowTotalTime += Time.deltaTime;

                    OldCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", Mathf.Clamp(1 - HideOrShowTotalTime / HideOrShowTime, 0.0f, 1.0f));
                }
                else
                {
                    Stage |= 1;
                    HideOrShowTotalTime = 0.0f;
                }

            }
            else if ((Stage & 1) == 1 && (Stage >> 1 & 1) == 0)
            {
                if (CopyTotalTime < CopyTime)
                {
                    CopyTotalTime += Time.deltaTime;
                    NewCellCurrentPos = OriginPos + (FinalPos - OriginPos) * (CopyTotalTime / CopyTime);

                    CurrentSmallCirclePos.Clear();
                    for (int i = 0; i < SmallCirclesPos.Count; ++i)
                    {
                        CurrentSmallCirclePos.Add(new Vector4(OriginPos.x + SmallCirclesPos[i].x, OriginPos.y + SmallCirclesPos[i].y, NewCellCurrentPos.x + SmallCirclesPos[i].x, NewCellCurrentPos.y + SmallCirclesPos[i].y));
                    }
                    CircleMaterial.SetVectorArray("_OldAndNewCellsPos", CurrentSmallCirclePos);

                    CircleMaterial.SetFloat("_Bigger", (CopyTotalTime / CopyTime) * BiggerXiShu);
                }
                else
                {
                    Stage |= 1 << 1;
                    CopyTotalTime = 0.0f;
                    CopyRemote.GetComponent<CopyController>().CreateIt();
                }
            }
            else if ((Stage >> 1 & 1) == 1 && (Stage >> 2 & 1) == 0)
            {
                if (HideOrShowTotalTime < HideOrShowTime)
                {
                    HideOrShowTotalTime += Time.deltaTime;

                    float TmpAlpha = Mathf.Clamp(HideOrShowTotalTime / HideOrShowTime, 0.0f, 1.0f);
                    OldCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", TmpAlpha);
                    NewCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", TmpAlpha);
                }
                else
                {
                    Stage = 0;
                    HideOrShowTotalTime = 0.0f;
                    CurrentSmallCirclePos.Clear();
                    for (int i = 0; i < SmallCirclesPos.Count; ++i)
                    {
                        CurrentSmallCirclePos.Add(new Vector4(-100.0f, -100.0f, -100.0f, -100.0f));
                    }
                    CircleMaterial.SetVectorArray("_OldAndNewCellsPos", CurrentSmallCirclePos);
                    UseDynamicMetaBall = false;
                }
            }
        }

    }
}
