using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorateController : MonoBehaviour
{
    private List<GameObject> TheDecSprite = new List<GameObject>();

    private Vector3 InitPos;

    private GameObject TrueFather;

    void Start()
    {
        InitPos = GameObject.Find("SceneController").GetComponent<CellController>().InitPos;

        InitPos = new Vector3(InitPos.x, InitPos.y, -2.0f);

        TrueFather = GameObject.FindWithTag("GoalTarget");
    }

    public void SetStage(List<string> DecName, List<Vector2> DecScale)
    {
        if (TheDecSprite.Count == 0)
        {
            TheDecSprite.Add(GameObject.Find("Dec1"));
            TheDecSprite.Add(GameObject.Find("Dec2"));
            TheDecSprite.Add(GameObject.Find("Dec3"));
        }

        for (int i = 0; i < DecName.Count; ++i)
        {
            if (TheDecSprite[i] == null)
            {
                continue;
            }

            Sprite DecSprite = Resources.Load<Sprite>(DecName[i]);
            TheDecSprite[i].GetComponent<SpriteRenderer>().sprite = DecSprite;

            if (!TheDecSprite[i].transform.parent.gameObject.Equals(TrueFather))
            {
                TheDecSprite[i].transform.localScale = new Vector3(DecScale[i].x, DecScale[i].y, 1.0f);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            //如果没发生碰撞
            if (!hit)
            {
                return;
            }
			if (hit.collider.gameObject.tag.Equals("Dec"))
			{
				int TheName = int.Parse(hit.collider.gameObject.name);

                TheDecSprite[TheName - 1].transform.parent = null;
                TheDecSprite[TheName - 1].transform.position = InitPos;
                TheDecSprite[TheName - 1].transform.localScale = new Vector3(TrueFather.transform.localScale.x, TrueFather.transform.localScale.y, 1.0f);

                TheDecSprite[TheName - 1].AddComponent<BoxCollider2D>();
                TheDecSprite[TheName - 1].AddComponent<MoveByMouse>();
                TheDecSprite[TheName - 1].gameObject.tag = "DecIns";
                TheDecSprite[TheName - 1].transform.parent = TrueFather.transform;
			}
        }
    }
}
