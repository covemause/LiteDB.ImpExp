namespace LiteDB.Export
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("file not found." + args[0]);
                    return;
                }

                var exportor = new LiteDbToLcsv(args[0]);
                if (exportor.Execute(args[1]))
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
            Console.WriteLine(" > LiteDb.Export.exe [LiteDB.db] [output lcsv-file dir.]");
        }
    }
}
