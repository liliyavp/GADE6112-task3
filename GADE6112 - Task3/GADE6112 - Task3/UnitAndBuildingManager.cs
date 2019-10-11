using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3 {
    class UnitAndBuildingManager {

        //dictionary that stores a list of units using their faction as a key
        Dictionary<string, List<Unit>> units;

        //dictionary that stores a list of buildings using their faction as a key
        Dictionary<string, List<Building>> buildings;

        List<string> factions = new List<string>();

        public UnitAndBuildingManager() {
            units = new Dictionary<string, List<Unit>>();
            buildings = new Dictionary<string, List<Building>>();

            foreach (string faction in factions) {
                units[faction] = new List<Unit>();
                buildings[faction] = new List<Building>();
            }
        }

        public void AddFaction(string faction) {
            if (factions.Contains(faction)) {
                return;
            }

            factions.Add(faction);
            units[faction] = new List<Unit>();
            buildings[faction] = new List<Building>();
        }

        public List<Unit> Units {
            get { return GetUnits(); }
        }

        public List<Building> Buildings {
            get { return GetBuildings(); }
        }

        List<Unit> GetUnits() {
            List<Unit> allUnits = new List<Unit>();
            foreach (KeyValuePair<string, List<Unit>> factionUnits in units) {
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

        public void AddUnit(Unit unit) {
            units[unit.Faction].Add(unit);
        }

        public void AddBuilding(Building building) {
            buildings[building.Faction].Add(building);
        }

        public virtual Target GetClosestTarget(Unit unit, string[] ignoreFactions, bool includeUnits = true, bool includeBuildings = true) {
            double closestDistance = int.MaxValue;
            Target closestTarget = null;

            List<Target> targets = GetPossibleTargets(ignoreFactions, includeUnits, includeBuildings);

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

        public List<Target> GetTargetsInArea(Target closestTarget, string[] ignoreFactions, bool includeUnits = true, bool includeBuildings = true) {

            List<Target> targets = new List<Target>();
            targets.Add(closestTarget);
           
            List<Target> possibleTargets = GetPossibleTargets(ignoreFactions, includeUnits, includeBuildings);

            foreach (Target target in possibleTargets) {
                //skip target if it is destroyed
                if (target.IsDestroyed || target == closestTarget) {
                    continue;
                }
                //skip target if it falls out of x range
                if(target.X < closestTarget.X - 1 || target.X > closestTarget.X + 1) {
                    continue;
                }
                //skip target if it falls out of y range
                if (target.Y < closestTarget.Y - 1 || target.Y > closestTarget.Y + 1) {
                    continue;
                }
                //if we're including units and this target is a unit
                if(target is Unit && includeUnits) {
                    targets.Add(target);
                }
                //if we're including buildings and this target is a building
                if (target is Building && includeBuildings) {
                    targets.Add(target);
                }
            }
            return targets;
        }

        private List<Target> GetPossibleTargets(string[] ignoreFactions, bool includeUnits, bool includeBuildings) {
            //create a list of all the targets not in ignored factions
            List<Target> targets = new List<Target>();

            foreach (string faction in factions) {
                //targets in the ignore list are skipped
                if (Array.IndexOf(ignoreFactions, faction) >= 0) {
                    continue;
                }
                //include units in list if includeUnits is set to true
                if (includeUnits) {
                    targets.AddRange(units[faction]);
                }
                //include buildings in list if includeUnits is set to true
                if (includeBuildings) {
                    targets.AddRange(buildings[faction]);
                }
            }
            return targets;
        }

        public List<Unit> GetUnitsByFaction(string faction) {
            return units[faction];
        }

        public List<Building> GetBuildingsByFaction(string faction) {
            return buildings[faction];
        }



        public int GetUnitsAliveCountByFaction(string faction) {
            int count = 0;
            foreach (Unit unit in units[faction]) {
                if (!unit.IsDestroyed) {
                    count++;
                }
            }
            return count;
        }

        public int GetBuildingsAliveCountByFaction(string faction) {
            int count = 0;
            foreach (Building building in buildings[faction]) {
                if (!building.IsDestroyed) {
                    count++;
                }
            }
            return count;
        }

        public bool AllUnitsDestroyed(string faction) {
            foreach(Unit unit in units[faction]) {
                if (!unit.IsDestroyed) {
                    return false;
                }
            }

            return true;
        }
    }
}
