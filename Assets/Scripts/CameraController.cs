using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private const float NEW_LEVEL_SOUND_DURATION = 1.2f;
    public float dampTime = 0.15f;
    public Vector3 offset;
    public Vector3 velocity = Vector3.zero;
    public Transform target;
    private new Camera camera;

    Color nextColor;
    public static Color currentColor;
    private static bool FirstTime = true;
    void Start()
    {
        camera = GetComponent<Camera>();
        if (FirstTime)
            currentColor = camera.backgroundColor;
        FirstTime = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 point = camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta + offset;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }

    }

    void OnEnable()
    {
        PlayerController.OnPlayerDirectionChanged += CameraRotatedHandler;
        MapGenerator.OnNewLevelLoaded += OnNewLevelLoaded;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerDirectionChanged -= CameraRotatedHandler;
        MapGenerator.OnNewLevelLoaded -= OnNewLevelLoaded;
    }


    private void OnNewLevelLoaded(int level)
    {
        SoundManager.PlayEffectSFX(NEW_LEVEL_SOUND_DURATION);
        nextColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        DOTween.To(() => Camera.main.backgroundColor, t => Camera.main.backgroundColor = t, nextColor, 2f);
    }


    void CameraRotatedHandler(Direction direction)
    {
        switch (direction)
        {
            case Direction.Straight:
                offset = new Vector3(0, 2, 0);
                break;
            case Direction.Left:
                offset = new Vector3(0, 0, 0);
                break;
            case Direction.Right:
                offset = new Vector3(0, 0, 0);
                break;
        }
    }
}

