using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3
{
    class RangedUnit : Unit  {
        public RangedUnit(int x, int y, string faction) : base(x, y, 150, 2, 15, 3, faction, '-', "Archer") { }

        public RangedUnit(string values) : base(values) { }

        public override string Save() {
            return string.Format(
                $"Ranged,{x},{y},{health},{maxHealth},{speed},{attack},{attackRange}," +
                $"{faction},{symbol},{name},{isDestroyed}"
            );
        }
    }
}
