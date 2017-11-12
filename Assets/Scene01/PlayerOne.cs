using UnityEngine;

public class PlayerOne : MonoBehaviour
{
    public float playerSpeed = 3;

    private Animator[] animators;

    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    void Update()
    {
        Animate();

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            float xAndY = Mathf.Sqrt(Mathf.Pow(moveX, 2) + Mathf.Pow(moveY, 2));
            transform.Translate(moveX * playerSpeed * Time.deltaTime / xAndY, moveY * playerSpeed * Time.deltaTime / xAndY, transform.position.z, Space.Self);
        }
    }

    void Animate()
    {
        float x = 0;
        float y = 0;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
        {
            x = Input.GetAxis("Horizontal");
        }
        else
        {
            y = Input.GetAxis("Vertical");
        }
        foreach (Animator animator in animators)
        {
            animator.SetFloat("horizontalAxis", x);
            animator.SetFloat("verticalAxis", y);
        }
    }
}
