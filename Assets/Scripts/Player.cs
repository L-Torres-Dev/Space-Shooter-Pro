using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 3.5f;

    private float horizontalInput;
    private float verticalInput;

    private float horizontalBound = 12.15f;
    private float verticalBound = -3.8f;
    //Start is called before the first frame update
    void Start()
    {
        //take the current position = new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(direction * Time.deltaTime * speed);

        Vector2 clampedPos = new Vector2(transform.position.x, transform.position.y);

        clampedPos.y = Mathf.Clamp(clampedPos.y, verticalBound, 0);

        if (clampedPos.x > horizontalBound)
        {
            clampedPos.x = -horizontalBound;
        }
        else if (clampedPos.x < -horizontalBound)
        {
            clampedPos.x = horizontalBound;
        }

        transform.position = clampedPos;
    }
}
