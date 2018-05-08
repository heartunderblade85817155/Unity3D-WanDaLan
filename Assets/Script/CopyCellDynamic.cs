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

    public float Radius;
    public float Threshold;
    public Color CellMetaColor;

    private GameObject CopyRemote;

    private int Stage;

	private GameObject OldCell;
	private GameObject NewCell;

    public void SetUseMetaBall(bool flag, Vector2 Origin, Vector2 Final, GameObject Remote)
    {
        UseDynamicMetaBall = flag;

        OriginPos = Origin;
        FinalPos = Final;
        NewCellCurrentPos = Origin;

        CopyRemote = Remote;

		OldCell = Remote.transform.parent.parent.parent.gameObject;
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
        CircleMaterial.SetFloat("_Threshold", Threshold);
        CircleMaterial.SetFloat("_Radius", Radius);
        CircleMaterial.SetColor("_MetaBallColor", CellMetaColor);
    }

    void Update()
    {
        if (UseDynamicMetaBall)
        {
            if ((Stage & 1) == 0)
            {
				CircleMaterial.SetVector("_OldCellPos", new Vector4(OriginPos.x, OriginPos.y, 1.0f, 1.0f));
                CircleMaterial.SetVector("_NewCellPos", new Vector4(OriginPos.x, OriginPos.y, 1.0f, 1.0f));

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

                    CircleMaterial.SetVector("_OldCellPos", new Vector4(OriginPos.x, OriginPos.y, 1.0f, 1.0f));
                    CircleMaterial.SetVector("_NewCellPos", new Vector4(NewCellCurrentPos.x, NewCellCurrentPos.y, 1.0f, 1.0f));
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

                    OldCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", Mathf.Clamp(HideOrShowTotalTime / HideOrShowTime, 0.0f, 1.0f));
					NewCell.GetComponent<SpriteRenderer>().material.SetFloat("_BlendAlpha", Mathf.Clamp(HideOrShowTotalTime / HideOrShowTime, 0.0f, 1.0f));
                }
                else 
                {
                    Stage = 0;
                    HideOrShowTotalTime = 0.0f;
                    CircleMaterial.SetVector("_OldCellPos", new Vector4(-100.0f, -100.0f, 1.0f, 1.0f));
                    CircleMaterial.SetVector("_NewCellPos", new Vector4(-100.0f, -100.0f, 1.0f, 1.0f));
                    UseDynamicMetaBall = false;
                }
            }

        }

        CircleMaterial.SetFloat("_Threshold", Threshold);
        CircleMaterial.SetFloat("_Radius", Radius);
        CircleMaterial.SetColor("_MetaBallColor", CellMetaColor);
    }
}
