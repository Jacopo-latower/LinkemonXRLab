using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private GameObject backSprite;
    [SerializeField] private GameObject frontSprite;
    [SerializeField] private GameObject rightSprite;

    [SerializeField] private float playerSpeed = 2f;

    private bool isFlipped = false;
    private bool canMove = true;

    public bool CanMove { set => canMove = value; get => canMove; }
    // Start is called before the first frame update
    void Start()
    {
        canMove = false;
        backSprite.SetActive(true);
        frontSprite.SetActive(false);
        rightSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
            Move();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x > 0f)
        {
            transform.Translate(playerSpeed * transform.right * Time.deltaTime);
            backSprite.SetActive(false);
            frontSprite.SetActive(false);
            rightSprite.SetActive(true);

            if (isFlipped)
            {
                isFlipped = false;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
           
        }
        else if (x < 0f)
        {
            transform.Translate(playerSpeed * -transform.right * Time.deltaTime);
            backSprite.SetActive(false);
            frontSprite.SetActive(false);
            rightSprite.SetActive(true);

            if (!isFlipped)
            {
                isFlipped = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (y > 0f)
        {
            transform.Translate(playerSpeed * transform.up * Time.deltaTime);

            backSprite.SetActive(true);
            frontSprite.SetActive(false);
            rightSprite.SetActive(false);

        }
        else if (y< 0f)
        {
            transform.Translate(playerSpeed * -transform.up * Time.deltaTime);

            backSprite.SetActive(false);
            frontSprite.SetActive(true);
            rightSprite.SetActive(false);
 
        }
    }
}
