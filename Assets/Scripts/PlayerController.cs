using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController characterController;
    
    [Header("Camera Settings")]
    private GameObject cam;
    [SerializeField] private float fovCamera = 20, smoothTime = 0.1f;
    private float defaultFovCamera;
    private Vector2 currentLook, currentLookVelocity;

    [Header("Sensitivity Settings")]
    [SerializeField] private float mouseSensitivity = 50;

    [Header("Movement Settings")]
    private float xRotation = 0f;

    private float speed;
    [SerializeField] private float defaultSpeed = 5f, sprintAcceleration = 5f, maxSprintSpeed = 1.5f, jumpForce = 10f;

    [SerializeField] private float slideSpeed = 10f, slideDuration = 1f, cooldownSlideTime = 0f, slideTime = 0f, slideCooldown = 0.5f;
    [SerializeField] private float sprintDuration = 1f, cooldownSprintTime = 0f, sprintTime = 0f, sprintCooldown = 0.5f;
    private bool isSliding = false, isSprinting = false, isMoving = false;
    private Vector3 direcctionToSlide;

    private Vector3 velocity; // Para manejar la gravedad y el salto
    [SerializeField] private float gravity = -9.81f;

    private void Awake() { controls = new PlayerControls(); }
    private void OnEnable() { controls.Enable(); }
    private void OnDisable() { controls.Disable(); }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = this.gameObject.GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        defaultFovCamera = cam.GetComponent<Camera>().fieldOfView;
        speed = defaultSpeed;
    }
    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        JumpLogic();
        SprintLogic();
        SlideLogic();
        AttackLogic();
    }
    private void HandleMouseLook()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();
        look *= mouseSensitivity * Time.deltaTime;

        currentLook = Vector2.SmoothDamp(currentLook, look, ref currentLookVelocity, smoothTime * 0.75f);

        xRotation -= currentLook.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * currentLook.x);
    }
    private void HandleMovement()
    {
        Vector2 move = controls.Player.Move.ReadValue<Vector2>();
        if (move.magnitude > 0)
            isMoving = true;
        else
            isMoving = false;
        Vector3 moveDirection = transform.right * move.x + transform.forward * move.y;
        if (!isSliding)
            characterController.Move(moveDirection * speed * Time.deltaTime);
    }
    private void JumpLogic()
    {

    }
    private void SprintLogic()
    {
        float sprintSpeed = defaultSpeed * maxSprintSpeed;
        if (controls.Player.Sprint.WasPressedThisFrame() && !isSprinting && cooldownSprintTime <= 0f && isMoving)
            StartSprinting();

        if (isSprinting)
        {
            sprintTime += Time.deltaTime;
            if (sprintTime >= sprintDuration)
                StopSprinting();
            else
            {
                isSprinting = true;
                float fovSprintCamera = fovCamera * .5f;
                cam.GetComponent<Camera>().fieldOfView = defaultFovCamera + fovSprintCamera;
                speed = Mathf.MoveTowards(speed, sprintSpeed, sprintAcceleration * Time.deltaTime);
            }
        }
        else
        {
            if (cooldownSprintTime > 0f)
                cooldownSprintTime -= Time.deltaTime;
        }
    }
    private void StartSprinting()
    {
        isSprinting = true;
        sprintTime = 0f;
        cooldownSprintTime = sprintCooldown;

        // Aquí puedes activar la animación del deslizamiento si tienes alguna
        // Ejemplo: animator.SetTrigger("Slide");

        // Opcional: aplicar efectos de sonido o partículas si lo deseas.
    }
    private void StopSprinting()
    {
        isSprinting = false;
        cam.GetComponent<Camera>().fieldOfView = defaultFovCamera;
        speed = Mathf.MoveTowards(speed, defaultSpeed, sprintAcceleration * Time.deltaTime);

        // Aquí puedes detener la animación del deslizamiento
        // Ejemplo: animator.ResetTrigger("Slide");

        // Opcional: aplicar efectos de sonido o partículas al terminar el deslizamiento.
    }
    private void SlideLogic()
    {
        if (controls.Player.Slide.WasPressedThisFrame() && !isSliding && cooldownSlideTime <= 0f && isSprinting)
            StartSliding();
        if (isSliding)
        {
            slideTime += Time.deltaTime;
            if (slideTime >= slideDuration)
                StopSliding();
            //else { }
                //rb.MovePosition(rb.position + direcctionToSlide * slideSpeed * Time.deltaTime);

        }
        else
        {
            if (cooldownSlideTime > 0f)
                cooldownSlideTime -= Time.deltaTime;
        }
    }
    private void StartSliding()
    {
        isSliding = true;
        slideTime = 0f;
        cooldownSlideTime = slideCooldown;
        direcctionToSlide = this.gameObject.transform.forward;
        // Aquí puedes activar la animación del deslizamiento si tienes alguna
        // Ejemplo: animator.SetTrigger("Slide");
        cam.GetComponent<Camera>().fieldOfView = defaultFovCamera + fovCamera;
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y / 2, gameObject.transform.position.z);
        // Opcional: aplicar efectos de sonido o partículas si lo deseas.
    }
    private void StopSliding()
    {
        isSliding = false;

        // Aquí puedes detener la animación del deslizamiento
        // Ejemplo: animator.ResetTrigger("Slide");
        cam.GetComponent<Camera>().fieldOfView = defaultFovCamera;
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y * 2, gameObject.transform.position.z);

        // Opcional: aplicar efectos de sonido o partículas al terminar el deslizamiento.
    }
    private void AttackLogic()
    {
        if (controls.Player.Attack.WasPressedThisFrame())
        {
        }
        if (controls.Player.Attack.IsPressed())
        {
        }
        if (controls.Player.Attack.WasReleasedThisFrame())
        {
        }
    }
}