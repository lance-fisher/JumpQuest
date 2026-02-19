using UnityEngine;
using JumpQuest.Core;

namespace JumpQuest.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float WalkSpeed = 6f;
        public float RunSpeed = 10f;
        public float RotationSpeed = 720f;
        public float Gravity = -30f;

        [Header("Jump")]
        public float JumpForce = 12f;
        public float DoubleJumpForce = 10f;
        public float CoyoteTime = 0.15f;
        public float JumpBufferTime = 0.12f;

        [Header("Speed Burst Power-Up")]
        public float SpeedBurstMultiplier = 1.6f;
        public float SpeedBurstDuration = 3f;
        public float SpeedBurstCooldown = 10f;

        [Header("Shield Power-Up")]
        public float ShieldDuration = 2.5f;
        public float ShieldCooldown = 15f;

        private CharacterController cc;
        private Vector3 velocity;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private bool hasUsedDoubleJump;
        private bool isRunning;

        // Power-up timers
        private float speedBurstTimer;
        private float speedBurstCooldownTimer;
        private float shieldTimer;
        private float shieldCooldownTimer;

        // Input (set by VirtualJoystick and buttons, or keyboard)
        public Vector2 MoveInput { get; set; }
        public bool JumpRequested { get; set; }
        public bool RunHeld { get; set; }

        // Public state for HUD / other systems
        public bool IsGrounded => cc != null && cc.isGrounded;
        public bool IsShieldActive => shieldTimer > 0f;
        public bool IsSpeedBurstActive => speedBurstTimer > 0f;
        public bool CanUseSpeedBurst => GameManager.Instance != null && GameManager.Instance.HasSpeedBurst && speedBurstCooldownTimer <= 0f;
        public bool CanUseShield => GameManager.Instance != null && GameManager.Instance.HasShield && shieldCooldownTimer <= 0f;

        private Transform cameraTransform;
        private Vector3 respawnPosition;

        private void Start()
        {
            cc = GetComponent<CharacterController>();
            cameraTransform = Camera.main?.transform;
            respawnPosition = transform.position;
        }

        private void Update()
        {
            ReadKeyboardInput();
            HandleTimers();
            HandleMovement();
            HandleJump();
            HandleFallReset();
        }

        private void ReadKeyboardInput()
        {
            // Keyboard fallback for dev testing
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
                MoveInput = new Vector2(h, v);

            if (Input.GetKeyDown(KeyCode.Space))
                JumpRequested = true;

            if (Input.GetKey(KeyCode.LeftShift))
                RunHeld = true;
        }

        private void HandleTimers()
        {
            float dt = Time.deltaTime;

            if (speedBurstTimer > 0f) speedBurstTimer -= dt;
            if (speedBurstCooldownTimer > 0f) speedBurstCooldownTimer -= dt;
            if (shieldTimer > 0f) shieldTimer -= dt;
            if (shieldCooldownTimer > 0f) shieldCooldownTimer -= dt;

            if (cc.isGrounded)
            {
                coyoteTimer = CoyoteTime;
                hasUsedDoubleJump = false;
            }
            else
            {
                coyoteTimer -= dt;
            }

            if (jumpBufferTimer > 0f)
                jumpBufferTimer -= dt;
        }

        private void HandleMovement()
        {
            Vector3 inputDir = new Vector3(MoveInput.x, 0f, MoveInput.y).normalized;

            if (inputDir.magnitude > 0.01f && cameraTransform != null)
            {
                // Move relative to camera forward (flattened)
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                Vector3 camRight = cameraTransform.right;
                camRight.y = 0f;
                camRight.Normalize();

                Vector3 worldDir = camForward * inputDir.z + camRight * inputDir.x;

                // Rotate toward movement direction
                Quaternion targetRot = Quaternion.LookRotation(worldDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);

                float speed = RunHeld ? RunSpeed : WalkSpeed;
                if (IsSpeedBurstActive)
                    speed *= SpeedBurstMultiplier;

                Vector3 move = worldDir * speed;
                velocity.x = move.x;
                velocity.z = move.z;
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }

            // Gravity
            if (cc.isGrounded && velocity.y < 0f)
                velocity.y = -2f; // small downward to keep grounded
            else
                velocity.y += Gravity * Time.deltaTime;

            cc.Move(velocity * Time.deltaTime);

            // Reset inputs each frame (touch sets them continuously)
            // Keyboard is read fresh each frame above
        }

        private void HandleJump()
        {
            if (JumpRequested)
            {
                jumpBufferTimer = JumpBufferTime;
                JumpRequested = false;
            }

            if (jumpBufferTimer > 0f)
            {
                // Grounded or coyote: normal jump
                if (coyoteTimer > 0f)
                {
                    velocity.y = JumpForce;
                    coyoteTimer = 0f;
                    jumpBufferTimer = 0f;
                }
                // Double jump
                else if (!hasUsedDoubleJump && GameManager.Instance != null && GameManager.Instance.HasDoubleJump)
                {
                    velocity.y = DoubleJumpForce;
                    hasUsedDoubleJump = true;
                    jumpBufferTimer = 0f;
                }
            }
        }

        private void HandleFallReset()
        {
            if (transform.position.y < -20f)
            {
                cc.enabled = false;
                transform.position = respawnPosition;
                velocity = Vector3.zero;
                cc.enabled = true;
            }
        }

        public void SetCheckpoint(Vector3 pos)
        {
            respawnPosition = pos;
        }

        // Called by JumpPad via SendMessage
        private void ApplyLaunch(float force)
        {
            velocity.y = force;
            coyoteTimer = 0f;
            hasUsedDoubleJump = false;
        }

        public void ActivateSpeedBurst()
        {
            if (!CanUseSpeedBurst) return;
            speedBurstTimer = SpeedBurstDuration;
            speedBurstCooldownTimer = SpeedBurstCooldown;
        }

        public void ActivateShield()
        {
            if (!CanUseShield) return;
            shieldTimer = ShieldDuration;
            shieldCooldownTimer = ShieldCooldown;
        }

        public void TakeHit()
        {
            if (IsShieldActive) return; // shield absorbs it
            // Reset to checkpoint
            cc.enabled = false;
            transform.position = respawnPosition;
            velocity = Vector3.zero;
            cc.enabled = true;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Hazard"))
            {
                TakeHit();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hazard"))
            {
                TakeHit();
            }
        }
    }
}
