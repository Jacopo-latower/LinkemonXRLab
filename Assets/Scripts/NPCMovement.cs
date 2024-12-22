using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float npcSpeed = 2f;


    public void Move(float x, float y)
    {

        if (x > 0f)
        {
            transform.Translate(npcSpeed * transform.right * Time.deltaTime);
            /*
            backSprite.SetActive(false);
            frontSprite.SetActive(false);
            rightSprite.SetActive(true);

            if (isFlipped)
            {
                isFlipped = false;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            */
        }
        else if (x < 0f)
        {
            transform.Translate(npcSpeed * -transform.right * Time.deltaTime);
            /*backSprite.SetActive(false);
            frontSprite.SetActive(false);
            rightSprite.SetActive(true);

            if (!isFlipped)
            {
                isFlipped = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }*/
        }
        else if (y > 0f)
        {
            transform.Translate(npcSpeed * transform.up * Time.deltaTime);

            /*backSprite.SetActive(true);
            frontSprite.SetActive(false);
            rightSprite.SetActive(false);*/

        }
        else if (y < 0f)
        {
            transform.Translate(npcSpeed * -transform.up * Time.deltaTime);

            /*backSprite.SetActive(false);
            frontSprite.SetActive(true);
            rightSprite.SetActive(false);
            */
        }
    }
}
