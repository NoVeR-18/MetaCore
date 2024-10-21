using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Spike : MonoBehaviour
{
    GameObject manager;
    GameObject player;
    IEnumerator resetScene()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(manager.GetComponent<GameManager>().currentScene);
    }
    void Start()
    {
        manager=GameObject.Find("GameManager");
        player=GameObject.Find("Travelboy");
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().PlayerDeath();
            StartCoroutine(resetScene());
        }
    }
}