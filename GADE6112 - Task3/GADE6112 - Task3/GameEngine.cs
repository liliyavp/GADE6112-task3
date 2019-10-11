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
        //Share a single Random object across all classes.
        //eg. GameEngine.random.Next(5);
        public static Random random = new Random();

        const string UNITS_FILENAME = "units.txt";
        const string BUIDLINGS_FILENAME = "buildings.txt";
        const string ROUND_FILENAME = "rounds.txt";

        Map map;
        bool isGameOver = false;
        string winningFaction = "";
        int round = 0;

        int loadedMapWidth; //for loading map width;
        int loadedMapHeight; //for loading map height;

        public GameEngine(int width, int height)
        {
            map = new Map(width, height, 10, 10);
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
            map.UpdateMap();
            round++;
        }

        void UpdateUnits()
        {
            foreach (Unit unit in map.Units)
            {
                //ignore this unit if it is destroyed
                if (unit.IsDestroyed) {
                    continue;
                }

                Target closestTarget = map.GetClosestTarget(unit);
                if (closestTarget == null) {
                    //if a unit has no target it means the game has ended
                    isGameOver = true;
                    winningFaction = unit.Faction;
                    map.UpdateMap();
                    return;
                }

                double healthPercentage = unit.Health / unit.MaxHealth;
                //units don't run away from buildings
                if (healthPercentage <= 0.25 && closestTarget is Unit) {
                    unit.RunAway();
                }
                else if (unit.IsInRange(closestTarget)) {
                    unit.Attack(closestTarget);
                }
                else {
                    unit.Move(closestTarget);
                }
                StayInBounds(unit, map.width, map.height);
            }
        }

        void UpdateBuildings()
        {
            foreach (Building building in map.Buildings)
            {

                if (building is FactoryBuilding)
                {
                    FactoryBuilding factoryBuilding = (FactoryBuilding)building;

                    if (round % factoryBuilding.ProductionSpeed == 0)
                    {
                        Unit newUnit = factoryBuilding.SpawnUnit();
                        map.AddUnit(newUnit);
                    }
                }
                else if (building is ResourceBuilding)
                {
                    ResourceBuilding resourceBuilding = (ResourceBuilding)building;
                    resourceBuilding.GenerateResources();
                }
            }
        }

        public int NumUnits
        {
            get { return map.Units.Length; }
        }

        public int NumBuildings
        {
            get { return map.Buildings.Length; }
        }

        public int NumUnitsAlive {
            get {
                int alive = 0;
                foreach(Unit unit in map.Units) {
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
                foreach (Building building in map.Buildings) {
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
            foreach (Unit unit in map.Units)
            {
                unitInfo += unit + Environment.NewLine;
            }
            return unitInfo;
        }

        public string GetBuildingsInfo()
        {
            string buildingsInfo = "";
            foreach (Building building in map.Buildings)
            {
                buildingsInfo += building + Environment.NewLine;
            }
            return buildingsInfo;
        }

        public void Reset(int width, int height)
        {
            map = new Map(width, height, 10, 10);
            isGameOver = false;
            round = 0;
        }

        public void SaveGame()
        {
            Save(UNITS_FILENAME, map.Units);
            Save(BUIDLINGS_FILENAME, map.Buildings);
            SaveSettings();
        }

        public void LoadGame()
        {
            LoadSettings();
            map = new Map(loadedMapWidth, loadedMapHeight);
            Load(UNITS_FILENAME);
            Load(BUIDLINGS_FILENAME);
            map.UpdateMap();
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
                    case "Melee": map.AddUnit(new MeleeUnit(recordIn)); break;
                    case "Ranged": map.AddUnit(new RangedUnit(recordIn)); break;
                    case "Factory": map.AddBuilding(new FactoryBuilding(recordIn)); break;
                    case "Resource": map.AddBuilding(new ResourceBuilding(recordIn)); break;
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
