using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			Generator gen = new Generator(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
			gen.Namespace = ConfigurationManager.AppSettings["Namespace"];
			gen.TableMetadataQuery = ConfigurationManager.AppSettings["TableMetadataQuery"];
			gen.TableTemplate = ConfigurationManager.AppSettings["TableTemplate"];
			gen.Generate();
		}
	}
}