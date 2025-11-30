using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float transperency;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;
    private Playeraction actions;

    private Vector2 moveDirection;
    private float currentSpeed;
    private bool usingDash;

    private void Awake(){
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        actions = new Playeraction();

    }
    void Start()
    {
        currentSpeed = speed;
        actions.Movement.Dash.performed += context => Dash();
    }

    // Update is called once per frame
    void Update()
    {
        CaptureInput();
    }
    private void FixedUpdate(){
        MovePlayer();
        RotatePlayer();
    }
    private void MovePlayer(){
        rb2D.MovePosition(rb2D.position + moveDirection*(currentSpeed*Time.fixedDeltaTime));
    }
    private void Dash(){
        if(usingDash){
            return;
        }
        usingDash = true;
        StartCoroutine(IEDash());
    }
    private IEnumerator IEDash(){
        currentSpeed = dashSpeed;
        ModifySpriteRenderer(transperency);
        yield return new WaitForSeconds(dashTime);
        currentSpeed = speed;
        ModifySpriteRenderer(1f);
        usingDash = false;
    }

    private void RotatePlayer(){
        if(moveDirection.x >= 0.1f){
            spriteRenderer.flipX = false;
        }else if(moveDirection.x <0f){
            spriteRenderer.flipX = true;
        }
    }

    private void ModifySpriteRenderer(float alpha){
        Color color = spriteRenderer.color;
        color = new Color(color.r, color.g, color.b, alpha);
        spriteRenderer.color = color; 
    }

    private void CaptureInput(){
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
    }
    private void OnEnable(){
        actions.Enable();
    }
    private void OnDisable(){
        actions.Disable();
    }
}
