using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]private Animator myanimator;
    [SerializeField]private Sprite[] availableSprites;
    [SerializeField]private SpriteRenderer myRenderer;

    [SerializeField]private float ClickInterval;
    private float LastClicked;
    private bool isDead;

    public bool deadlyEffect;
    // Start is called before the first frame update
    void Start()
    {
        myRenderer.sprite = availableSprites[Random.Range(0, availableSprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if ((Time.time - LastClicked) > ClickInterval)
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Camera.main.transform.position.x)
                {
                    myanimator.SetTrigger("jumpLeft");
                    myRenderer.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    myanimator.SetTrigger("jumpRight");
                    myRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                LastClicked = Time.time;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;
        if (collision.CompareTag("powerUp"))
        {
            PowerUpsController powerController = collision.GetComponent<PowerUpsController>();
            if (powerController.myType == PowerUpType.Spike)
            {
                myanimator.SetTrigger("death");
                isDead = true;
            }
            else if(powerController.myType == PowerUpType.Time)
            {
                Destroy(collision.gameObject);
                //add time;
                GameManager.instance.AddTime(10);
            }
            
        }
    }
    public void DeathAnim()
    {
        if (isDead)
            return;
        myanimator.SetTrigger("death");
        isDead = true;
    }



}
