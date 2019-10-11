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

        string[,] map;

        public Map(int width, int height) {

            this.width = width;
            this.height = height;
            map = new string[width, height];
        }

        public void UpdateMap(UnitAndBuildingManager manager)
        {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    map[x, y] = "   ";
                }
            }

            foreach (Unit unit in manager.Units) {
                if (unit.IsVisible) {
                    map[unit.X, unit.Y] = unit.Symbol + "|" + unit.Faction[0];
                }
            }

            foreach (Building building in manager.Buildings) {
                if (building.IsVisible) {
                    map[building.X, building.Y] = building.Symbol + "|" + building.Faction[0];
                }
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
    }
}
