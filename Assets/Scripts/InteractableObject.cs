using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public TypeObject typeObject;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            switch (typeObject)
            {
                case TypeObject.Coin: { ColectCoin(); break; }
                case TypeObject.People: { ColectPeople(collision); break; }
                case TypeObject.Crystal: { ColectCrystal(); break; }
            }
        }
    }


    private void ColectCoin()
    {
        LevelManager.Instance.ColectedCoins++;
        Destroy(gameObject);
    }
    private void ColectCrystal()
    {
        LevelManager.Instance.ColectedCrystal++;
        Destroy(gameObject);
    }
    private void ColectPeople(Collider collision)
    {
        LevelManager.Instance.ColectedPeople++;
        gameObject.transform.parent = collision.transform;
        transform.localPosition = Vector3.zero;
        var colectedPeople = GetComponentInParent<PlayerTileInteraction>().ColectedPeople;
        colectedPeople.Add(gameObject);
        Destroy(this);
    }

}
public enum TypeObject
{
    SpawnPossiton,
    Coin,
    Crystal,
    People
}