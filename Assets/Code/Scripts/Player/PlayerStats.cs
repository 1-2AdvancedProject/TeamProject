using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Custom/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public int id;
    public int level;
    public int coin;
    public float maxHP;
    public float currentHP;
    public float speed;
    public float attack;
    public float magic;
    public float attackSpeed;
    public int criticalChance;
    public int criticalValue;
    public float defense;
    public float drainLife;
    public float Luck;

    // PlayerStats 초기화
    public void ResetStats()
    {
        speed = 5.5f;
        coin = 0;
        maxHP = 800;
        currentHP = maxHP;
        attack = 150;
    }
}