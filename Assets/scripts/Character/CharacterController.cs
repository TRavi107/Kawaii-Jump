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
    public Vector3 destination;
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
        if (UIManager.instance.activeUIPanel.uiPanelType == UIPanelType.howToplay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayGame();
            }
        }
        else if (Input.GetMouseButtonDown(0))
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

    public void PlayGame()
    {
        UIManager.instance.SwitchCanvas(UIPanelType.mainGame);
        GameManager.instance.StartGame();
        GameManager.instance.handImg.SetActive(false);
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
                
                myanimator.SetTrigger("fallDeath");
                isDead = true;
            }
            else if(powerController.myType == PowerUpType.Time)
            {
                Destroy(collision.gameObject);
                //add time;
                GameManager.instance.AddTime(5);
                soundManager.instance.PlaySound(SoundType.powerUpSound);
            }
            
        }
    }
    public void SetPos()
    {
        //myanimator.enabled=false;
        //transform.position = destination;
        ////myanimator.enabled=true;
        //myanimator.speed = 1;
        //myanimator.applyRootMotion = true;
    }

    public void DeathAnim()
    {
        if (isDead)
            return;
        myanimator.SetTrigger("death");
        isDead = true;
    }
    public void FallDeath()
    {
        if (isDead)
            return;
        myanimator.SetTrigger("fallDeath");
        isDead = true;
    }

    
}
