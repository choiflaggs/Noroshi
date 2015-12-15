using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Noroshi.Server.Daos.Rdb;

namespace SchemaGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var baseDir = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;
            var outputDir = Path.Combine(baseDir.FullName, "Noroshi", "Daos", "Rdb", "Schemas");
            _generateSchemas(MySqlConnectionHandler.Database.Noroshi, outputDir);
            _generateSchemas(MySqlConnectionHandler.Database.NoroshiShard1, outputDir);
        }

        static void _generateSchemas(MySqlConnectionHandler.Database database, string outputDir)
        {
            var typeDictionary = (new MySqlConnectionHandler()).GetSchema(database, "DataTypes")
                .ToDictionary(x => Tuple.Create((string)x.TypeName.ToLower(), (bool?)x.IsUnsigned ?? false), x => (Type)Type.GetType(x.DataType));

            typeDictionary[Tuple.Create("tinyint", true)] = typeof(byte);
            typeDictionary[Tuple.Create("tinyint", false)] = typeof(sbyte);
            typeDictionary[Tuple.Create("smallint", true)] = typeof(ushort);
            typeDictionary[Tuple.Create("smallint", false)] = typeof(ushort);
            typeDictionary[Tuple.Create("float", true)] = typeof(float);

            var tableToPkMap = (new MySqlConnectionHandler()).GetSchema(database, "IndexColumns")
                .Where(x => x.INDEX_NAME == "PRIMARY")
                .ToLookup(x => x.TABLE_NAME);

            var schemaTemplates = (new MySqlConnectionHandler()).GetSchema(database, "Columns")
                .Where(x => !x.TABLE_NAME.Contains("sequential_id"))
                .GroupBy(x => x.TABLE_NAME)
                .Select(g =>
                {
                    var tableName = g.Key;
                    var columns = g
                        .OrderBy(x => x.ORDINAL_POSITION)
                        .Select(x => new
                        {
                            Name = x.COLUMN_NAME,
                            Type = typeDictionary[Tuple.Create(x.DATA_TYPE, x.COLUMN_TYPE.Contains("unsigned"))],
                        })
                        .ToArray();
                    var columnMap = columns.ToDictionary(c => c.Name);
                    var pk = tableToPkMap[(string)tableName]
                        .OrderBy(x => x.ORDINAL_POSITION)
                        .Select(x => new
                        {
                            Name = x.COLUMN_NAME,
                            Type = columnMap[x.COLUMN_NAME].Type,
                        })
                        .ToArray();
                    return new SchemaTemplate(tableName, columns, pk);
                });
            foreach (var schemaTemplate in schemaTemplates)
            {
                schemaTemplate.Save(outputDir);
            }
        }
    }
    public partial class SchemaTemplate
    {
        public string TableName { get; private set; }
        public IEnumerable<dynamic> Columns { get; private set; }
        public IEnumerable<dynamic> PrimaryKey { get; private set; }

        public SchemaTemplate(string tableName, IEnumerable<dynamic> columns, IEnumerable<dynamic> pk)
        {
            TableName = tableName;
            Columns = columns;
            PrimaryKey = pk;
        }

        public string ClassName => _snakeToUpperCamel(TableName) + "Schema";

        string _snakeToUpperCamel(string snake)
        {
            if (string.IsNullOrEmpty(snake))
            {
                return snake;
            }

            return snake
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
        }

        public void Save(string outputDir)
        {
            File.WriteAllText(Path.Combine(outputDir, ClassName + ".cs"), TransformText(), Encoding.UTF8);
        }
    }
}
