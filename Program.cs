namespace ConsoleAppSquareMaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            World world = new World();
            var w=world.BuildWorld2(120,120,0.45);
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

        }
    }
}
