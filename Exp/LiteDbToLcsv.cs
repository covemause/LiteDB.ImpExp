using LiteDB;

using System.Text;

namespace LiteDB.Export
{
    internal class LiteDbToLcsv
    {
        string _liteDbPath = "";
        public LiteDbToLcsv(string liteDbPath)
        {
            _liteDbPath = liteDbPath;
        }

        public bool Execute(string lcsvDir)
        {
            if (string.IsNullOrEmpty(_liteDbPath) || string.IsNullOrEmpty(lcsvDir)) return false;

            if (!Directory.Exists(lcsvDir)) return false;

            try
            {
                var buff = new StringBuilder();

                using (var context = new LiteDatabase(_liteDbPath))
                {

                    var collections = context.GetCollectionNames();
                    if (collections == null) return false;

                    foreach (var collection in collections)
                    {
                        var target = context.GetCollection(collection);
                        if (target == null) continue;

                        var records = target.FindAll();

                        buff.AppendLine(collection);

                        var isFirst = false;
                        foreach (var record in records)
                        {
                            if (record.IsArray) throw new NotSupportedException();

                            if (!isFirst)
                            {

                                // Names
                                buff.AppendLine(string.Join(",", record.Keys.ToArray()));

                                // Types
                                buff.AppendLine(string.Join(",", ConvertString(record.Values.ToArray())));

                                isFirst = true;
                            }

                            buff.AppendLine(string.Join(",", record.Values));


                        }

                        using (var writer = new StreamWriter(lcsvDir + "\\" + collection + ".lcsv"))
                        {
                            writer.WriteLine(buff.ToString());
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
        private string[] ConvertString(BsonValue[] values)
        {
            var result = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                switch (values[i].Type)
                {
                    case BsonType.ObjectId:
                        result[i] = "objectid";
                        break;
                    case BsonType.String:
                        result[i] = "string";
                        break;
                    case BsonType.DateTime:
                        result[i] = "datetime";
                        break;
                    case BsonType.Int32:
                        result[i] = "int";
                        break;
                    case BsonType.Double:
                        result[i] = "double";
                        break;
                    case BsonType.Int64:
                        result[i] = "long";
                        break;
                    case BsonType.Boolean:
                        result[i] = "long";
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return result;
        }

    }
}