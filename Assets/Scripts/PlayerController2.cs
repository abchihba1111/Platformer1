using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpForce = 300f;
    public Vector3 MoveDirection = Vector3.right;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Движение влево-вправо (инвертировано)
        float horizontal = Input.GetAxis("Horizontal");
        transform.Translate(MoveDirection * -horizontal * Speed * Time.deltaTime, Space.World);

        // Поворот персонажа (инвертировано)
        if (horizontal > 0)
            transform.rotation = Quaternion.LookRotation(-MoveDirection);
        else if (horizontal < 0)
            transform.rotation = Quaternion.LookRotation(MoveDirection);

        // Передаём скорость в Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * JumpForce);
            animator.SetTrigger("Jump");
        }

        // Анимация смерти по O
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetTrigger("Death");
        }

        // Анимация танца по I
        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetTrigger("Dance");
        }
    }
}