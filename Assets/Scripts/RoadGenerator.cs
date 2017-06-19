using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace EndlessRun
{
    public enum Direction
    {
        Straight,
        Left,
        Right,
        NotChange
    }

    public class RoadGenerator : MonoBehaviour
    {
        public const float LAND_HEIGHT = 1f;
        public const float PATH_WIDTH = 3.8f;
        public const float PATH_HEIGHT = 3.8f;

        [HideInInspector]
        public MapGenerator Map;
        [SerializeField]
        private GameObject start;
        [SerializeField]
        private GameObject path;
        [SerializeField]
        private GameObject end;
        [SerializeField]
        private GameObject CurveLeft;
        [SerializeField]
        private GameObject CurveRight;

        [SerializeField]
        private Sprite DarkArrow;
        [SerializeField]
        private Sprite LightArrow;

        public Direction direction;
        public Vector3 EndPosition;
        public int length;

        public void Generate(Direction lastDir, bool newLevel = false, bool endPath = false)
        { 

            GameObject obj = null;
            for (int i = 0; i < length; i++)
            {
                if (direction == Direction.Straight)
                {
                    if (i == 0)
                    {
                        obj = (GameObject)Instantiate(start, transform.localPosition + new Vector3(0, 0, i * PATH_HEIGHT), Quaternion.identity);
                    }
                    else if (i == length - 1)
                    {
                        obj = (GameObject)Instantiate(end, transform.localPosition + new Vector3(0, 0, i * PATH_HEIGHT), Quaternion.identity);
                    }
                    else
                    {
                        obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(0, 0, i * PATH_HEIGHT), Quaternion.identity);
                    }
                }
                else if (direction == Direction.Left)
                {
                    if (i == 0)
                    {
                        if (lastDir == Direction.Straight)
                        {
                            obj = (GameObject)Instantiate(CurveLeft, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0, 0), Quaternion.identity);
                        }
                        else
                            obj = (GameObject)Instantiate(start, transform.localPosition + new Vector3(-i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    }
                    else if (i == length - 1)
                    {
                        obj = (GameObject)Instantiate(end, transform.localPosition + new Vector3(-i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    }
                    else
                    {
                        obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(-i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    }
                    obj.transform.localRotation = Quaternion.Euler(0, -90, 0);
                }
                else if (direction == Direction.Right)
                {
                    if (i == 0)
                    {
                        if (lastDir == Direction.Straight)
                        {
                            obj = (GameObject)Instantiate(CurveRight, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0, 0), Quaternion.identity);
                        }
                        else
                            obj = (GameObject)Instantiate(start, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    }
                    else if (i == 1) continue;
                    else if (i == length - 1)
                    {
                        obj = (GameObject)Instantiate(end, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    }
                    else
                        obj = (GameObject)Instantiate(path, transform.localPosition + new Vector3(+i * PATH_WIDTH, 0, 0), Quaternion.identity);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                obj.transform.parent = transform;
                obj.transform.position = new Vector3(obj.transform.position.x, LAND_HEIGHT, obj.transform.position.z);
                Path currentPath = obj.GetComponent<Path>();
                if (currentPath == null)
                {
                    currentPath = obj.AddComponent<Path>();
                }
                currentPath.Road = this;
                currentPath.GetDirection = Direction.NotChange;

                if ((i == 0) || (i == length - 1 && !endPath))
                {
                    currentPath.needAction = true;
                    currentPath.GetDirection = direction;

                    if (i == 0)
                    {
                        if (endPath)
                        {
                            obj.tag = "PathEnd";
                            Vector3 offset;
                            if (direction == Direction.Straight)
                            {
                                offset = new Vector3(0, 0, ((length) * PATH_HEIGHT));
                            }
                            else if (direction == Direction.Left)
                            {
                                offset = new Vector3(-(length) * PATH_WIDTH, 0, 0);
                            }
                            else
                            {
                                offset = new Vector3((length) * PATH_WIDTH, 0, 0);
                            }
                            EndPosition = obj.transform.position + offset;
                        }
                    }
                }
            }
        }
    }
}