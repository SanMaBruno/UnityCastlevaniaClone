using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControler : MonoBehaviour
{
    [SerializeField] int damagePoints = 10;
    [SerializeField] TagId targetTag;

    public void SetDirection(Vector2 direction)
    {
        if(direction.x < 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(targetTag.ToString()))
        {
            var component = collision.gameObject.GetComponent<ITargetCombat>();
            if (component != null)
            {
                component.TakeDamage(damagePoints);
            }
            Destroy(this.gameObject);
        }
    }
}
