using Photon.Pun;
using UnityEngine;
public class PlayerController : MonoBehaviourPun
{
    private CharacterController m_characterController;
    private float m_speed;
    private float maxSpeed = 2.5f;
    private const float staticSpeed = 2.5f;
    public bool stopMovement;
    private float rotationSpeed = 30;
    public Vector3 normalized_input
    {
        get; private set;
    }
    public Transform camera { get; private set; }

    public Transform target;
    #region Animator
    [SerializeField]
    private Transform movementLink;
    private Animator m_animator;
    private float animatorForward;
    #endregion

    public PlayerManager playerManager
    {
        get => GetComponent<PlayerManager>();
    }

    public void SlowPlayer(int percentage)
    {
        float newspeed = maxSpeed -  maxSpeed *percentage / 100;
        Debug.Log(newspeed);
        maxSpeed = newspeed;
    }
    public void ResetSlow()
    {
        maxSpeed = staticSpeed;
    }
    private void Start()
    {
        ActivateBehaviour();
    }
    public void StopMovement(bool stopmovement)
    {
        stopMovement = stopmovement;
        if (stopMovement)
        {
            rotationSpeed = 0;
            animatorForward = 0;
            m_speed = 0;
        }
        else if (!stopmovement)
        {
            rotationSpeed = 30;
        }
    }
    public void CameraSet()
    {
        camera = Camera.main.transform;

        CharacterSelect.instance.CameraSet((int)playerManager.playerTeam, transform);

    }

    public void ActivateBehaviour()
    {
        m_characterController = GetComponent<CharacterController>();

        m_animator = GetComponentInChildren<Animator>();

        movementLink = transform.Find("MovementLink");

        if (photonView.IsMine)
        {
            Physics.autoSyncTransforms = true;
            CameraSet();
        }
    }
    public void PlayerForward()
    {
        if (!target)
            if (normalized_input != Vector3.zero && !stopMovement)
                transform.forward = Math.dampVector3(transform.forward, normalized_input, rotationSpeed, Time.deltaTime);
            else if (target)
            {
                Vector3 ab = Math.vectorABXZ(transform.forward, target.position);
                transform.forward = Math.dampVector3(transform.forward, ab, rotationSpeed, Time.deltaTime);
            }
    }
    public void PlayerMovementInput()
    {
        float y = SimpleInput.GetAxis("Horizontal");
        float x = SimpleInput.GetAxis("Vertical");
        Vector3 direction = new Vector3(y, 0, x).normalized;
        if (camera)
            direction = camera.forward * x + camera.right * y;

        direction.y = 0;

        direction = direction.normalized;

        normalized_input = direction;

        movementLink.localPosition = normalized_input;
    }
    public void MotionUpdate()
    {

        if (normalized_input != Vector3.zero && !stopMovement) { PlayerMovementAcceleration(); }
        else if (normalized_input == Vector3.zero) { PlayerMovementDeceleration(); }

            m_animator.SetFloat("Forward", animatorForward);

        if (!stopMovement)
        {
            m_characterController.Move(normalized_input * m_speed * Time.fixedDeltaTime);
        }
    }
    private void PlayerMovementAcceleration()
    {
        m_speed = Math.dampFloat(m_speed, maxSpeed, 15f, Time.fixedDeltaTime);
        animatorForward = Math.dampFloat(animatorForward, 0.9f, maxSpeed * 10f, Time.fixedDeltaTime);
    }
    private void PlayerMovementDeceleration()
    {
        m_speed = Math.dampFloat(m_speed, 0, 5f, Time.fixedDeltaTime);
        animatorForward = Math.dampFloat(animatorForward, 0, maxSpeed * 5f, Time.fixedDeltaTime);
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerMovementInput();
        }
        else if (!photonView.IsMine) AnimationSync();
    }
    private void AnimationSync()
    {
        if (movementLink.localPosition != Vector3.zero)
            animatorForward = Math.dampFloat(animatorForward, 0.9f, maxSpeed * 10f, Time.fixedDeltaTime);

        else if (movementLink.localPosition == Vector3.zero)
            animatorForward = Math.dampFloat(animatorForward, 0, maxSpeed * 5f, Time.fixedDeltaTime);

        m_animator.SetFloat("Forward", animatorForward);
    }
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            PlayerForward();
            MotionUpdate();
        }
    }

}
