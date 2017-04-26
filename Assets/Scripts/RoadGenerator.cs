using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    Straight,
    Left,
    Right,
    NotChange
}

public class RoadGenerator : MonoBehaviour
{

    public const float PATH_WIDTH = .7f;
    public const float PATH_HEIGHT = .7f;

    [HideInInspector]
    public MapGenerator Map;
    [SerializeField]
    private GameObject path;
    [SerializeField]
    private Sprite Normal;
    [SerializeField]
    private Sprite Forward;
    [SerializeField]
    private Sprite CurveLeft;
    [SerializeField]
    private Sprite CUrveRight;

    [SerializeField]
    private Sprite DarkArrow;
    [SerializeField]
    private Sprite LightArrow;

    public Direction direction;
    public Vector3 EndPosition;
    public int length;

    public void Generate(bool hasSpace, bool hasLongSpace, bool endPath = false, bool newLevel = false)
    {
        GameObject obj = null;
        for (int i = 0; i < length; i++)
        {
            if (direction == Direction.Straight)
            {
                obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(0, i * PATH_HEIGHT), Quaternion.identity);
            }
            else if (direction == Direction.Left)
            {
                obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(-i * PATH_WIDTH, 0), Quaternion.identity);
            }
            else
            {
                obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0), Quaternion.identity);
            }
            obj.transform.parent = transform;

            Path currentPath = obj.GetComponent<Path>();
            currentPath.Road = this;
            currentPath.GetDirection = Direction.NotChange;
            //if (hasSpace)
            //    currentPath.NeedLongJump = hasLongSpace;
            //else
            //    currentPath.NeedLongJump = false;

            GameObject diamond = obj.transform.FindChild("Diamond").gameObject;
            diamond.SetActive(true);
            GameObject arrow = obj.transform.FindChild("Arrow").gameObject;
            arrow.SetActive(false);
            switch (direction)
            {
                case Direction.Left:
                    arrow.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                    diamond.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                    break;
                case Direction.Right:
                    arrow.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
                    diamond.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
                    break;
                case Direction.Straight:
                    currentPath.spriteR.sprite = LightArrow;
                    break;
            }

            if (i == 2 && newLevel)
            {
                arrow.SetActive(false);
                Destroy(diamond);
            }

            if ((i == 0) || (i == length - 1))
            {
                obj.GetComponent<SpriteRenderer>().sprite = Normal;
            }


            if ((i == 0) || (i == length - 1 && hasSpace && !endPath))
            {
                arrow.SetActive(true);
                Destroy(diamond);
                currentPath.needAction = true;
                currentPath.GetDirection = direction;
                currentPath.NeedLongJump = hasLongSpace;

                if (i == 0)
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            obj.GetComponent<SpriteRenderer>().sprite = CurveLeft;
                            break;
                        case Direction.Right:
                            obj.GetComponent<SpriteRenderer>().sprite = CUrveRight;
                            break;
                    }

                    if (endPath)
                    {
                        obj.tag = "PathEnd";
                        Vector3 offset;
                        if (direction == Direction.Straight)
                        {
                            offset = new Vector3(0, ((length + (hasSpace ? 1 : 0)) * PATH_HEIGHT));
                        }
                        else if (direction == Direction.Left)
                        {
                            offset = new Vector3(-(length + (hasSpace ? 1 : 0)) * PATH_WIDTH, 0);
                        }
                        else
                        {
                            offset = new Vector3((length + (hasSpace ? 1 : 0)) * PATH_WIDTH, 0);
                        }
                        EndPosition = obj.transform.position + offset;
                    }
                }

                if (i == length - 1 && hasSpace)
                {
                    currentPath.NeedJump = true;
                    if (hasLongSpace)
                    {
                        currentPath.NeedLongJump = true;
                    }
                }
                else
                    currentPath.NeedJump = false;

                currentPath.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
}
