using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3
{
    class Map
    {
        public readonly int width = 20;
        public readonly int height = 20;

        Unit[] units;
        Building[] buildings;

        string[,] map;
        string[] factions = { "A-Team", "B-Team" };

        int numUnits;
        int numBuildings;

        public Map(int width, int height, int numUnits, int numBuildings) {
            this.numUnits = numUnits;
            this.numBuildings = numBuildings;

            this.width = width;
            this.height = height;

            map = new string[width, height];
            InitializeUnits();
            InitializeBuildings();
            UpdateMap();
        }
        public Map(int width, int height)
        {
            this.numUnits = 0;
            this.numBuildings = 0;

            this.width = width;
            this.height = height;

            map = new string[width, height];
            units = new Unit[numUnits];
            buildings = new Building[numBuildings];
            UpdateMap();

        }

        public Unit[] Units
        {
            get { return units; }
        }

        public Building[] Buildings
        {
            get { return buildings; }
        }

        private void InitializeUnits()
        {
            units = new Unit[numUnits];

            for (int i = 0; i < units.Length; i++)
            {
                int x = GameEngine.random.Next(0, width);
                int y = GameEngine.random.Next(0, height);
                int factionIndex = GameEngine.random.Next(0, 2);
                int unitType = GameEngine.random.Next(0, 2);

                while (map[x, y] != null)
                {
                    x = GameEngine.random.Next(0, width);
                    y = GameEngine.random.Next(0, height);
                }

                if (unitType == 0)
                {
                    units[i] = new MeleeUnit(x, y, factions[factionIndex]);
                }
                else
                {
                    units[i] = new RangedUnit(x, y, factions[factionIndex]);
                }
                map[x, y] = units[i].Faction[0] + "/" + units[i].Symbol;
            }
        }

        private void InitializeBuildings()
        {
            buildings = new Building[numBuildings];

            for (int i = 0; i < buildings.Length; i++)
            {
                int x = GameEngine.random.Next(0, width);
                int y = GameEngine.random.Next(0, height);
                int factionIndex = GameEngine.random.Next(0, 2);
                int buildingType = GameEngine.random.Next(0, 2);

                while (map[x, y] != null)
                {
                    x = GameEngine.random.Next(0, width);
                    y = GameEngine.random.Next(0, height);
                }

                if (buildingType == 0)
                {
                    buildings[i] = new ResourceBuilding(x, y, factions[factionIndex]);
                }
                else
                {
                    buildings[i] = new FactoryBuilding(x, y, factions[factionIndex], height);
                }
                map[x, y] = buildings[i].Faction[0] + "/" + buildings[i].Symbol;
            }
        }

        public void AddUnit(Unit unit)
        {
            //We can use Array.Resize, but let's do it ourselves
            Unit[] resizeUnits = new Unit[units.Length + 1];

            for (int i = 0; i < units.Length; i++)
            {
                resizeUnits[i] = units[i];
            }
            resizeUnits[resizeUnits.Length - 1] = unit;
            units = resizeUnits;

            //It would make sense to use List instead - Lists can change size dynamically
        }

        public void AddBuilding(Building building)
        {
            Array.Resize(ref buildings, buildings.Length + 1);
            buildings[buildings.Length - 1] = building;
        }

        public virtual Target GetClosestTarget(Unit unit)
        {
            double closestDistance = int.MaxValue;
            Target closestTarget = null;

            List<Target> targets = new List<Target>();
            targets.AddRange(units);
            targets.AddRange(buildings);

            foreach (Target target in targets)
            {
                //check if this target is a unit and if it is the unit that is seeking a target
                if(target is Unit && (Unit)target == unit) {
                    continue; //continue so that unit doesn't attack itself
                }
                // we don't members of our faction or destroyed units.
                if (target.Faction == unit.Faction || target.IsDestroyed) {
                    continue;
                }

                double distance = unit.GetDistance(target);

                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        public void UpdateMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = "   ";
                }
            }

            foreach (Unit unit in units)
            {
                map[unit.X, unit.Y] = unit.Symbol + "|" + unit.Faction[0];
            }

            foreach (Building building in buildings)
            {
                map[building.X, building.Y] = building.Symbol + "|" + building.Faction[0];
            }
        }

        public string GetMapDisplay()
        {
            string mapString = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mapString += map[x, y];
                }
                mapString += "\n";
            }
            return mapString;
        }

        public void Clear()
        {
            units = new Unit[0];
            buildings = new Building[0];
        }
    }
}
