using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < GameManager.instance.bgspawnPosLeft.position.x)
        {
            GameManager.instance.spawnBgCloud(GameManager.instance.bgspawnPosRight.position);
            Destroy(this.gameObject);
        }
        else if (transform.position.x > GameManager.instance.bgspawnPosRight.position.x)
        {
            GameManager.instance.spawnBgCloud(GameManager.instance.bgspawnPosLeft.position);
            Destroy(this.gameObject);

        }
        else if (transform.position.y < GameManager.instance.bgdeletePosBottom.position.y)
        {
            GameManager.instance.spawnBgCloud(GameManager.instance.bgspawnPosTop.position);
            Destroy(this.gameObject);

        }
    }
}
