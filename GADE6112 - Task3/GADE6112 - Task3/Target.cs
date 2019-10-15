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
        //instead of being in unit and building, these are shared. in single class.

        int hideChecksBeforeInvisible = 5;
        bool isVisible = true;

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
        public bool IsVisible {
            get { return isVisible; }
        }

        public int Health {
            get { return health; }
            set { health = value; }
        }

        public int MaxHealth {
            get { return maxHealth; }
        }

        public double GetDistance(Target to) {//NOTE also moved here
            double xDistance = to.X - X;
            double yDistance = to.Y - Y;

            return Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }

        public virtual void Destroy() {
            isDestroyed = true;
        }

        public void CheckHide() {
            if (hideChecksBeforeInvisible == 0)
                return;
            //checks if target that's been dead for a  while should be hidden
            hideChecksBeforeInvisible--;
            isVisible = hideChecksBeforeInvisible > 0;
        }

    }
}
