using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private GameObject shooter; // Pole przechowujące referencję na obiekt, który wystrzelił kulę

    // Metoda ustawiająca referencję na obiekt, który wystrzelił kulę
    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && null != shooter)
        {
            EntityStatus playerStatus = collision.gameObject.GetComponent<EntityStatus>();
            EntityStatus shooterStatus = shooter.gameObject.GetComponent<EntityStatus>();
            
            // zadanie obrażeń graczowi
            playerStatus.DealDamage(shooterStatus.GetAttackDamageCount(), shooter);
            
            // usunięcie pocisku
            Destroy(gameObject);
        }
    }
}
