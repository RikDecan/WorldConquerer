using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster
{
    public class WorldConquer
    {
        /* world indicates whether the grid cell on coordinate x,y is part of the world or not*/
        private bool[,] world;
        /* the values in worldempires are -1 if not part of the world, 0 if part of the world but not conquered by any empire, any other positive value indicates the empire (id) the grid cell belongs to
         */
        private int[,] worldempires;
        private int maxx, maxy;
        private Random random = new Random(1);

        public WorldConquer(bool[,] world)
        {
            this.world = world;
            maxx = world.GetLength(0);
            maxy = world.GetLength(1);
            worldempires = new int[maxx,maxy];
            for (int i = 0; i < world.GetLength(0); i++) for (int j = 0; j < world.GetLength(1); j++) if (world[i, j]) worldempires[i, j] = 0; else worldempires[i, j] = -1;
        }
        /*
         * nEmpires indicates the number of empires who will try to conquer the world
         * it is a turned based algorithme, every turn each empire will try to expand
         * on each turn, 
         *          for each empire a random location owned by the empire is selected
         *          a new location is selected by picking a random adjacent location
         *          if that location is within the boundary of the map and not occupied by an empire, the location is assigned to the empire
         */
        public int[,] Conquer1(int nEmpires, int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new(); //key is the empire id, value is the list of cells (x,y) the empire controls
            //search random start positions of each empire
            //start positions must be located on the world and each empire requires a different start position
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            int direction;//0-right,1-left,2-top,3-bottom
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    index = random.Next(empires[e].Count);
                    direction = random.Next(4);
                    x = empires[e][index].Item1;
                    y = empires[e][index].Item2;
                    switch (direction)
                    {
                        case 0:
                            if (x < maxx - 1 && worldempires[x + 1, y] == 0)
                            {
                                worldempires[x + 1, y] = e;
                                empires[e].Add((x + 1, y));
                            }
                            break;
                        case 1:
                            if (x > 0 && worldempires[x - 1, y] == 0)
                            {
                                worldempires[x - 1, y] = e;
                                empires[e].Add((x - 1, y));
                            }
                            break;
                        case 2:
                            if (y < maxy - 1 && worldempires[x, y + 1] == 0)
                            {
                                worldempires[x, y + 1] = e;
                                empires[e].Add((x, y + 1));
                            }
                            break;
                        case 3:
                            if (y > 0 && worldempires[x, y - 1] == 0)
                            {
                                worldempires[x, y - 1] = e;
                                empires[e].Add((x, y - 1));
                            }
                            break;
                    }
                }
            }
            return worldempires;
        }
        /*
         * nEmpires indicates the number of empires who will try to conquer the world
         * it is a turned based algorithme, every turn each empire will try to expand
         * on each turn, 
         *          for each empire a random location owned by the empire is selected
         *          a new location is selected by searching a free adjacent location
         *          if that location is within the boundary of the map and not occupied by an empire, the location is assigned to the empire
         */
        public int[,] Conquer3(int nEmpires, int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new();//key is the empire id, value is the list of cells (x,y) the empire controls
            //search random start positions of each empire
            //start positions must be located on the world and each empire requires a different start position
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    index = random.Next(empires[e].Count);
                    pickEmpty(empires[e], index, e);
                }
            }
            return worldempires;
        }
        /* e is the id of the empire
         * index is the selected location within the empire
         * empire contains the locations owned by the empire
         * each of the adjacent locations is checked and if the adjacent location is not occupied and part of the world it is added to the list n
         * out of these free locations a free location is selected
         */
        private void pickEmpty(List<(int,int)> empire,int index,int e)
        {
            //search neighbours
            List<(int, int)> n = new List<(int, int)>();
            if (IsValidPosition(empire[index].Item1-1, empire[index].Item2)
                && (worldempires[empire[index].Item1 - 1, empire[index].Item2]==0)) n.Add((empire[index].Item1-1, empire[index].Item2));
            if (IsValidPosition(empire[index].Item1+1, empire[index].Item2)
                && (worldempires[empire[index].Item1 + 1, empire[index].Item2] == 0)) n.Add((empire[index].Item1+1, empire[index].Item2));
            if (IsValidPosition(empire[index].Item1, empire[index].Item2-1)
                && (worldempires[empire[index].Item1, empire[index].Item2-1] == 0)) n.Add((empire[index].Item1, empire[index].Item2-1));
            if (IsValidPosition(empire[index].Item1, empire[index].Item2+1)
                && (worldempires[empire[index].Item1, empire[index].Item2+1] == 0)) n.Add((empire[index].Item1, empire[index].Item2+1));
            int x = random.Next(n.Count);
            if (n.Count > 0)
            {
                empire.Add(n[x]);
                worldempires[n[x].Item1, n[x].Item2] = e;
            }
        }
        /*
         * nEmpires indicates the number of empires who will try to conquer the world
         * it is a turned based algorithme, every turn each empire will try to expand
         * on each turn, 
         *          for each empire the locations with the most free adjacent locations are selected
         *          out of this list a random location of the empire is selected
         *          a new location is selected by picking a random adjacent location
         *          if that location is within the boundary of the map and not occupied by an empire, the location is assigned to the empire
         */
        public int[,] Conquer2(int nEmpires,int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new();//key is the empire id, value is the list of cells (x,y) the empire controls
            //search random start positions of each empire
            //start positions must be located on the world and each empire requires a different start position
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            int direction;//0-right,1-left,2-top,3-bottom
            for (int i = 0; i < turns; i++)
            {
                for(int e = 1; e <= nEmpires; e++)
                {
                    index =FindWithMostEmptyNeighbours(e, empires[e]);
                    direction = random.Next(4);
                    x=empires[e][index].Item1;
                    y=empires[e][index].Item2;
                    switch (direction)
                    {
                        case 0:
                            if (x<maxx-1 && worldempires[x+1,y]==0)
                            {
                                worldempires[x + 1, y] = e;
                                empires[e].Add((x+1, y));
                            }
                            break;
                        case 1:
                            if (x>0 && worldempires[x - 1, y] == 0)
                            {
                                worldempires[x - 1, y] = e;
                                empires[e].Add((x - 1, y));
                            }
                            break;
                        case 2:
                            if (y < maxy - 1 && worldempires[x, y+1] == 0)
                            {
                                worldempires[x, y+1] = e;
                                empires[e].Add((x, y+1));
                            }
                            break;
                        case 3:
                            if (y >0 && worldempires[x, y-1] == 0)
                            {
                                worldempires[x, y-1] = e;
                                empires[e].Add((x, y-1));
                            }
                            break;
                    }
                }
            }
            return worldempires;
        }
        /* e is the id of the empire
         * empire contains a list of locations (x,y) owned by the empire
         * for each location the number of not-occupied adjacent locations is calculated
         * the list of indexes contains all locations with the max score
         * out of these locations a random location will be selected
         */
        private int FindWithMostEmptyNeighbours(int e, List<(int, int)> empire)
        {            
            List<int> indexes= new List<int>();
            int n = 0;
            int calcN;
            for (int i = 0; i < empire.Count; i++)
            {
                calcN = EmptyNeighbours(e, empire[i].Item1, empire[i].Item2);
                if (calcN >= n)
                {
                    indexes.Clear();
                    n= calcN;
                    indexes.Add(i);
                }
            }
            return indexes[random.Next(indexes.Count)];
        }
        /* counts the number of not-occupied locations adjacent to the x,y-coordinates */
        private int EmptyNeighbours(int empire,int x,int y)
        {
            int n = 0;
            if (IsValidPosition(x-1,y) && worldempires[x-1, y] == 0) n++; //empty square
            if (IsValidPosition(x+1, y) && worldempires[x + 1, y] == 0) n++;
            if (IsValidPosition(x, y-1) && worldempires[x, y-1] == 0) n++;
            if (IsValidPosition(x, y+1) && worldempires[x, y+1] == 0) n++;
            return n;
        }
        /* checks if the location is within the boundary of the map */
        private bool IsValidPosition(int x, int y)
        {
            if (x<0) return false;
            if (x >= world.GetLength(0)) return false;
            if (y<0) return false;
            if (y>= world.GetLength(1)) return false;
            return true;
        }
    }
}
