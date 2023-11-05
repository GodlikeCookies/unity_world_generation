using UnityEngine;

/*
 * ѕримитивна€ логика перемещени€ игрока
 */
public class CharacterController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    private void Update()
    {
        MovementLogic();
    }
    void MovementLogic()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.GetComponent<Rigidbody>().MovePosition(transform.position + movement * Time.deltaTime * speed);
    }
}
