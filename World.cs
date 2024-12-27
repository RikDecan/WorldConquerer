using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster
{
    public class World
    {
        private Random random = new Random();
        private int maxRandom = 10;
        private int chanceExtra = 6;
        private int chanceLess = 3;
   
        public bool[,] BuildWorld1(int maxy,int maxx)
        {          
            int dx=maxx;
            int dy=maxy;
            bool[,] squares=new bool[dx,dy];
            int y1 = random.Next(dy);
            int y2 = random.Next(dy);
            int yb = Math.Min(y1,y2);
            int ye = Math.Max(y1,y2);
            for (int i = 0; i < dx; i++)
            {
                for(int j=yb;j<ye;j++) squares[i,j] = true;
                switch (build())
                {
                    case 1: if (yb>0) yb--; break;
                    case -1: if (yb<maxy) yb++; break;
                }
                switch (build())
                {
                    case 1: if (ye<maxy) ye++; break;
                    case -1: if (ye>0) ye--; break;
                }
                if (ye < yb) break;
            }
            return squares;
        }

        public bool[,] BuildWorld2(int maxy,int maxx,double coverage)
        {
            
            bool[,] squares = new bool[maxx, maxy];
            int seeds = 5;
            int coverageRequired =(int)(coverage*maxx*maxy);
            int currentCoverage = 0;
            int x, y;
            List<(int, int)> list = new();
            for(int i = 0;i<seeds;i++)
            {
                x=random.Next(maxx); y=random.Next(maxy);
                if (!list.Contains((x, y))) { list.Add((x, y)); currentCoverage++; squares[x, y] = true; }

            }
            int index;
            int direction;
            while(currentCoverage < coverageRequired)
            {
                index=random.Next(list.Count);
                direction = random.Next(4);
                switch (direction)
                {
                    case 0:
                        if ((list[index].Item1 < maxx - 1) && !squares[list[index].Item1 + 1, list[index].Item2]) 
                        {
                            squares[list[index].Item1 + 1, list[index].Item2] = true;
                            list.Add((list[index].Item1 + 1, list[index].Item2));
                            currentCoverage++; 
                        }
                        break;
                    case 1:
                        if ((list[index].Item1 > 0) && !squares[list[index].Item1 - 1, list[index].Item2])
                        {
                            squares[list[index].Item1 - 1, list[index].Item2] = true;
                            list.Add((list[index].Item1 - 1, list[index].Item2));
                            currentCoverage++;
                        }
                        break;
                    case 2:
                        if ((list[index].Item2 < maxy - 1) && !squares[list[index].Item1, list[index].Item2 + 1])
                        {
                            squares[list[index].Item1, list[index].Item2 + 1]=true;
                            list.Add((list[index].Item1, list[index].Item2 + 1));
                            currentCoverage++;
                        }
                        break;
                    case 3:
                        if ((list[index].Item2 > 0) && !squares[list[index].Item1, list[index].Item2 - 1])
                        {
                            squares[list[index].Item1, list[index].Item2 - 1] = true;
                            list.Add((list[index].Item1, list[index].Item2 - 1));
                            currentCoverage++;
                        }
                        break;
                }
            }            
            return squares;
        }
        private int build()
        {
            int x = random.Next(maxRandom);
            if (x > chanceExtra) return 1;
            if (x < chanceLess) return -1;
            return 0;
        }
    }
}
