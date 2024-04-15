using LiteDB;

namespace LiteDB.Import
{
    internal class LcsvToLiteDb
    {
        string _liteDbPath = "";

        public LcsvToLiteDb(string liteDbPath)
        {
            _liteDbPath = liteDbPath;
        }

        public bool Execute(string lcsvPath)
        {
            if (string.IsNullOrEmpty(_liteDbPath) || string.IsNullOrEmpty(lcsvPath)) return false;

            try
            {
                using (var context = new LiteDatabase(_liteDbPath))
                {
                    using (StreamReader reader = new StreamReader(lcsvPath))
                    {

                        var collectionName = reader.ReadLine();
                        if (collectionName == null) { return false; }

                        var columnStrings = reader.ReadLine();
                        if (columnStrings == null) { return false; }

                        var columnTypeStrings = reader.ReadLine();
                        if (columnTypeStrings == null) { return false; }

                        // Start importing data
                        var target = context.GetCollection(collectionName);
                        if (target.Count() > 0) { target.DeleteAll(); }


                        var columns = columnStrings.Split(',');
                        if (columns == null || columns.Length < 1) { return false; }

                        var columnTypes = columnTypeStrings.Split(',');
                        if (columnTypes == null || columnTypes.Length < 1) { return false; }

                        if (columns.Length != columnTypes.Length) { return false; }

                        var convertTypes = ConvertTypes(columnTypes);

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line == null) continue;

                            string[] values = line.Split(',');

                            var doc = new LiteDB.BsonDocument();
                            for (var i = 0; i < columns.Length; i++)
                            {
                                doc.Add(columns[i], new BsonValue(Convert.ChangeType(values[i], convertTypes[i])));
                            }

                            target.Insert(doc);
                        }
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private Type[] ConvertTypes(string[] strings)
        {
            var types = new Type[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                switch(strings[i].ToLower())
                {
                    case "string":
                        types[i] = typeof(string);
                        break;
                    case "datetime":
                        types[i] = typeof(DateTime);
                        break;
                    case "int":
                        types[i] = typeof(int);
                        break;
                    case "double":
                        types[i] = typeof(double);
                        break;
                    case "long":
                        types[i] = typeof(long);
                        break;
                    case "bool":
                        types[i] = typeof(bool);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return types;
        }
    }
}
