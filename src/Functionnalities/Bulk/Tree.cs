using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace fidelizPlus_back
{
	public class Tree
	{
		private XElement Content { get; }
		public string Type => Content.Attribute("type").Value;

		public string Name => Content.Name.ToString();

		private Tree(XElement content)
		{
			Content = content;
		}

		public IEnumerable<Tree> Childs => Type == "object"
			? Content
				.XPathSelectElements("*")
				.Select(x => new Tree(x))
				.ToList()
			: new Break("Not an object JSON: " + this.Json, BreakCode.ErrTree).Throw<IEnumerable<Tree>>();

		private void CheckKeysUnicity()
		{
			if (Type == "object")
			{
				IEnumerable<Tree> trees = Childs;
				IEnumerable<string> keys = trees.Select(t => t.Name);
				if (keys.Distinct().Count() != keys.Count())
				{
					throw new Exception();
				}
				trees
					.Where(t => t.Type == "object")
					.ForEach(tree => tree.CheckKeysUnicity());
			}
		}

		public Tree(string json)
		{
			try
			{
				Content = XElement.Load(
					JsonReaderWriterFactory.CreateJsonReader(
						json.ToBytes(),
						new XmlDictionaryReaderQuotas()
					)
				);
				CheckKeysUnicity();
			}
			catch
			{
				throw new Break("Bad Json: " + json, BreakCode.ErrTree);
			}
		}

		public string StringValue => Type == "string"
			? Content.Value
			: new Break("Not a string: " + this.Json, BreakCode.ErrTree).Throw<string>();

		public bool BoolValue => Type == "boolean"
			? Content.Value == "true"
			: new Break("Not a boolean: " + this.Json, BreakCode.ErrTree).Throw<bool>();

		public decimal DecimalValue => Type == "number"
			? Decimal.Parse(
				Content.Value,
				NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
				new CultureInfo("en-US")
			)
			: new Break("Not a number: " + this.Json, BreakCode.ErrTree).Throw<decimal>();

		public int IntValue
		{
			get
			{
				int? ret = null;
				if (Type == "number")
				{
					decimal d = DecimalValue;
					decimal x = (int)d;
					if ((int)(1000 * (d - x)) == 0)
					{
						ret = (int)d;
					}
				}
				return ret != null
					? (int)ret
					: new Break("Not an integer: " + this.Json, BreakCode.ErrTree).Throw<int>();
			}
		}

		public Tree Get(string path)
		{
			XElement xelt = Content.XPathSelectElement(path);
			return xelt == null ? null : new Tree(xelt);
		}

		public string Json
		{
			get
			{
				string type = Type;
				return
					type == "string" ? StringValue.Quote() :
					type == "number" ? DecimalValue.ToString(new CultureInfo("en-US")) :
					type == "boolean" ? (BoolValue ? "true" : "false") :
					type == "object" ?
						"{" +
						Childs
							.Select(child => child.Name.Quote() + ":" + child.Json)
							.Aggregate("", (x, y) => x == "" ? y : x + "," + y)
						+ "}" :
					"null";
			}
		}
	}
}
