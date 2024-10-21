using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpikeDetector : MonoBehaviour
{
    public float spikeTimer;
    public GameObject specialSpike;
    private bool spikeActivated=false;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")&&!spikeActivated)
        {
            StartCoroutine(spawnSpike());
        }
    }
    IEnumerator spawnSpike()
    {
        spikeActivated=true;
        yield return new WaitForSeconds(spikeTimer);
        GameObject spike=Instantiate(specialSpike,transform.position,transform.rotation);
        yield return new WaitForSeconds(1.5f);
        Destroy(spike);
        spikeActivated=false;
    }
}