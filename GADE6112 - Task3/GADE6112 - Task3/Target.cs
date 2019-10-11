using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3 {

    class Target {
        protected int x;
        protected int y;
        protected int health;
        protected int maxHealth;
        protected bool isDestroyed;
        protected string faction;

        public string Faction {
            get { return faction; }
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        public int Y {
            get { return y; }
            set { y = value; }
        }

        public bool IsDestroyed {
            get { return isDestroyed; }
        }

        public int Health {
            get { return health; }
            set { health = value; }
        }

        public int MaxHealth {
            get { return maxHealth; }
        }

        public double GetDistance(Target to) {
            double xDistance = to.X - X;
            double yDistance = to.Y - Y;

            return Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }

        public virtual void Destroy() {
            isDestroyed = true;
        }

    }
}
