using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Star : MonoBehaviour
{
    public Sprite GoldStar;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().collectedStars+=1;
            if(GameObject.Find("GameManager").GetComponent<GameManager>().collectedStars==1)
            {
                GameObject.Find("Star1").GetComponent<Image>().sprite=GoldStar;
            }
            else if(GameObject.Find("GameManager").GetComponent<GameManager>().collectedStars==2)
            {
                GameObject.Find("Star2").GetComponent<Image>().sprite=GoldStar;
            }
            else if(GameObject.Find("GameManager").GetComponent<GameManager>().collectedStars==3)
            {
                GameObject.Find("Star3").GetComponent<Image>().sprite=GoldStar;
            }
            Destroy(gameObject);
        }
    }
}