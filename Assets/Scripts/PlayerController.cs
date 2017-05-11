using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using System;

public struct Instruction
{
    public Direction direction;
    public bool needJump;
}

public class PlayerController : MonoBehaviour
{
    List<Collider2D> pathTriggers = new List<Collider2D>();
    Rigidbody2D rigidBody;
    new Camera camera;
    Animator animator;
    Transform spriteT;
    bool lockForJump = false;
    public GameObject level;
    public bool OnThePath
    {
        get
        {
            return OnMap || pathTriggers.Count > 0;
        }
    }
    public Camera Camera { get { if (camera == null) camera = Camera.main; return camera; } }
    public bool autoRun_Inspec = false;
    public bool inputReady = false;
    public static bool lose = false;
    public static bool autoRun = false;
    public static bool running = false;
    public static System.Action OnPerfectMove;
    public static System.Action OnPlayerLost;
    public static System.Action<Direction> OnPlayerDirectionChanged;
    public static System.Action<Path> OnDiamondPicked;

    void OnEnable()
    {

        UIManager.OnScreenTouched += OnScreenTouched;
        UIManager.OnBtnAutoTouched += OnBtnAutoTouched;
        MapGenerator.OnNewLevelLoaded += OnNewLevelLoaded;
    }

    private void OnBtnAutoTouched()
    {
        justUseAutoRun = true;
        DOVirtual.DelayedCall(1f, () => justUseAutoRun = false);

        if (pathTriggers.Count == 0)
        {
            GetComponent<Transform>().DOMove(lastPathPosition, .1f);
        }
        else
        {

            transform.DOMove(currentPath.transform.position, .1f);
        }
    }

    void OnDisable() { UIManager.OnScreenTouched -= OnScreenTouched; UIManager.OnBtnAutoTouched -= OnBtnAutoTouched; MapGenerator.OnNewLevelLoaded -= OnNewLevelLoaded; }

    private void OnNewLevelLoaded(int obj)
    {
        if (UIManager.AutoRunTween != null)
        {
            UIManager.AutoRunTween.Complete();
            UIManager.AutoRunTween = null;
        }
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteT = transform.FindChild("Sprite");
        running = false;
        lose = false;
        autoRun = false;
        inputReady = false;
        animator.enabled = false;

        StartCoroutine(GetInputReady());
    }
    IEnumerator GetInputReady() { yield return new WaitForSeconds(.5f); inputReady = true; }

    const float JUMP_DURATION = .8f;
    private const float PERFECT_MOVEE_THRESOLD = 0.025f;
    public float speed = 5;
    private bool spaceDown;

    private void OnScreenTouched()
    {
        if (lockForJump || lose || !inputReady)
        {
            return;
        }

        if (OnThePath)
        {
            spaceDown = true;
        }
    }

    void Update()
    {

        if (lockForJump || lose || !inputReady)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            spaceDown = true;

        if (autoRun)
        {
            if (pathTriggers.Count <= 0)
            {
                autoRun = false;
                return;
            }


            if (currentPath.NeedJump || currentPath.needAction)
            {
                float delta = GetDelta(currentPath);
                if (delta <= PERFECT_MOVEE_THRESOLD)
                    spaceDown = true;
            }
        }

        if (running && !OnThePath && !lose)
        {
            Lose();
        }
    }

    private float GetDelta(Path currentPath)
    {
        float delta = 0f;

        if (currentPath.Road.direction == Direction.Straight)
        {
            delta = Mathf.Abs(currentPath.transform.position.x - transform.position.x);
        }
        else if (currentPath.Road.direction == Direction.Left || currentPath.Road.direction == Direction.Right)
        {
            delta = Mathf.Abs(currentPath.transform.position.y - transform.position.y);
        }

        return delta;
    }

    private void Lose()
    {
        if (autoRun || OnThePath)
            return;

        lose = true;
        running = false;
        Debug.Log("LOSE!!!");
        SoundManager.PlayLoseSFX();
        DOVirtual.DelayedCall(.15f, () =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, 760), 1.5f, RotateMode.FastBeyond360);
            transform.DOScale(0f, 1.5f);
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;

            DOVirtual.DelayedCall(1.5f, () =>
            {
                if (OnPlayerLost != null)
                    OnPlayerLost();
            });
        });
    }

    bool justUseAutoRun = false;

    void FixedUpdate()
    {
        if (!spaceDown || lose)
            return;
        if (autoRun)
        {

            if (currentPath.NeedJump)
                SoundManager.PlayEffectSFX();
        }
        else
            SoundManager.PlayEffectSFX();

        spaceDown = false;
        Action();
    }

    private void Action()
    {
        if (pathTriggers.Count == 0)
        {
            return;
        }
        //Time.timeScale = Mathf.Min(1.2f, 1 + (0.01f * MapGenerator.CurrentLevel));

        Direction direction = Direction.NotChange;
        if (currentPath.GetDirection == Direction.NotChange && !OnMap)  // going to lose
        {
            if (currentPath.Road.direction == Direction.Straight)
            {
                direction = Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right;
            }
            else if (currentPath.Road.direction == Direction.Left || currentPath.Road.direction == Direction.Right)
            {
                direction = Direction.Straight;
            }
            justUseAutoRun = true;
        }
        else // proper move
        {
            animator.enabled = true;
            running = true;
            direction = currentPath.GetDirection;
        }

        if (OnMap)
        {
            direction = Direction.Straight;
        }

        float delta = GetDelta(currentPath);

        if (delta <= PERFECT_MOVEE_THRESOLD)
        {
            if (OnPerfectMove != null && !justUseAutoRun)
                OnPerfectMove();
        }
        else if (delta > .4f && OnThePath)
        {
            Lose();
            return;
        }

        Debug.Log("Delta: " + delta);

        bool needJump = currentPath.NeedJump;

        if (needJump)
        {
            rigidBody.velocity = Vector2.zero;
            Tweener tween = null;
            tween = spriteT.DOScale(1.5f, JUMP_DURATION * .5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
            tween.SetEase(Ease.InOutSine).OnStart(() => { lockForJump = true; animator.enabled = false; }).OnComplete(() => { lockForJump = false; animator.enabled = true; });
        }


        rigidBody.velocity = Vector2.zero;
        if (autoRun && needJump)
        {
            if(currentPath.NeedLongJump)
                speed = 1.25f;
            else
                speed = 1.1f;
        }
        else
            speed = 1.1f;

        const float rotateSpeed = .3f;
        switch (direction)
        {
            case Direction.Straight:
                rigidBody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
                transform.DOLocalRotate(new Vector3(0, 0, 0), JUMP_DURATION * rotateSpeed);
                Camera.transform.DOLocalRotate(new Vector3(0, 0, 0), JUMP_DURATION);
                break;
            case Direction.Left:
                rigidBody.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
                transform.DOLocalRotate(new Vector3(0, 0, 90), JUMP_DURATION * rotateSpeed);
                Camera.transform.DOLocalRotate(new Vector3(0, 0, 45), JUMP_DURATION);
                break;
            case Direction.Right:
                rigidBody.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
                transform.DOLocalRotate(new Vector3(0, 0, -90), JUMP_DURATION * rotateSpeed);
                Camera.transform.DOLocalRotate(new Vector3(0, 0, -45), JUMP_DURATION);
                break;
        }

        if (OnPlayerDirectionChanged != null)
        {
            OnPlayerDirectionChanged(direction);
        }
    }


    Path currentPath;
    void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.gameObject.tag == "Map")
        {
            OnMap = true;
        }

        if (Other.gameObject.tag == "Path" || Other.gameObject.tag == "PathEnd")
        {
            pathTriggers.Clear();
            //add the object to the list
            pathTriggers.Add(Other);

            currentPath = Other.GetComponent<Path>();
            //take the diamond from path
            Transform diamond = Other.gameObject.transform.FindChild("Diamond");
            if (diamond)
            {
                diamond.gameObject.SetActive(false);

                if (OnDiamondPicked != null)
                {
                    OnDiamondPicked(currentPath);
                }
            }

            if (Other.gameObject.tag == "PathEnd")
            {
                requireAction = true;
            }
        }
    }
    bool OnMap = false;
    Vector3 lastPathPosition;
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D Other)
    {
        if (Other.gameObject.tag == "Map")
        {
            OnMap = false;
        }

        if (Other.gameObject.tag == "Path" || Other.gameObject.tag == "PathEnd")
        {
            requireAction = false;
            Path currentPath = Other.GetComponent<Path>();
            lastPathPosition = currentPath.transform.position;
            //remove it from the list
            pathTriggers.Remove(Other);

            if (Other.gameObject.tag == "PathEnd")
            {
                Instantiate(level, currentPath.Road.Map.EndPosition, Quaternion.identity);
            }
        }
    }

    private bool requireAction;

    public Direction convertIntToDirection(int i)
    {
        switch (i)
        {
            case 0:
                return Direction.Straight;
            case 1:
                return Direction.Left;
            case 2:
                return Direction.Right;
        }

        return Direction.Straight;
    }
}
