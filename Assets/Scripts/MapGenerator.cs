using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject roadPrefap;
    public int NumberOfRoad;
    Direction nextDirection;
    int nextLength;
    Vector3 nextPostiton;
    public Vector3 EndPosition;
    bool hasSpace;
    bool firstRoad = true;
    public static int CurrentLevel = 1;
    public static List<MapGenerator> Maps = new List<MapGenerator>();
    public static float CurrentTimeScale { get { return 1f/*Mathf.Max(.9f, 1f - CurrentLevel * .005f);*/; } }
    void Awake() { if (CurrentLevel == 1) CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", CurrentLevel); }
    void Start()
    {
        CurrentLevel = 20;

        nextPostiton = transform.position;
        //Debug.Log(CurrentLevel);
        Time.timeScale = CurrentTimeScale;

        NumberOfRoad += Hardness();

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
        if (Maps.Count > 2)
        {
            Destroy(Maps[0].gameObject);
            Maps.RemoveAt(0);
        }



        if (OnNewLevelLoaded != null)
        {
            OnNewLevelLoaded(CurrentLevel);
        }
        CurrentLevel++;


        Debug.Log("Map ready");

        SoundManager.PlayBGMusic(true);
    }

    private static int Hardness()
    {
        return (int)(CurrentLevel * .3f);
    }

    public static System.Action<int> OnNewLevelLoaded;

    // Update is called once per frame
    RoadGenerator nextRoad(bool newLevel = false, bool endPath = false)
    {
        if (firstRoad)
        {
            nextDirection = Direction.Straight;
            firstRoad = false;
        }
        int space = 2 + Hardness();
        hasSpace = Random.Range(0, space) < 2 ? false : true; //random hasSpace
        nextLength = Random.Range((newLevel || endPath) ? (CurrentLevel < 5 ? 8 : 5) : (CurrentLevel < 5 ? 4 : 3), CurrentLevel < 5 ? 10 : 5); // random length

        GameObject obj = (GameObject)Instantiate(roadPrefap, nextPostiton, Quaternion.identity); //get the road
        RoadGenerator road = obj.GetComponent<RoadGenerator>();
        bool longSpace = hasSpace ? Random.Range(0, 2) == 0 : false;
        road.Map = this;
        road.direction = nextDirection;
        road.length = nextLength;
        calculateNextPosition(longSpace);
        road.Generate(hasSpace, longSpace, endPath, newLevel);

        //next spawm position
        int changeDirectionRate = 2 + Hardness();
        if (Random.Range(0, changeDirectionRate) == 0)
            nextDirection = convertIntToDirection(Random.Range(0, 3)); // random direction
        else
        {
            int next = (Random.Range(0, 3));
            while (next == (int)nextDirection)
            {
                next = (Random.Range(0, 3));
            }
            nextDirection = convertIntToDirection(next);
        }

        //nextDirection = convertIntToDirection(Random.Range(0, 3)); // random direction
        obj.tag = nextDirection.ToString();

        return road;
    }

    void calculateNextPosition(bool longSpace)
    {
        float space = hasSpace ? (longSpace ? 1.5f : 1f) : 0f;

        switch (nextDirection)
        {
            case Direction.Straight:
                nextPostiton.Set(nextPostiton.x, nextPostiton.y + RoadGenerator.PATH_HEIGHT * (nextLength + space), 0);
                break;
            case Direction.Left:
                nextPostiton.Set(nextPostiton.x - RoadGenerator.PATH_WIDTH * (nextLength + space), nextPostiton.y, 0);
                break;
            case Direction.Right:
                nextPostiton.Set(nextPostiton.x + RoadGenerator.PATH_WIDTH * (nextLength + space), nextPostiton.y, 0);
                break;
        }
    }

    public Direction convertIntToDirection(int i)
    {
        switch (i)
        {
            case 0:
                return Direction.Straight;
            case 1:
                if (nextDirection == Direction.Right)
                {
                    int random = Random.Range(0, 3);
                    return convertIntToDirection(random);
                }
                return Direction.Left;
            case 2:
                if (nextDirection == Direction.Left)
                {
                    int random = Random.Range(0, 3);
                    return convertIntToDirection(random);
                }
                return Direction.Right;
        }

        return Direction.Straight;
    }
}
