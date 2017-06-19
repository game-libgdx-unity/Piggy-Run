using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RobotDemo;
using UnitySampleAssets.Cameras;

namespace EndlessRun
{
    public class MapGenerator : MonoBehaviour
    {
        public GameObject roadPrefap;
        public GameObject playerPrefab;

        public int NumberOfRoad;
        Direction nextDirection;
        int nextLength;
        Vector3 nextPostiton;
        public Vector3 EndPosition;
        bool firstRoad = true;
        public static GameObject player;
        public static int CurrentLevel = 1;
        public static List<MapGenerator> Maps = new List<MapGenerator>();
        public static float CurrentTimeScale { get { return 1f/*Mathf.Max(.9f, 1f - CurrentLevel * .005f);*/; } }

        public Direction NextDirection { get { return nextDirection; } set { if (nextDirection != value) { nextDirection = value; } } }

        void Awake() { if (CurrentLevel == 1) CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", CurrentLevel); }
        void Start()
        {
            nextPostiton = transform.position;
            //Debug.Log(CurrentLevel);
            Time.timeScale = CurrentTimeScale;
            NumberOfRoad += Hardness();
            //create level
            for (int i = 0; i < NumberOfRoad; i++)
            {
                RoadGenerator road = nextRoad(i == 0, i == NumberOfRoad - 1);
                road.transform.parent = this.transform;

                if (i == NumberOfRoad - 1)
                {
                    EndPosition = road.EndPosition;
                }
            }
            Maps.Add(this);
            //destroy old level
            if (Maps.Count > 2)
            {
                Destroy(Maps[0].gameObject);
                Maps.RemoveAt(0);
            }
            //create player
            StartCoroutine(CreatePlayer());
            //call the event
            if (OnNewLevelLoaded != null)
            {
                OnNewLevelLoaded(CurrentLevel);
            }
            CurrentLevel++;
            Debug.Log("Map ready");
        }

        private IEnumerator CreatePlayer()
        {
            yield return new WaitForSeconds(.5f);
            if (!player && playerPrefab)
            {
                player = Instantiate(playerPrefab, Vector3.up * RoadGenerator.LAND_HEIGHT, Quaternion.identity);
                AutoCam camera = GameObject.Find("MultipurposeCameraRig").GetComponent<AutoCam>();
                camera.SetTarget(player.transform);
            }
        }

        private static int Hardness()
        {
            return (int)(CurrentLevel * .1f);
        }

        public static System.Action<int> OnNewLevelLoaded;
        private Direction lastDir = Direction.Straight;
        private int space;

        // Update is called once per frame
        RoadGenerator nextRoad(bool newLevel = false, bool endPath = false)
        {
            if (firstRoad)
            {
                NextDirection = Direction.Straight;
                firstRoad = false;
            }


            //nextLength = Random.Range((newLevel || endPath) ? (CurrentLevel < 5 ? 8 : 5) : (CurrentLevel < 5 ? 4 : 3), CurrentLevel < 5 ? 10 : 5); // random length
            nextLength = Random.Range(15, 20);
            GameObject obj = (GameObject)Instantiate(roadPrefap, nextPostiton, Quaternion.identity); //get the road
            RoadGenerator road = obj.GetComponent<RoadGenerator>();
            road.Map = this;
            lastDir = road.direction;
            road.direction = NextDirection;
            road.length = nextLength;
            space = 0;
            road.Generate(lastDir, newLevel, endPath);
            calculateNextPosition();
            setNextDirection();
            obj.tag = NextDirection.ToString();

            return road;
        }

        private Direction setNextDirection()
        {
            if (NextDirection == Direction.Straight)
            {
                int next = (Random.Range(1, 3));
                NextDirection = convertIntToDirection(next);
                return NextDirection;
            }
            else
            {
                NextDirection = Direction.Straight;
                return NextDirection;
            }
        }

        private void calculateNextPosition()
        {
            switch (NextDirection)
            {
                case Direction.Straight:
                    nextPostiton.Set(nextPostiton.x, 0, nextPostiton.z + RoadGenerator.PATH_HEIGHT * (nextLength + space));
                    break;
                case Direction.Left:
                    nextPostiton.Set(nextPostiton.x - RoadGenerator.PATH_WIDTH * (nextLength + space), 0, nextPostiton.z);
                    break;
                case Direction.Right:
                    nextPostiton.Set(nextPostiton.x + RoadGenerator.PATH_WIDTH * (nextLength + space), 0, nextPostiton.z);
                    break;
            }
            //nextPostiton += new Vector3(-1.9f, 0, -2.8f);

            if (nextDirection == Direction.Straight)
                nextPostiton += new Vector3(-1.9f, 0, -2.8f);
            else
                nextPostiton += new Vector3(-1.9f, 0, 1.6f);
        }

        public Direction convertIntToDirection(int i)
        {
            switch (i)
            {
                case 0:
                    return Direction.Straight;
                case 1:
                    if (NextDirection == Direction.Right)
                    {
                        int random = Random.Range(0, 3);
                        return convertIntToDirection(random);
                    }
                    return Direction.Left;
                case 2:
                    if (NextDirection == Direction.Left)
                    {
                        int random = Random.Range(0, 3);
                        return convertIntToDirection(random);
                    }
                    return Direction.Right;
            }

            return Direction.Straight;
        }
    }
}