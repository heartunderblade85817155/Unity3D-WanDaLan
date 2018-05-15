using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiaHaoController : MonoBehaviour
{
    public float Frenquency = 1.0f;

    private float TotalTime;

	private bool MouseIn;

	private GameObject Case;

	private List<string> DecNames = new List<string>();
	private List<Vector2> DecScales = new List<Vector2>();

	void OnMouseOver()
	{
		MouseIn = true;
	}

	void OnMouseExit()
	{
		MouseIn = false;
	}

    void Start()
    {
		MouseIn = false;

		Case = this.transform.Find("cemenu").gameObject;

		Case.SetActive(false);
    }

    private void ChangeAlpha(float Alpha)
    {
        Color TmpColor = this.GetComponent<SpriteRenderer>().color;
        TmpColor.a = Alpha;
        this.GetComponent<SpriteRenderer>().color = TmpColor;
    }

	public void SetDec(List<string> TheDecName, List<Vector2> TheDecScale)
	{
		DecNames = TheDecName;
		DecScales = TheDecScale;
	}

    public void OpenDecorate()
    {
		if (Case.activeInHierarchy)
		{
			Case.SetActive(false);
		}
		else
		{
			Case.SetActive(true);
			Case.GetComponent<DecorateController>().SetStage(DecNames, DecScales);
		}
    }

    void Update()
    {
        ChangeAlpha(Mathf.Sin(Mathf.Repeat(TotalTime, Frenquency) * 2.0f * 3.1415926f));

		TotalTime += Time.deltaTime;

        if (MouseIn)
        {
            if (Input.GetMouseButtonDown(0))
			{
				OpenDecorate();
			}
        }

    }
}
