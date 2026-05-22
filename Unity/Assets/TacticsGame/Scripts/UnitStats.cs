using UnityEngine;

namespace Tactics
{
    [System.Serializable]
    public class UnitStats
    {
        public string unitName = "Unit";
        public int maxHP = 100;
        public int currentHP;

        [Header("Turn Order")]
        public int speed = 5;       // CT gained per time-tick

        [Header("Movement")]
        public int move = 4;        // max tiles per turn
        public int jump = 2;        // max height difference per step

        [Header("Combat")]
        public int attack = 20;
        public int defense = 10;
        public int attackRange = 1; // Manhattan distance, default melee

        [Header("Items")]
        public int itemCount = 2;
        public int itemHealAmount = 40;

        [System.NonSerialized] public bool isDefending = false;

        public void Initialize()
        {
            currentHP = maxHP;
            isDefending = false;
        }

        public int TakeDamage(int rawDamage)
        {
            int effectiveDef = isDefending ? defense * 2 : defense;
            int actual = Mathf.Max(1, rawDamage - effectiveDef);
            currentHP = Mathf.Max(0, currentHP - actual);
            return actual;
        }

        public int Heal(int amount)
        {
            int actual = Mathf.Min(amount, maxHP - currentHP);
            currentHP += actual;
            return actual;
        }

        public bool IsDead => currentHP <= 0;
    }
}
