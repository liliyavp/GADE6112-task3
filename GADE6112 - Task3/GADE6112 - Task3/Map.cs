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
        //can only assign readonly here or in constructor

        string[,] map;

        public Map(int width, int height) {
            //NOTE: no more numUnits, numBuildings
            //separated maps to display, and managers into separate class
            this.width = width;
            this.height = height;
            map = new string[width, height];
            //sets width and height and creates map string
        }

        public void UpdateMap(UnitAndBuildingManager manager)
            //passes manager class
        {
            for (int y = 0; y < height; y++) { //height and width instead of y and x in all code
                for (int x = 0; x < width; x++) {
                    map[x, y] = "   ";
                }
            }

            foreach (Unit unit in manager.Units) {
                if (unit.IsVisible) { //if visible, draw it. After a while, dead things disappear
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
            //puts str arr into single string then returns to display later
        }
    }
}
//purpose of entire code just to display