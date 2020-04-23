using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
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
            ? Content.XPathSelectElements("*").Select(x => new Tree(x))
            : new AppException("Not an object JSON").As<IEnumerable<Tree>>();

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
                trees = trees.Where(t => t.Type == "object");
                foreach (Tree tree in trees)
                {
                    tree.CheckKeysUnicity();
                }
            }
        }

        public Tree(string json)
        {
            try
            {
                Content = XElement.Load(
                    JsonReaderWriterFactory.CreateJsonReader(
                        Encoding.UTF8.GetBytes(json),
                        new System.Xml.XmlDictionaryReaderQuotas()
                    )
                );
                CheckKeysUnicity();
            }
            catch
            {
                throw new AppException($"Bad Json: {json}");
            }
        }

        public string StringValue => Type == "string"
            ? Content.Value
            : new AppException("Not a string").As<string>();

        public bool BoolValue => Type == "boolean"
            ? Content.Value == "true"
            : new AppException("Not a boolean").As<bool>();

        public decimal DecimalValue => Type == "number"
            ? Decimal.Parse(
                Content.Value,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                new CultureInfo("en-US")
            )
            : new AppException("Not a number").As<decimal>();

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
                    : new AppException("Not an integer").As<int>();
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
                    type == "null" ? "null" :
                    type == "object" ?
                        "{" +
                        Childs
                            .Select(child => child.Name.Quote() + ":" + child.Json)
                            .Aggregate("", (x, y) => x == "" ? y : x + "," + y)
                        + "}" :
                    new AppException($"Unhandled type : {type}").As<string>();
            }
        }
    }
}
