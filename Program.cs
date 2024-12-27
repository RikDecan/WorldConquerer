namespace ConsoleAppSquareMaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            World world = new World();
            var w=world.BuildWorld2(100,100,0.60);
            //var w = world.BuildWorld1(100, 100);
            for (int i = 0; i < w.GetLength(1); i++)
            {
                for (int j = 0; j < w.GetLength(0); j++)
                {
                    char ch;
                    if (w[j, i]) ch = '*'; else ch = ' ';
                    Console.Write(ch);
                }
                Console.WriteLine();
            }
            WorldConquer wq = new WorldConquer(w);
            //var ww = wq.Conquer3(5, 25000);
            //for (int i = 0; i < ww.GetLength(1); i++)
            //{
            //    for (int j = 0; j < ww.GetLength(0); j++)
            //    {
            //        string ch;
            //        switch (ww[j, i])
            //        {
            //            case -1: ch = " "; break;
            //            case 0: ch = "."; break;
            //            default: ch = ww[j, i].ToString(); break;
            //        }
            //        Console.Write(ch);
            //    }
            //    Console.WriteLine();
            //}
            //BitmapWriter bmw = new BitmapWriter();
            //bmw.DrawWorld(ww);
        }
    }
}
