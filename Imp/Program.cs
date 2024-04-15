namespace LiteDB.Import
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2) 
            {
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("file not found." + args[1]);
                    return;
                }

                var Importor = new LcsvToLiteDb(args[0]);
                if (Importor.Execute(args[1]))
                {
                    Console.WriteLine("Completed!");
                }
                else
                {
                    Console.WriteLine("Failed!");
                }
                return;
            }

            Console.WriteLine("invalid argument.");
            Console.WriteLine("");
            Console.WriteLine(" > LiteDb.Import.exe [LiteDB.db] [import.lcsv]");

        }
    }
}
