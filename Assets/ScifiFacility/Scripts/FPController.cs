// FPController.cs — Assets/ScifiFacility/Scripts/
// Actualizado para usar New Input System
// Mantiene las mismas variables públicas del original

using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{
    public float speed           = 6f;
    public float mouseSensitivity = 5f;
    public float jumpSpeed       = 10f;

    private float   rotationLeftRight;
    private float   verticalRotation;
    private float   forwardspeed;
    private float   sideSpeed;
    private float   verticalVelocity;
    private Vector3 speedCombined;

    private CharacterController cc;
    private Camera              cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cc  = GetComponent<CharacterController>();
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ── Rotar con mouse ──────────────────────────────
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        rotationLeftRight = mouseDelta.x * mouseSensitivity * Time.deltaTime * 10f;
        transform.Rotate(0, rotationLeftRight, 0);

        verticalRotation -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 10f;
        verticalRotation  = Mathf.Clamp(verticalRotation, -60f, 60f);
        cam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // ── Movimiento WASD ──────────────────────────────
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y =  1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y = -1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x =  1f;

        forwardspeed = moveInput.y * speed;
        sideSpeed    = moveInput.x * speed;

        // Correr con Shift
        if (Keyboard.current.leftShiftKey.isPressed)
            forwardspeed *= 2f;

        // ── Gravedad y salto ─────────────────────────────
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (cc.isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            verticalVelocity = jumpSpeed;

        // ── Aplicar movimiento ───────────────────────────
        speedCombined   = new Vector3(sideSpeed, verticalVelocity, forwardspeed);
        speedCombined   = transform.rotation * speedCombined;
        cc.Move(speedCombined * Time.deltaTime);
    }
}