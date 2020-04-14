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

        public string Name
        {
            get => Content.Name.ToString();
            set { Content.Name = value; }
        }

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

        public Tree() : this("{}")
        { }

        public Tree(string json, string name) : this(json)
        {
            Content.Name = name;
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

        public Tree Copy => new Tree(Json, Name);

        public bool Remove(string childName)
        {
            bool ret = false;
            Tree child = Get(childName);
            if (child != null)
            {
                child.Content.Remove();
                ret = true;
            }
            return ret;
        }

        public bool IsIn(Tree tree)
        {
            return tree.Type == "object" && tree.Childs.Any(t => t.Content == Content || IsIn(t));
        }

        public void Add(Tree t)
        {
            if (t == null)
            {
                throw new AppException("Inappropriate use of Tree.Add");
            }
            if (Get(t.Name) != null)
            {
                throw new AppException("Existing key");
            }
            if (Content == t.Content || IsIn(t))
            {
                throw new AppException("Trees don't support cycles");
            }
            Content.Add(t.Content);
        }

        public Tree Concat(Tree t)
        {
            if (Type != "object" || t.Type != "object")
            {
                throw new AppException("Inappropriate use of Tree.Concat");
            }
            Tree ret = Copy;
            IEnumerable<Tree> toAdd = t.Childs;
            foreach (Tree tree in toAdd)
            {
                ret.Add(tree.Copy);
            }
            return ret;
        }
    }
}
