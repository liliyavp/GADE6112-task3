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

        //dictionary that stores a list of units using their faction as a key
        Dictionary<string, List<Unit>> units; 

        //dictionary that stores a list of buildings using their faction as a key
        Dictionary<string, List<Building>> buildings; 

        string[,] map;
        string[] factions = {};

        int numUnits;
        int numBuildings;

        public Map(int width, int height, string[] factions, int numUnits = 0, int numBuildings = 0) {
            this.numUnits = numUnits;
            this.numBuildings = numBuildings;
            this.factions = factions;

            this.width = width;
            this.height = height;

            map = new string[width, height];
            units = new Dictionary<string, List<Unit>>();
            buildings = new Dictionary<string, List<Building>>();

            foreach(string faction in factions) {
                units[faction] = new List<Unit>();
                buildings[faction] = new List<Building>();
            }

            InitializeUnits();
            InitializeBuildings();
            UpdateMap();
        }

        public List<Unit> Units
        {
            get { return GetUnits(); }
        }

        public List<Building> Buildings
        {
            get { return GetBuildings(); }
        }

        List<Unit> GetUnits() {
            List<Unit> allUnits = new List<Unit>();
            foreach(KeyValuePair<string, List<Unit>> factionUnits in units) {
                allUnits.AddRange(factionUnits.Value);
            }
            return allUnits;
        }

        List<Building> GetBuildings() {
            List<Building> allBuildings = new List<Building>();
            foreach (KeyValuePair<string, List<Building>> factionBuildings in buildings) {
                allBuildings.AddRange(factionBuildings.Value);
            }
            return allBuildings;
        }

        private void InitializeUnits()
        {
            for (int i = 0; i < numUnits; i++)
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

                Unit unit;
                if (unitType == 0) {
                    unit = new MeleeUnit(x, y, factions[factionIndex]);
                }
                else {
                     unit = new RangedUnit(x, y, factions[factionIndex]);
                }
                units[factions[factionIndex]].Add(unit);
                map[x, y] =  unit.Faction[0] + "/" + unit.Symbol;
            }
        }

        private void InitializeBuildings()
        {
            for (int i = 0; i < numBuildings; i++)
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

                Building building;
                if (buildingType == 0)
                {
                    building = new ResourceBuilding(x, y, factions[factionIndex]);
                }
                else
                {
                    building = new FactoryBuilding(x, y, factions[factionIndex], height);
                }
                buildings[factions[factionIndex]].Add(building);
                map[x, y] = building.Faction[0] + "/" + building.Symbol;
            }
        }

        public void AddUnit(Unit unit)
        {
            units[unit.Faction].Add(unit);
        }

        public void AddBuilding(Building building)
        {
            buildings[building.Faction].Add(building);
        }

        public virtual Target GetClosestTarget(Unit unit, string[] ignoreFactions)
        {
            double closestDistance = int.MaxValue;
            Target closestTarget = null;

            //create a list of all the targets not in the unit's faction
            List<Target> targets = new List<Target>();
            foreach(string faction in factions) {
                //targets of our own faction or in the ignore list are skipped
                if(faction == unit.Faction || Array.IndexOf(ignoreFactions, faction) >= 0) {
                    continue;
                }
                targets.AddRange(units[faction]);
                targets.AddRange(buildings[faction]);
            }

            //no need to check if we are attacking units or buildings of our own faction
            //since we now only have a list of targets from other factions
            foreach (Target target in targets) {
                if (target.IsDestroyed) {
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

        public List<Unit> GetUnitsByFaction(string faction) {
            return units[faction];
        }

        public List<Building> GetBuildingsByFaction(string faction) {
            return buildings[faction];
        }

        public void UpdateMap()
        {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    map[x, y] = "   ";
                }
            }

            foreach (Unit unit in Units) {
                map[unit.X, unit.Y] = unit.Symbol + "|" + unit.Faction[0];
            }

            foreach (Building building in Buildings) {
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
            units = new Dictionary<string, List<Unit>>();
            buildings = new Dictionary<string, List<Building>>();
        }
    }
}
