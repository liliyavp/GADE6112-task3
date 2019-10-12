using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GADE6112___Task3
{
    class GameEngine
    {
        const string A_TEAM = "A-Team";
        const string B_TEAM = "B-Team";
        const string WIZARDS = "Wizards";

        //Share a single Random object across all classes.
        //eg. GameEngine.random.Next(5);
        public static Random random = new Random();

        const string UNITS_FILENAME = "units.txt";
        const string BUIDLINGS_FILENAME = "buildings.txt";
        const string ROUND_FILENAME = "rounds.txt";

        Map map;
        UnitAndBuildingManager manager;

        bool isGameOver = false;
        string winningFaction = "";
        int round = 0;
        string[] factions = { A_TEAM, B_TEAM, WIZARDS};

        int loadedMapWidth; //for loading map width;
        int loadedMapHeight; //for loading map height;

        public GameEngine(int width, int height)
        {
            Reset(width, height);
            //map = new Map(width, height);
            //manager = new UnitAndBuildingManager();
            //CreateUnitsAndBuildings();
        }

        public bool IsGameOver
        {
            get { return isGameOver; }
        }

        public string WinningFaction
        {
            get { return winningFaction; }
        }

        public int Round
        {
            get { return round; }
        }

        public int LoadedMapWidth
        {
            get { return loadedMapWidth;  }
        }

        public int LoadedMapHeight
        {
            get { return loadedMapHeight; }
        }

        public void GameLoop()
        {
            UpdateUnits();
            UpdateBuildings();
            map.UpdateMap(manager);
            round++;
        }

        void UpdateUnits()
        {

            foreach (string faction in factions) {

                string[] ignoreFactions = GetFactionsToIgnore(faction);

                foreach (Unit unit in manager.Units) {
                    //ignore this unit if it is destroyed
                    if (unit.IsDestroyed) {
                        unit.CheckHide();
                        continue;
                    }

                    //unit has to wait a number of rounds equal to speed before it can take action
                    if(round % unit.Speed != 0) {
                        continue;
                    }

                    Target closestTarget = manager.GetClosestTarget(unit, ignoreFactions);

                    if (faction == WIZARDS) {
                        //wizards only target units
                        closestTarget = manager.GetClosestTarget(unit, ignoreFactions, true, false);
                    }

                    if (closestTarget == null) {
                        //if a unit has no target it means the game has ended
                        isGameOver = true;
                        winningFaction = unit.Faction;

                        map.UpdateMap(manager);
                        return;
                    }

                    double healthPercentage = unit.Health / (double)unit.MaxHealth;
                    bool isWizard = unit is WizardUnit;

                    //if not a wizard, run away if health is below 25%
                    if (healthPercentage <= 0.25 && !isWizard) {
                        unit.RunAway();
                    
                    }
                    //if a wizard and health is below 50%, run away
                    else if (healthPercentage <= 0.5 && isWizard) {
                        unit.RunAway();
                    
                    }
                    //if target is in range and this unit is not a wizard, attack closest target
                    else if (unit.IsInRange(closestTarget) && !isWizard) {
                        if (unit.Attack(closestTarget)) {
                            //if killed unit, add to resource pool of all faction resource buildings
                            AddToResourcePoolByFaction(unit.Faction);
                        }
                    
                    }
                    //if target is in range and  this unit is a wizard, attack all targets in range
                    else if (unit.IsInRange(closestTarget) && isWizard) {
                        //find all units close to the target we are attacking
                        foreach(Target targetInArea in manager.GetTargetsInArea(closestTarget, ignoreFactions, true, false)) {
                            unit.Attack(targetInArea);
                        }
                    
                    }
                    //if none of the above just move the unit
                    else {
                        unit.Move(closestTarget);
                    }

                    StayInBounds(unit, map.width, map.height);
                }
            }
        }

        void UpdateBuildings()
        {
            foreach (string faction in factions) {
                //new resources are only considered at the beginning of the next round
                int resources = GetResourcesTotalByFaction(faction);
                int usedResources = 0;

                foreach (Building building in manager.GetBuildingsByFaction(faction)) {
                    //ignore destroyed buildings
                    if (building.IsDestroyed) {
                        building.CheckHide();
                        continue;
                    }

                    if (building is FactoryBuilding) {
                        FactoryBuilding factoryBuilding = (FactoryBuilding)building;

                        if (factoryBuilding.CanProduce(round) && factoryBuilding.SpawnCost <= resources) {
                            resources -= factoryBuilding.SpawnCost;
                            usedResources += factoryBuilding.SpawnCost;

                            Unit newUnit = factoryBuilding.SpawnUnit(round);
                            manager.AddUnit(newUnit);
                        }
                    } else if (building is ResourceBuilding) {
                        ResourceBuilding resourceBuilding = (ResourceBuilding)building;
                        resourceBuilding.GenerateResources();
                    }
                }

                //remove used resources from faction's available resource buildings
                UseResourcesFromFaction(faction, usedResources);
            }
        }

        //calculate how much resources a faction has at it's disposal
        int GetResourcesTotalByFaction(string faction) {
            int totalResources = 0;

            foreach (Building building in manager.GetBuildingsByFaction(faction)) {
                //we are interested in resource buildings that have not been destroyed
                if (building is ResourceBuilding && !building.IsDestroyed) {
                    ResourceBuilding resourceBuilding = (ResourceBuilding)building;
                    totalResources += resourceBuilding.Generated;
                }
            }
            return totalResources;
        }

        void UseResourcesFromFaction(string faction, int usedResources) {
            foreach (Building building in manager.GetBuildingsByFaction(faction)) {
                if (building is ResourceBuilding && !building.IsDestroyed) {
                    ResourceBuilding resourceBuilding = (ResourceBuilding)building;

                    //this buildings has not resources left, let's skip it
                    if(resourceBuilding.Generated <= 0) {
                        continue;
                    }

                    //determine how many resources can be used from this buildings
                    int resourcesToUse = Math.Min(usedResources, resourceBuilding.Generated);

                    //subtract resources from total used by faction
                    usedResources -= resourcesToUse;
                    //subtract resources from actual building
                    resourceBuilding.Generated -= resourcesToUse;

                    //if we've subtracted all the resources use by this faction
                    if(usedResources <= 0) {
                        return;
                    }
                }
            }
        }

        void AddToResourcePoolByFaction(string faction) {
            foreach (Building building in manager.GetBuildingsByFaction(faction)) {
                if (building is ResourceBuilding && !building.IsDestroyed) {
                    ResourceBuilding resourceBuilding = (ResourceBuilding)building;
                    resourceBuilding.Pool += 1;
                }
            }
        }

        string[] GetFactionsToIgnore(string faction) {

            bool canAttackWizards = false;
            if (faction == A_TEAM) {
                canAttackWizards = manager.AllUnitsDestroyed(B_TEAM);
            }else if(faction == B_TEAM) {
                canAttackWizards = manager.AllUnitsDestroyed(A_TEAM);
            }

            string[] ignoreFactions = new string[] { faction, WIZARDS };
            if (faction == WIZARDS || canAttackWizards) {
                //wizards ignore only their own faction 
                //and other units stop ignoring wizards if there are no units alive in the other team
                ignoreFactions = new string[] { faction };
            }

            return ignoreFactions;
        }

        public int NumUnits
        {
            get { return manager.Units.Count; }
        }

        public int NumBuildings
        {
            get { return manager.Buildings.Count; } 
        }

        public int NumUnitsAlive {
            get {
                int alive = 0;
                foreach(Unit unit in manager.Units) {
                    if (!unit.IsDestroyed) {
                        alive++;
                    }
                }
                return alive;
            }
        }

        public int NumBuildingsAlive {
            get {
                int alive = 0;
                foreach (Building building in manager.Buildings) {
                    if (!building.IsDestroyed) {
                        alive++;
                    }
                }
                return alive;
            }
        }

        public string MapDisplay
        {
            get { return map.GetMapDisplay(); }
        }

        public string GetUnitInfo()
        {
            string unitInfo = "";
            foreach (Unit unit in manager.Units)
            {
                unitInfo += unit + Environment.NewLine;
            }
            return unitInfo;
        }

        public string GetBuildingsInfo()
        {
            string buildingsInfo = "";
            foreach (Building building in manager.Buildings)
            {
                buildingsInfo += building + Environment.NewLine;
            }
            return buildingsInfo;
        }

        public string GetDetails() {
            string details = "";

            foreach(string faction in factions) {
                details += faction + Environment.NewLine;
                details += "------------------" + Environment.NewLine;
                details += "Units: " + 
                    manager.GetUnitsAliveCountByFaction(faction) + "/" + 
                    manager.GetUnitsByFaction(faction).Count + Environment.NewLine;
                details += "Buildings: " + 
                    manager.GetBuildingsAliveCountByFaction(faction) + "/" + 
                    manager.GetBuildingsByFaction(faction).Count + Environment.NewLine;
                details += Environment.NewLine;
            }

            return details;
        }

        public void Reset(int width, int height)
        {
            map = new Map(width, height);
            manager = new UnitAndBuildingManager();

            CreateUnitsAndBuildings();
            map.UpdateMap(manager);

            isGameOver = false;
            round = 0;
        }

        public void SaveGame()
        {
            Save(UNITS_FILENAME, manager.Units.ToArray());
            Save(BUIDLINGS_FILENAME, manager.Buildings.ToArray());
            SaveSettings();
        }

        public void LoadGame()
        {
            LoadSettings();

            map = new Map(loadedMapWidth, loadedMapHeight);
            manager = new UnitAndBuildingManager();
            foreach (string faction in factions) {
                manager.AddFaction(faction);
            }

            Load(UNITS_FILENAME);
            Load(BUIDLINGS_FILENAME);

            map.UpdateMap(manager);
        }

        private void Load(string filename)
        {
            FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;

            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                int length = recordIn.IndexOf(",");

                string firstField = recordIn.Substring(0, length);

                switch (firstField)
                {
                    case "Melee": manager.AddUnit(new MeleeUnit(recordIn)); break;
                    case "Ranged": manager.AddUnit(new RangedUnit(recordIn)); break;
                    case "Wizard": manager.AddUnit(new WizardUnit(recordIn)); break;
                    case "Factory": manager.AddBuilding(new FactoryBuilding(recordIn)); break;
                    case "Resource": manager.AddBuilding(new ResourceBuilding(recordIn)); break;
                }

                recordIn = reader.ReadLine();
            }
            reader.Close();
            inFile.Close();
        }

        private void Save(string filename, object[] objects)
        {
            FileStream outFile = new FileStream(filename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            foreach (object obj in objects)
            {
                if (obj is Unit)
                {
                    Unit unit = (Unit)obj;
                    writer.WriteLine(unit.Save());
                }
                else if (obj is Building)
                {
                    Building unit = (Building)obj;
                    writer.WriteLine(unit.Save());
                }
            }
            writer.Close();
            outFile.Close();
        }

        private void SaveSettings()
        {
            FileStream outFile = new FileStream(ROUND_FILENAME, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            writer.WriteLine(round);
            writer.WriteLine(map.width);
            writer.WriteLine(map.height);
            writer.Close();
            outFile.Close();
        }

        private void LoadSettings()
        {
            FileStream inFile = new FileStream(ROUND_FILENAME, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            round = int.Parse(reader.ReadLine());
            loadedMapWidth = int.Parse(reader.ReadLine());
            loadedMapHeight = int.Parse(reader.ReadLine());
            reader.Close();
            inFile.Close();
        }

        private void CreateUnitsAndBuildings() {
            foreach (string faction in factions) {
                manager.AddFaction(faction);

                if (faction != WIZARDS) {
                    AddUnits(random.Next(5, 10), faction);
                    AddBuildings(random.Next(10, 15), faction);
                } else {
                    AddUnits(random.Next(5, 10), faction);
                }
            }
        }

        private void AddUnits(int numUnits, string faction) {
            for (int i = 0; i < numUnits; i++) {
                int x = random.Next(0, map.width);
                int y = random.Next(0, map.height);

                Unit unit;
                if (faction != WIZARDS) {
                    int unitType = random.Next(0, 2);
                    if (unitType == 0) {
                        unit = new MeleeUnit(x, y, faction);
                    } else {
                        unit = new RangedUnit(x, y, faction);
                    }
                } else {
                    unit = new WizardUnit(x, y, faction);
                }

                manager.AddUnit(unit);
            }
        }

        private void AddBuildings(int numBuildings, string faction) {
            for (int i = 0; i < numBuildings; i++) {
                int x = random.Next(0, map.width);
                int y = random.Next(0, map.height);
                int buildingType = random.Next(0, 2);

                Building building;
                if (buildingType == 0) {
                    building = new FactoryBuilding(x, y, faction, map.height);
                } else {
                    building = new ResourceBuilding(x, y, faction);
                }

                manager.AddBuilding(building);
            }
        }

        private void StayInBounds(Unit unit, int width, int height)
        {
            if (unit.X < 0)
            {
                unit.X = 0;
            }
            else if (unit.X >= width)
            {
                unit.X = width - 1;
            }

            if (unit.Y < 0)
            {
                unit.Y = 0;
            }
            else if (unit.Y >= height)
            {
                unit.Y = height - 1;
            }
        }
    }
}
