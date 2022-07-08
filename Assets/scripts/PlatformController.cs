using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] SpriteRenderer myrenderer;
    [Range(0,100)]
    [SerializeField] float crackChance;
    [Range(0, 100)]
    [SerializeField] float dangerPlatChance;
    [Range(0,100)]
    [SerializeField] float powerUpSpawnChance;
    [SerializeField] GameObject[] powerUps;
    [SerializeField] GameObject dangerPow;

    public bool spawnPowerUps;
    GameTheme theme;
    public bool isCracked;
    public bool isDanger;

    GameObject spawnedPowerUp;
    void Start()
    {
        theme = GameManager.instance.GetCurrentTheme();
        isDanger = false;
        if (spawnPowerUps)
        {
            if (Random.Range(0, 100) < crackChance)
            {
                myrenderer.sprite = theme.platformCracked;
                isCracked = true;
            }
            else if(Random.Range(0, 100) < dangerPlatChance)
            {
                myrenderer.sprite = GameManager.instance.GetOtherTheme().platform;
                spawnedPowerUp=Instantiate(dangerPow, transform.position, Quaternion.identity);
                isDanger = true;
            }
            else
                myrenderer.sprite = theme.platform;

            if (!isDanger)
            {
                if (Random.Range(0, 100) < powerUpSpawnChance)
                    SpawnPowerup();
            }
        }
        else
            myrenderer.sprite = theme.platform;
    }
    private void SpawnPowerup()
    {
        spawnedPowerUp = Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.identity);
    }

    void Update()
    {
        if (transform.position.x <= GameManager.instance.spawnPosLeft.position.x)
        {
            GameManager.instance.SpawnPlatform(new(GameManager.instance.spawnPosRight.position.x, transform.position.y));
            Destroy(this.gameObject);
        }
        else if (transform.position.x >= GameManager.instance.spawnPosRight.position.x)
        {
            GameManager.instance.SpawnPlatform(new(GameManager.instance.spawnPosLeft.position.x,transform.position.y));
            Destroy(this.gameObject);

        }
        else if (transform.position.y <= GameManager.instance.deletePosBottom.position.y)
        {
            GameManager.instance.SpawnPlatform(new(transform.position.x, GameManager.instance.spawnPosTop.position.y));
            Destroy(this.gameObject);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isCracked)
            return;
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterController>().deadlyEffect = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            if (isCracked)
            {
                //StopCoroutine(nameof(CrackingEffect));
                StartCoroutine(nameof(CrackingEffect), collision.GetComponent<CharacterController>());
                collision.GetComponent<CharacterController>().deadlyEffect = true;
            }
            else if (isDanger)
            {
                //StopCoroutine(nameof(dangerEffect));
                StartCoroutine(nameof(dangerEffect), collision.GetComponent<CharacterController>());
            }
            collision.GetComponent<CharacterController>().destination = transform.position;
        }

    }
    void OnDestroy()
    {
        Destroy(spawnedPowerUp);
    }
    IEnumerator dangerEffect(CharacterController controller)
    {
        bool up = true;
        for (int i = 0; i < 5; i++)
        {
            if (up)
                transform.position = new(transform.position.x, transform.position.y + 0.05f);
            else
            {
                transform.position = new(transform.position.x, transform.position.y - 0.05f);
            }
            up = !up;
            yield return new WaitForSeconds(0.035f);
        }
        GetComponent<Rigidbody2D>().isKinematic = false;

        controller.FallDeath();
    }

    IEnumerator CrackingEffect(CharacterController controller)
    {
        bool up = true;
        Vector2 initialPos = transform.position;
        for (int i = 0; i < 15; i++)
        {
            if (up)
                transform.position = new(transform.position.x, transform.position.y + 0.025f);
            else
            {
                transform.position = new(transform.position.x, transform.position.y - 0.025f);
            }
            up = !up;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = initialPos;
        //GetComponent<Rigidbody2D>().isKinematic = false;
        this.transform.localScale = Vector2.zero;
        if (controller.deadlyEffect)
        {
            controller.FallDeath();
        }
    }
}
