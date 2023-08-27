using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem dust;
     private Rigidbody2D rb;
     public Animator animator;
     public float speed;
     public float jumpForce;
     private float moveInput;
 // todos acima são variáveis de movimento/jump;
    private bool canDash = true; // variável de controle para saber se já podemos dar o dash;
    public bool isDashing; // variável que define se estamos ou não no dash;
    IEnumerator dashCoroutine; // define a coroutine do dash;
    public float Direction = 1; // define a direção do dash
    public float gravityN; // variavel que define nossa gravidade
//todos acima são variáveis de dash;

     private bool isGrounded;
     public Transform feetPos;
     public float checkRadius;
     public LayerMask whatIsGround;
     
//todos acima são variáveis que identificam o chão

     private float jumpTimeCounter;
     public float jumpTime;
     private bool isJumping;
//todos acima são variáveis que identificam o tempo de pulo/se está pulando;   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canDash = true;
        isDashing = false;
        gravityN = rb.gravityScale; // define que nossa variável é igual a gravidade;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        if(isDashing) 
        {
            rb.AddForce(new Vector2(Direction * 5,0), ForceMode2D.Impulse); // ou seja, quando o personagem dar o dash, ele vai ser impulsionado para frente, por conta desta linha de código;
        }
    }

    void Update()
    {
        if(moveInput != 0 )
        {
            Direction = moveInput; // a direção do dash varia com a direção do moveInput;
        }

        if(Input.GetKeyDown(KeyCode.Z) && canDash == true) // quando a tecla Z for pressionada, as coroutines vão iniciar, se o player puder dar dash;
        {
            if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = Dash(.2f,.3f);
        StartCoroutine(dashCoroutine);
        }
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if(moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0,0, 0);
        } else if (moveInput < 0) {
            transform.eulerAngles = new Vector3(0,180,0);
        }

        if(isGrounded == true && Input.GetKeyDown(KeyCode.Space)){
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            }

        if(Input.GetKey(KeyCode.Space) && isJumping == true){
            if(jumpTimeCounter > 0)
            {
                dust.Play();
                rb.velocity = Vector2.up * jumpForce * speed;
                jumpTimeCounter -= Time.deltaTime;
                animator.SetBool("Jumping", true);
            }
            else
            {
               isJumping = false; 
               animator.SetBool("Jumping", false);
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            animator.SetBool("Jumping", false);
        }
    }

    IEnumerator Dash(float dashDuration, float dashCooldown)
    {
        Vector2 originalVelocity = rb.velocity;
        isDashing = true;
        canDash = false;
        rb.gravityScale = 0; // aqui, você define que o dash não terá gravidade.
        rb.velocity = Vector2.zero; // aqui, você define que, mesmo sem gravidade, o dash ainda irá inclinar um pouco para cair.
        dust.Play();
        animator.SetBool("Dashing", true);
        //todo o código acima desta função será realizado na hora.
        yield return new WaitForSeconds(dashDuration);
        isDashing = false; 
        rb.velocity = Vector2.zero; // define que o player irá parar depois do Dash;
        rb.gravityScale = gravityN; // retorna a  gravidade ao normal;
        yield return new WaitForSeconds(dashCooldown);
        animator.SetBool("Dashing", false);
        canDash = true;
        //todo código abaixo desta função será realizado depois de 3 segundos.
    }

}
