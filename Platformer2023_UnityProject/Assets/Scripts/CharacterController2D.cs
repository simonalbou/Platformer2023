using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    // valeurs à équilibrer
    [Header("Parameters")]
    public float speed = 5;
    public float gravity = -9.81f;
    public float jumpStrength = 10;
    [Tooltip("Used for double-jumps, triple-jumps, or more.")]
    public int numberOfJumps = 2;
    [Tooltip("Acts as a gravity multiplier during jump.")]
    public AnimationCurve jumpCurve;

    // références de components
    [Header("References")]
    public Transform selfTransform;
    public Raycaster2D raycaster;
    public CharacterProfile characterControllerData;

    public UnityEvent onBecameGrounded;

    // private variables
    Vector2 inputVector;
    bool isJumping;
    float timeSinceJumped;
    int remainingJumps;

    void Start()
    {
        ResetAllowedJumps();
        raycaster.onCollisionEnter.AddListener(OnRaycasterDetectedObstacle);
    }

    void Update()
    {
        ListenToInputs();
        UpdateMovement();
        UpdateGraphics();
    }

    void OnRaycasterDetectedObstacle(RayDirection dir)
    {
        if (dir == RayDirection.Below)
        {
            onBecameGrounded?.Invoke();
        }
    }

    public void ResetAllowedJumps()
    {
        remainingJumps = numberOfJumps;
    }

    void ListenToInputs()
    {
        // movement
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            timeSinceJumped = 0;
            isJumping = true;
            remainingJumps--;
        }
    }

    void UpdateMovement()
    {
        Vector2 movementVector = Vector2.zero;
        movementVector.x = CalculateHorizontalMovement();
        movementVector.y = CalculateVerticalMovement();
        Move(movementVector * Time.deltaTime);
    }

    float CalculateHorizontalMovement()
    {
        return inputVector.x * speed;
    }

    // calcule l'intensité de la gravité en fonction du saut
    float CalculateVerticalMovement()
    {
        if (isJumping)
        {
            timeSinceJumped += Time.deltaTime;

            // vérifier si on a fini le saut (fin de la courbe) ou pas encore
            if (timeSinceJumped >= jumpCurve.keys[jumpCurve.keys.Length-1].time)
            {
                isJumping = false;
            }

            return gravity * jumpCurve.Evaluate(timeSinceJumped);

            // méthode alternative : pour une courbe qui dessine la trajectoire du saut
            /**
            float altitudeThisFrame = jumpCurve.Evaluate(timeSinceJumped);
            float altitudePreviousFrame = jumpCurve.Evaluate(timeSinceJumped - Time.deltaTime);
            return (altitudeThisFrame - altitudePreviousFrame) * jumpStrength;
            /**/
        }

        // else : le joueur n'est pas en train de sauter (on applique donc la gravité simple)
        return gravity;
    }

    void UpdateGraphics()
    {
        // n'affecte que le sprite, pas ses enfants
        //if (inputVector.x < 0) spriteRenderer.flipX = true;
        //if (inputVector.x > 0) spriteRenderer.flipX = false;

        // affecte tous les enfants de l'objet
        if (inputVector.x != 0)
        {
            selfTransform.localScale = new Vector3(
                Mathf.Abs(selfTransform.localScale.x) * Mathf.Sign(inputVector.x),
                selfTransform.localScale.y,
                selfTransform.localScale.z
            );
        }
    }

    void Move(Vector2 direction)
    {
        MoveHorizontal(direction.x);
        MoveVertical(direction.y);
    }

    void MoveHorizontal(float direction)
    {
        if (direction == 0) return;
        if (direction < 0 && raycaster.CastRays(RayDirection.Left, Mathf.Abs(direction))) return;
        if (direction > 0 && raycaster.CastRays(RayDirection.Right, Mathf.Abs(direction))) return;

        // proceed to movement
        selfTransform.Translate(Vector3.right * direction);
    }

    void MoveVertical(float direction)
    {
        if (direction == 0) return;
        if (direction < 0 && raycaster.CastRays(RayDirection.Below, Mathf.Abs(direction))) return;
        if (direction > 0 && raycaster.CastRays(RayDirection.Above, Mathf.Abs(direction))) return;

        // proceed to movement
        selfTransform.Translate(Vector3.up * direction);
    }
}
