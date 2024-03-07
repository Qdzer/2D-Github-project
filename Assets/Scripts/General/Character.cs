using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    [Header("受伤无敌")]
    [HideInInspector] public float invulnerableCounter;
    public float invulnerableDuration;
    public bool invulnerable;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <=0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    public void TakeDamage(Attack attacker)
    {   if (invulnerable)
            return;
        //Debug.Log(attacker.damage);
        if (currentHealth - attacker.damage > 0)
        {   //数值减少
            currentHealth -= attacker.damage;
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
            //触发无敌
            TriggerInvulnerable();
          
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        if(!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
