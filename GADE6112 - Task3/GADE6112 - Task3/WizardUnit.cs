using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3 {
    class WizardUnit : Unit{
        public WizardUnit(int x, int y, string faction) : base(x, y, 100, 3, 10, 5, faction, '*', "Sorceress") { }

        public WizardUnit(string values) : base(values) { }

        public override string Save() {
            return string.Format(
                $"Wizard,{x},{y},{health},{maxHealth},{speed},{attack},{attackRange}," +
                $"{faction},{symbol},{name},{isDestroyed}"
            );
        }
    }
}
