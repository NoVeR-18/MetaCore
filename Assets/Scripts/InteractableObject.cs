using Player;
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
                case TypeObject.Coin:
                    {
                        ColectCoin(collision.GetComponent<PlayerTileInteraction>().Wallet);
                        break;
                    }
                case TypeObject.People: { ColectPeople(collision); break; }
                case TypeObject.Crystal: { ColectCrystal(collision.GetComponent<PlayerTileInteraction>().Wallet); break; }
            }
        }
    }


    private void ColectCoin(PlayerWallet playerWallet)
    {
        LevelManager.Instance.ColectedCoins++;
        playerWallet.AddMoney(1);
        Destroy(gameObject);
    }
    private void ColectCrystal(PlayerWallet playerWallet)
    {
        LevelManager.Instance.ColectedCrystal++;
        playerWallet.AddCrystal(1);
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