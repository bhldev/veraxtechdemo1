using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateDemo
{
	/// <summary>
	/// Sample code generator
	/// </summary>
	public class Generator
	{

		public string ConnectionString { get; set; }
		public string TableTemplate { get; set; }
		public string TableMetadataQuery { get; set; }
		public string Namespace { get; set; }

		public Generator(string connectionString)
		{
			ConnectionString = connectionString;
			
			TableTemplate = @"
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace [namespace]
{
	public class [className]DAO
	{

		public string ConnectionString { get; set; }		

		protected const string SelectQuery = ""SELECT * FROM [tableName];"";

		public [className] (string connectionString)
		{
			ConnectionString = connectionString;
		}

		public DataTable Get[tableName]()
		{
			DataTable metadata = new DataTable();
			using (SqlDataAdapter adapter = new SqlDataAdapter(SelectQuery, ConnectionString))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.Add;
				adapter.FillLoadOption = LoadOption.OverwriteChanges;
				adapter.Fill(metadata);
			}
			return metadata;
		}
	}
}";

			TableMetadataQuery = @"SELECT TABLE_NAME ""TABLENAME"" FROM information_schema.tables WHERE TABLE_CATALOG = """";";
			Namespace = @"Verax.Data";
		}

		public void Generate()
		{
			DataTable metadata = ReadMetadata();
			foreach (DataRow row in metadata.Rows)
			{
				string tableString = CreateTable(row["TABLENAME"].ToString());
				WriteFile(tableString, row["TABLENAME"].ToString() + "DAO");
			}
		}

		protected string CreateTable(string tableName)
		{
			return TableTemplate.Replace("[namespace]", Namespace).
				Replace("[tableName]", tableName).
				Replace("[className]", tableName);
		}

		protected void WriteFile(string text, string fileName)
		{
			using (StreamWriter file = new StreamWriter(fileName + ".cs"))
			{
				file.Write(text);
			}
		}

		protected DataTable ReadMetadata()
		{
			DataTable metadata = new DataTable();
			using (SqlDataAdapter adapter = new SqlDataAdapter(TableMetadataQuery, ConnectionString))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.Add;
				adapter.FillLoadOption = LoadOption.OverwriteChanges;
				adapter.Fill(metadata);
			}
			return metadata;
		}

	}
}
