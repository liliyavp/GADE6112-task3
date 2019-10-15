using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3
{
    abstract class Building : Target {
        protected char symbol;
        //no longer has properties in target bc inherits from target
        public Building(int x, int y, int health, string faction, char symbol)
        {
            this.x = x;
            this.y = y;
            this.health = health;
            this.maxHealth = health;
            this.faction = faction;
            this.symbol = symbol;
        }

        public Building() {}

        public char Symbol {
            get { return symbol; }
        }

        public override void Destroy() {
            isDestroyed = true;
            symbol = 'X';
        }

        public abstract string Save();

        public override string ToString() {
            return
                "Faction: " + faction + Environment.NewLine +
                "Position: " + x + ", " + y + Environment.NewLine +
                "Health: " + health + " / " + maxHealth + Environment.NewLine;
        }
    }
}
