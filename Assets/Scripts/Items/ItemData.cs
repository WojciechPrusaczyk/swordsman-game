using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class ItemData
    {
        private string itemName;
        private string damageType;
        private string description;
        private string lore;
        private double cooldown;
        private double actualCooldown;

        public ItemData(string itemName, string damageType, string description, string Lore, double cooldown = 0, double actualCooldown = 0)
        {
            this.itemName = itemName;
            this.damageType = damageType;
            this.description = description;
            this.lore = Lore;
            this.cooldown = cooldown;
            this.actualCooldown = actualCooldown;
        }
    
        /*
         * Nazwa przedmiotu
         */
        public void SetName(string itemName)
        {
            this.itemName = itemName;
        }
        public string GetName()
        {
            return this.itemName;
        }
    
        /*
         * Damage type
         */
        public void SetDamageType(string damageType)
        {
            this.damageType = damageType;
        }
        public string GetDamageType()
        {
            return this.damageType;
        }

        /*
         * Description
         */
        public void SetDescription(string description)
        {
            this.description = description;
        }
        public string GetDescription()
        {
            return this.description;
        }

        /*
         * Lore
         */
        public void SetLore(string lore)
        {
            this.lore = lore;
        }
        public string GetLore()
        {
            return this.lore;
        }

        /*
         * Cooldown
         */
        public void SetCooldown(double cooldown)
        {
            this.cooldown = cooldown;
        }
        public double GetCooldown()
        {
            return this.cooldown;
        }
    }
}

