
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody rb;
    
    [Header("Camera Settings")]
    private GameObject cam;
    [SerializeField] private float fovCamera = 10;
    private float defaultFovCamera;

    [Header("Sensitivity Settings")]
    [SerializeField] private float mouseSensitivity = 300;

    [Header("Movement Settings")]
    private float xRotation = 0f;

    private float speed;
    [SerializeField] private float defaultSpeed = 5f, sprintAcceleration = 5f, maxSprintSpeed = 1.5f;

    [SerializeField] private float slideSpeed = 10f, slideDuration = 1f, cooldownTime = 0f, slideTime = 0f, slideCooldown = 0.5f;
    private bool isSliding = false, isSprinting = false, isMoving = false;
    private Vector3 direcctionToSlide;


    public float smoothTime = 0.1f; // Tiempo de suavizado (ajusta para m�s o menos fluidez)

    private Vector2 currentLook; // Almacena la posici�n de rotaci�n actual
    private Vector2 currentLookVelocity; // Velocidad de cambio de rotaci�n


    private void Awake() { controls = new PlayerControls(); }
    private void OnEnable() { controls.Enable(); }
    private void OnDisable() { controls.Disable(); }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = this.gameObject.GetComponent<Rigidbody>();
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
            transform.position += moveDirection * speed * Time.deltaTime;
    }
    private void JumpLogic()
    {
        if (controls.Player.Jump.WasPressedThisFrame())
            Debug.Log("Jump");
    }
    private void SprintLogic()
    {
        float sprintSpeed = defaultSpeed * maxSprintSpeed;
        if (controls.Player.Sprint.IsPressed() && isMoving)
        {
            isSprinting = true;
            speed = Mathf.MoveTowards(speed, sprintSpeed, sprintAcceleration * Time.deltaTime);
        }
        else
        {
            isSprinting = false;
            speed = Mathf.MoveTowards(speed, defaultSpeed, sprintAcceleration * Time.deltaTime);
        }
    }
    private void SlideLogic()
    {
        if (controls.Player.Slide.WasPressedThisFrame() && !isSliding && cooldownTime <= 0f && isSprinting)
            StartSliding();
        if (isSliding)
        {
            slideTime += Time.deltaTime;
            if (slideTime >= slideDuration)
                StopSliding();
            else
                rb.MovePosition(rb.position + direcctionToSlide * slideSpeed * Time.deltaTime);

        }
        else
        {
            if (cooldownTime > 0f)
                cooldownTime -= Time.deltaTime;
        }
    }
    private void StartSliding()
    {
        isSliding = true;
        slideTime = 0f;
        cooldownTime = slideCooldown;
        direcctionToSlide = this.gameObject.transform.forward;
        // Aqu� puedes activar la animaci�n del deslizamiento si tienes alguna
        // Ejemplo: animator.SetTrigger("Slide");
        cam.GetComponent<Camera>().fieldOfView = defaultFovCamera + fovCamera;
        // Opcional: aplicar efectos de sonido o part�culas si lo deseas.
    }
    private void StopSliding()
    {
        isSliding = false;

        // Aqu� puedes detener la animaci�n del deslizamiento
        // Ejemplo: animator.ResetTrigger("Slide");
        cam.GetComponent<Camera>().fieldOfView = defaultFovCamera;

        // Opcional: aplicar efectos de sonido o part�culas al terminar el deslizamiento.
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