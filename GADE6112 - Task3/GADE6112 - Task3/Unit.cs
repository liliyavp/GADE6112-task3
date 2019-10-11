using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3
{
    abstract class Unit : Target
    {
        protected int speed;
        protected int attack;
        protected int attackRange;
        protected char symbol;
        protected bool isAttacking = false;
        protected string name;

        public static Random random = new Random();

        public Unit(int x, int y, int health, int speed, int attack, int attackRange, string faction, char symbol, string name) {
 
            this.health = health;
            maxHealth = health;
            this.speed = speed;
            this.attack = attack;
            this.attackRange = attackRange;
            this.symbol = symbol;
            this.name = name;

            this.x = x;
            this.y = y;
            this.faction = faction;
        }

        public Unit(string values)
        {
            string[] parameters = values.Split(',');

            X = int.Parse(parameters[1]);
            Y = int.Parse(parameters[2]);
            health = int.Parse(parameters[3]);
            maxHealth = int.Parse(parameters[4]);
            speed = int.Parse(parameters[5]);
            attack = int.Parse(parameters[6]);
            attackRange = int.Parse(parameters[7]);
            faction = parameters[8];
            symbol = parameters[9][0];
            name = parameters[10];
            isDestroyed = parameters[11] == "True" ? true : false;
        }

        public abstract string Save();

        public char Symbol {
            get { return symbol; }
        }

        public string Name {
            get { return name; }
        }

        //returns true if target was destroyed
        public virtual bool Attack(Target target) {
            isAttacking = true;
            target.Health -= attack;

            if (target.Health <= 0) {
                target.Health = 0;
                target.Destroy();
                return true;
            }

            return false;
        }

        public override void Destroy() {
            isDestroyed = true;
            isAttacking = false;
            symbol = 'X';
        }

        public virtual bool IsInRange(Target target) {
            return GetDistance(target) <= attackRange;
        }

        public virtual void Move(Target closestTarget) {
            int xDirection = closestTarget.X - X;
            int yDirection = closestTarget.Y - Y;

            if (Math.Abs(xDirection) > Math.Abs(yDirection))  {
                x += Math.Sign(xDirection);
            }
            else {
                y += Math.Sign(yDirection);
            }
        }

        public virtual void RunAway() {
            int direction = random.Next(0, 4);
            if (direction == 0) {
                x += 1;
            }
            else if (direction == 1) {
                x -= 1;
            }
            else if (direction == 2) {
                y += 1;
            }
            else {
                y -= 1;
            }
        }

        public override string ToString() {
            return
                "------------------------------------------" + Environment.NewLine +
                name + " (" + symbol + "/" + faction[0] + ")" + Environment.NewLine +
                "------------------------------------------" + Environment.NewLine +
                "Faction: " + faction + Environment.NewLine +
                "Position: " + x + ", " + y + Environment.NewLine +
                "Health: " + health + " / " + maxHealth + Environment.NewLine;
        }

    }
}
