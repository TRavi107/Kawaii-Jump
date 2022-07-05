using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] SpriteRenderer myrenderer;
    [Range(0,100)]
    [SerializeField] float crackChance;
    [Range(0,100)]
    [SerializeField] float powerUpSpawnChance;
    [SerializeField] GameObject[] powerUps;

    public bool spawnPowerUps;
    GameTheme theme;
    public bool isCracked;
    void Start()
    {
        theme = GameManager.instance.GetCurrentTheme();
        
        if (spawnPowerUps)
        {
            if (Random.Range(0, 100) < crackChance)
            {
                myrenderer.sprite = theme.platformCracked;
                isCracked = true;
            }
            else
                myrenderer.sprite = theme.platform;
            if (Random.Range(0, 100) < powerUpSpawnChance)
                SpawnPowerup();
        }
        else
            myrenderer.sprite = theme.platform;
    }
    private void SpawnPowerup()
    {
        GameObject powerup = Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.identity);
    }

    void Update()
    {
        if (transform.position.x < GameManager.instance.spawnPosLeft.position.x)
        {
            GameManager.instance.SpawnPlatform(new(GameManager.instance.spawnPosRight.position.x, transform.position.y));
            Destroy(this.gameObject);
        }
        else if (transform.position.x > GameManager.instance.spawnPosRight.position.x)
        {
            GameManager.instance.SpawnPlatform(new(GameManager.instance.spawnPosLeft.position.x,transform.position.y));
            Destroy(this.gameObject);

        }
        else if (transform.position.y < GameManager.instance.deletePosBottom.position.y)
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
        if (!isCracked)
            return;
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(nameof(CrackingEffect));
            StartCoroutine(nameof(CrackingEffect), collision.GetComponent<CharacterController>());
            collision.GetComponent<CharacterController>().deadlyEffect = true;
        }
    }
    


    IEnumerator CrackingEffect(CharacterController controller)
    {
        bool up = true;
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
        GetComponent<Rigidbody2D>().isKinematic = false;
        if (controller.deadlyEffect)
        {
            controller.DeathAnim();
        }
    }
}
