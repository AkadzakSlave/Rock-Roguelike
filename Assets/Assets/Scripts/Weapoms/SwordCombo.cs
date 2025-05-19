using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCombo : MonoBehaviour
{
    public int[] comboDamage = { 10, 15, 20 }; // Урон для 3 ударов
    public float[] comboDelays = { 0.3f, 0.5f, 1f };
    public float comboWindow = 0.8f; // Время между ударами для комбо
    
    private int currentCombo = 0;
    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < lastAttackTime + comboWindow)
            {
                currentCombo = (currentCombo + 1) % comboDamage.Length;
            }
            else
            {
                currentCombo = 0;
            }
            
            Attack(comboDamage[currentCombo]);
            lastAttackTime = Time.time;
        }
    }

    void Attack(int damage)
    {
        // Анимация атаки + обработка попадания
        Debug.Log($"Комбо {currentCombo+1}! Урон: {damage}");
    }
}