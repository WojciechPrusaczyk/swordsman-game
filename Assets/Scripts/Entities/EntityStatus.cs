using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatus : MonoBehaviour
{
    public string entityName = "";
    public int entityLevel = 1;
    public int entityExperiencePoints = 0;
    [ReadOnly] public int entityExperienceToNextLvl = 50;
    public float entityHealthPoints = 100.0f;
    public float entityMaxHelath = 100.0f;
    public int droppedXp = 0;
    public int gold = 0;
    private float BaseAttackDamage = 0;
    public float AttackDamage = 0;
    public float MovementSpeed = 0;
    public bool isFacedRight = true;
    public bool isEnemy = false;
    public GameObject detectedTarget;
    public float attackRange;
    public Color lightDamageColor;
    public Color heavyDamageColor;
    public Color deathColor;
    private GameObject entityObject;
    public GameObject healthBar;

    private GameObject mainUserInterface;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    

    // Wartość wyrażona w procentach, która odpowiada za % otrzymywanych obrażeń
    public float incomingDamagePercent = 1.0f;
    /*
     * TODO: dodać base ad, oraz hp, aby uniknąć możliwości zmieniania statystyk w nieskończoność
     */
    private void Awake()
    {
        mainUserInterface = GameObject.Find("Main User Interface");
        player = GameObject.Find("Player");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        BaseAttackDamage = AttackDamage;
    }
    
    /*
     * Nazwa
     */
    public void SetName(string name)
    {
        this.entityName = name;
    }
    public string GetName()
    {
        return this.entityName;
    }
    
    /*
     * Poziom doświadczenia
     */
    public void SetLevel(int level)
    {
        this.entityLevel = level;
    }
    public int GetLevel()
    {
        return this.entityLevel;
    }
    
    /*
     * Ilość punktów doświadczenia
     */
    public void SetXp(int xp)
    {
        this.entityExperiencePoints = xp;
    }
    public int GetXp()
    {
        return this.entityExperiencePoints;
    }
    public void AddXp(int xpAmount)
    {
        while (xpAmount > 0)
        {
            int xpToLvlUp = entityExperienceToNextLvl - GetXp();
            
            // Jeżeli nie mamy wystaczająco xp do lvl up
            if (xpAmount < xpToLvlUp)
            {
                SetXp( GetXp() + xpAmount );
                break;
            }
            if ( xpToLvlUp <= xpAmount )
            {
                SetLevel( GetLevel() + 1 );
                SetExpToNextLVl(  Convert.ToInt32(Convert.ToDouble(GetExpToNextLVl()) * 1.25 ) );
                xpAmount -= xpToLvlUp;
                this.entityExperiencePoints = 0;
            }
        }
    }
    
    /*
     * Punkty życia
     */
    public void SetHp(float hp)
    {
        this.entityHealthPoints = hp;
    }
    public float GetHp()
    {
        return this.entityHealthPoints;
    }
    
    /*
     * Maksymalne punkty życia
     */
    public void SetMaxHp(float maxHp)
    {
        this.entityMaxHelath = maxHp;
    }
    public float GetMaxHp()
    {
        return this.entityMaxHelath;
    }

    public void DealDamage(float damage, GameObject attackingEntity = null)
    {
        // zawsze podawać attackingEntity, gdy przeciwnik atakuej gracza
        if (gameObject.CompareTag("Player"))
        {
            bool isBLocking = gameObject.GetComponent<Player>().isBlocking;
            bool isParrying = gameObject.GetComponent<Player>().isParrying;
            bool isPlayerFacedToEnemy = false;
            
            if (attackingEntity)
            {
                EntityStatus playerStatus = gameObject.GetComponent<EntityStatus>();
                EntityStatus enemyStatus = attackingEntity.GetComponent<EntityStatus>();
                isPlayerFacedToEnemy = (playerStatus.isFacedRight && !enemyStatus.isFacedRight) ||
                                       (!playerStatus.isFacedRight && enemyStatus.isFacedRight);
            }
            /*Debug.Log(isPlayerFacedToEnemy);
            Debug.Log(gameObject.GetComponent<EntityStatus>().isFacedRight);
            Debug.Log(gameObject.GetComponent<EntityStatus>().isFacedRight);*/

            if (isParrying && isPlayerFacedToEnemy)
            {
                // gracz sparował cios
                float parryingDamageReduction = 0f; // 0 = 100% redukcji
                if ( damage * parryingDamageReduction * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * parryingDamageReduction * incomingDamagePercent < GetHp())
                {
                    // gracz otrzymuje obrażenia
                    GettingDamageEvent(damage * parryingDamageReduction * incomingDamagePercent);
                    
                    // odgrywanie animacji po sparowaniu
                    gameObject.GetComponent<Animator>().Play("parryAttack");
                }
                
                
            } else if (isBLocking && isPlayerFacedToEnemy)
            {
                // gracz zablokował cios
                float blockingDamageReduction = 0.6f; // 0.6 = 40% redukji
                if ( damage * blockingDamageReduction * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * blockingDamageReduction * incomingDamagePercent < GetHp())
                {
                    // gracz otrzymuje obrażenia
                    GettingDamageEvent(damage * blockingDamageReduction * incomingDamagePercent);
                }
            }
            else
            {
                StartCoroutine(SygnalizeGettingDamage());
                // gracz nie sparował, ani nie zablokował ciosu
                if ( damage * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * incomingDamagePercent < GetHp())
                {
                    // gracz otrzymuje obrażenia
                    GettingDamageEvent(damage * incomingDamagePercent);
                    
                    // animacja paska hp sygnalizująca otrzymanie obrażeń
                }
            }
        }
        else
        {
            // Kod dla wszystkich encji poza graczem
            if ( damage * incomingDamagePercent >= GetHp() )
            {
                // Encja ginie
                DeathEvent();
            } else if (damage * incomingDamagePercent < GetHp())
            {
                // encja otrzymuje obrażenia
                GettingDamageEvent(damage);
            }
        }
    }

    void DeathEvent()
    {
        player.GetComponent<EntityStatus>().AddXp(droppedXp);
        StartCoroutine(DeathAnimation(deathColor, 0.1f));
    }

    public void PlayerDeathEvent()
    {
        EntityStatus playerStatus = player.GetComponent<EntityStatus>();
        
        // przelanie expa z gracza na przeciwnika
        AddXp(playerStatus.GetXp());
        playerStatus.SetXp( 0 );
        
        StartCoroutine(DeathAnimation(deathColor, 0.1f));
    }
    
    void GettingDamageEvent( float damage)
    {
        SetHp(GetHp() - damage);
            
        if (damage >= ( GetHp() / 2 ) ) StartCoroutine(ChangeColorForInterval(heavyDamageColor, 0.05f));
        else StartCoroutine(ChangeColorForInterval(lightDamageColor, 0.12f));
    }

    private IEnumerator SygnalizeGettingDamage()
    {
        // w tym momencie gameObject musi być graczem
        Image healthBarImage = healthBar.GetComponent<Image>();
        Color currentColor = healthBarImage.color;
        
        // animacja otrzymania obrażeń
        healthBarImage.color = Color.red;

        MoveHpBar(0, 3);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, -6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, 6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, -6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, 3);
        
        healthBarImage.color = currentColor;
    }
    
    void MoveHpBar(float xOffset, float yOffset)
    {
        // Pobierz bieżące pozycje
        Vector3 currentPosition = healthBar.transform.localPosition;

        // Dodaj przesunięcie
        currentPosition.x += xOffset;
        currentPosition.y += yOffset;

        // Ustaw nową pozycję
        healthBar.transform.localPosition = currentPosition;
    }
    
    private IEnumerator ChangeColorForInterval(Color color, float duration)
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.Log("Nie wykryto spriteRenderer");
        }
    }
    
    private IEnumerator DeathAnimation(Color color, float duration)
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Nie wykryto spriteRenderer");
        }
    }
    
    /*
     * Punkty doświadczenia do następnego poziomu
     */
    public void SetExpToNextLVl(int expToNextLvl)
    {
        this.entityExperienceToNextLvl = expToNextLvl;
    }
    public int GetExpToNextLVl()
    {
        return this.entityExperienceToNextLvl;
    }

    public float GetBaseAttackDamage()
    {
        return this.BaseAttackDamage;
    }
    
    /*
     * Ilość złota
     */
    public void SetGold(int gold)
    {
        this.gold = gold;
    }
    public int GetGold()
    {
        return this.gold;
    }
    public void AddGold(int gold)
    {
        this.gold += gold;

        /*
         * Jeśli encja jest graczem, to wyświetlamy złoto w UI
         */
        if ( gameObject.CompareTag("Player") )
        {
            GameObject UiGoldCount = mainUserInterface.transform.Find("Gold/Count").gameObject;
            if (UiGoldCount)
            {
                UiGoldCount.GetComponent<TextMeshProUGUI>().text = Convert.ToString( this.gold );
            }
        }
    }
    
    /*
     * Obrażenia
     */
    public void SetAttackDamageCount(float AttackDamage)
    {
        this.AttackDamage = AttackDamage;
    }
    public float GetAttackDamageCount()
    {
        return this.AttackDamage;
    }

    /*
     * Prędkość ruchu
     */
    public void SetMovementSpeed(float MovementSpeed)
    {
        this.MovementSpeed = MovementSpeed;
    }
    public float GetMovementSpeed()
    {
        return this.MovementSpeed;
    }
}
