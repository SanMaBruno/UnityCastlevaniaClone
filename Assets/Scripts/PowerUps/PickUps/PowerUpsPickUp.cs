using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsPickUp : MonoBehaviour
{
    [SerializeField] PowerUpId powerUpId;
    [SerializeField] TagId playerTag;
    [SerializeField] TagId groundTag;
    [SerializeField] AudioClip pickSfx;
    [SerializeField] int maxAmount = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(playerTag.ToString()))
        {
            HeroController.instance.ChangePowerUp(powerUpId,Random.Range(5,maxAmount));
            AudioManager.Instance.PlaySfx(pickSfx);
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag.Equals(groundTag.ToString()))
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
