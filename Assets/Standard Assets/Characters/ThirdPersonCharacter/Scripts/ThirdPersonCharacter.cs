using UnityEngine;
using System.Collections;

namespace UnitySampleAssets.Characters.ThirdPerson
{
    public class ThirdPersonCharacter : MonoBehaviour
    {

        [SerializeField] private float jumpPower = 12; // determines the jump force applied when jumping (and therefore the jump height)
        [SerializeField] private float airSpeed = 6; // determines the max speed of the character while airborne
        [SerializeField] private float airControl = 2; // determines the response speed of controlling the character while airborne
        [Range(1, 4)] [SerializeField] public float gravityMultiplier = 2; // gravity modifier - often higher than natural gravity feels right for game characters
        [SerializeField] [Range(0.1f, 3f)] private float moveSpeedMultiplier = 1; // how much the move speed of the character will be multiplied by
        [SerializeField] [Range(0.1f, 3f)] private float animSpeedMultiplier = 1; // how much the animation of the character will be multiplied by
        [SerializeField] private AdvancedSettings advancedSettings; // Container for the advanced settings class , thiss allows the advanced settings to be in a foldout in the inspector


        [System.Serializable]
        public class AdvancedSettings
        {
            public float stationaryTurnSpeed = 180; // additional turn speed added when the player is stationary (added to animation root rotation)
            public float movingTurnSpeed = 360; // additional turn speed added when the player is moving (added to animation root rotation)
            public float headLookResponseSpeed = 2; // speed at which head look follows its target
            public float crouchHeightFactor = 0.6f; // collider height is multiplied by this when crouching
            public float crouchChangeSpeed = 4; // speed at which capsule changes height when crouching/standing
            public float autoTurnThresholdAngle = 100; // character auto turns towards camera direction if facing away by more than this angle
            public float autoTurnSpeed = 2; // speed at which character auto-turns towards cam direction
            public PhysicMaterial zeroFrictionMaterial; // used when in motion to enable smooth movement
            public PhysicMaterial highFrictionMaterial; // used when stationary to avoid sliding down slopes
            public float jumpRepeatDelayTime = 0.25f; // amount of time that must elapse between landing and being able to jump again
            public float runCycleLegOffset = 0.2f; // animation cycle offset (0-1) used for determining correct leg to jump off
            public float groundStickyEffect = 5f; // power of 'stick to ground' effect - prevents bumping down slopes.
        }

        public Transform lookTarget { get; set; } // The point where the character will be looking at

        public LayerMask groundCheckMask;
        public LayerMask crouchCheckMask;
        public Animator animator; // The animator for the character

        private bool onGround; // Is the character on the ground
        private Vector3 currentLookPos; // The current position where the character is looking
        private float originalHeight; // Used for tracking the original height of the characters capsule collider
        private float lastAirTime; // USed for checking when the character was last in the air for controlling jumps
        private CapsuleCollider capsule; // The collider for the character
        private const float half = 0.5f; // whats it says, it's a constant for a half
        private Vector3 moveInput;
        private bool crouchInput;
        private bool jumpInput;
        private float turnAmount;
        private float forwardAmount;
        private Vector3 velocity;
        private IComparer rayHitComparer;
        public float lookBlendTime;
        public float lookWeight;

        // Use this for initialization
        private void Start()
        {
            capsule = GetComponent<Collider>() as CapsuleCollider;

            // as can return null so we need to make sure thats its not before assigning to it
            if (capsule == null)
            {
                Debug.LogError(" collider cannot be cast to CapsuleCollider");
            }
            else
            {
                originalHeight = capsule.height;
                capsule.center = Vector3.up*originalHeight*half;
            }

            rayHitComparer = new RayHitComparer(); 

            // give the look position a default in case the character is not under control
            currentLookPos = Camera.main.transform.position;
        }

        IEnumerator BlendLookWeight()
        {
            float t = 0f;
            while (t < lookBlendTime)
            {
                lookWeight = t / lookBlendTime;
                t += Time.deltaTime;
                yield return null;
            }
            lookWeight = 1f;
        }

        void OnEnable()
        {
            if (lookWeight == 0f)
            {
                StartCoroutine(BlendLookWeight());
            }
        }
        // The Move function is designed to be called from a separate component
        // based on User input, or an AI control script
        public void Move(Vector3 move, float v, float h, bool crouch, bool jump, Vector3 lookPos)
        {
            if (move.magnitude > 1) move.Normalize();
            this.moveInput = move;
            this.crouchInput = crouch;
            this.jumpInput = jump;
            this.currentLookPos = lookPos;
            velocity = GetComponent<Rigidbody>().velocity;
            GroundCheck(); 
            SetFriction();
            if (onGround && jumpInput && !crouchInput)
            {
                // jump!
                onGround = false;
                velocity.y = jumpPower;
            }
            animator.SetFloat("vSpeed", v);
            animator.SetFloat("hSpeed", h);
            animator.SetBool("onGround", onGround);
            velocity.x = h;
            velocity.z = v;
            GetComponent<Rigidbody>().velocity = velocity; 
        } 

        private void GroundCheck()
        {
            Ray ray = new Ray(transform.position + Vector3.up*.1f, -Vector3.up);
            RaycastHit[] hits = Physics.RaycastAll(ray, .5f,groundCheckMask);
            System.Array.Sort(hits, rayHitComparer);

            if (velocity.y < jumpPower*.5f)
            {
                onGround = false;
                GetComponent<Rigidbody>().useGravity = true;
                foreach (var hit in hits)
                {
                    // check whether we hit a non-trigger collider (and not the character itself)
                    if (!hit.collider.isTrigger)
                    {
                        // this counts as being on ground.

                        // stick to surface - helps character stick to ground - specially when running down slopes
                        if (velocity.y <= 0)
                        {
                            GetComponent<Rigidbody>().position = Vector3.MoveTowards(GetComponent<Rigidbody>().position, hit.point,
                                                                     Time.deltaTime*advancedSettings.groundStickyEffect);
                        }

                        onGround = true;
                        GetComponent<Rigidbody>().useGravity = false;
                        break;
                    }
                }
            }

            // remember when we were last in air, for jump delay
            if (!onGround) lastAirTime = Time.time;

        }
        private void SetFriction()
        {

            if (onGround)
            {
                // set friction to low or high, depending on if we're moving
                if (moveInput.magnitude == 0)
                {
                    // when not moving this helps prevent sliding on slopes:
                    GetComponent<Collider>().material = advancedSettings.highFrictionMaterial;
                }
                else
                {
                    // but when moving, we want no friction:
                    GetComponent<Collider>().material = advancedSettings.zeroFrictionMaterial;
                }
            }
            else
            {
                // while in air, we want no friction against surfaces (walls, ceilings, etc)
                GetComponent<Collider>().material = advancedSettings.zeroFrictionMaterial;
            }
        }  


        void OnDisable()
        {
            lookWeight = 0f;
        }

        //used for comparing distances
        private class RayHitComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((RaycastHit) x).distance.CompareTo(((RaycastHit) y).distance);
            }
        }
    }
}