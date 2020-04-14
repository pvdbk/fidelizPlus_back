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
        private int? _IntValue { get; set; }

        public string Name => Content.Name.ToString();

        public int IntValue => _IntValue == null
            ? new AppException("Unallowed use of the Tree.IntValue method").Cast<int>()
            : (int)_IntValue;

        private Tree(XElement content)
        {
            Content = content;
            _IntValue = null;
        }

        public IEnumerable<Tree> Childs => Type == "object"
            ? Content.XPathSelectElements("*").Select(x => new Tree(x))
            : new AppException("Not an object JSON").Cast<IEnumerable<Tree>>();

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
                _IntValue = null;
                CheckKeysUnicity();
            }
            catch
            {
                throw new AppException("Bad Json");
            }
        }

        public object Value
        {
            get
            {
                string type = Type;
                string value = Content.Value;
                return
                    type == "object" ? this :
                    type == "string" ? value :
                    type == "boolean" ? value == "true" :
                    type == "null" ? null :
                    type == "number" ? Decimal.Parse(
                        value,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                        new CultureInfo("en-US")
                    ) :
                    new AppException($"Unhandled type : {type}").Cast<object>();
            }
        }

        public Tree Get(string path)
        {
            XElement xelt = Content.XPathSelectElement(path);
            return xelt == null ? null : new Tree(xelt);
        }

        public bool IsInteger
        {
            get
            {
                bool ret = false;
                if (Type == "number")
                {
                    decimal d = (decimal)Value;
                    decimal x = (int)d;
                    ret = (int)(1000 * (d - x)) == 0;
                    if (ret)
                    {
                        _IntValue = (int)d;
                    }
                }
                return ret;
            }
        }

        public string Json
        {
            get
            {
                string type = Type;
                return
                    type == "string" ? "\"" + (string)Value + "\"" :
                    type == "number" ? ((decimal)Value).ToString(new CultureInfo("en-US")) :
                    type == "boolean" ? ((bool)Value ? "true" : "false") :
                    type == "null" ? "null" :
                    type == "object" ?
                        "{" +
                        Childs
                            .Select(child => "\"" + child.Name + "\":" + child.Json)
                            .Aggregate("", (x, y) => x == "" ? y : x + "," + y)
                        + "}" :
                    new AppException($"Unhandled type : {type}").Cast<string>();
            }
        }
    }
}
