using System;
using System.Globalization;
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

        public int IntValue => _IntValue == null
            ? new AppException("Unallowed use of the Tree.IntValue method").Cast<int>()
            : (int)_IntValue;

        private Tree(XElement content)
        {
            Content = content;
            _IntValue = null;
        }

        public Tree(string s)
        {
            try
            {
                Content = XElement.Load(
                    JsonReaderWriterFactory.CreateJsonReader(
                        Encoding.UTF8.GetBytes(s),
                        new System.Xml.XmlDictionaryReaderQuotas()
                    )
                );
            }
            catch
            {
                throw new AppException("Bad Json", 400);
            }
            _IntValue = null;
        }

        public object Value()
        {
            string type = Type;
            string value = Content.Value;
            return
                type == "object" ? this :
                type == "string" ? value :
                type == "number" ? Decimal.Parse(value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US")) :
                type == "boolean" ? value == "true" :
                type == "null" ? null :
                new AppException($"Unhandled type : {type}").Cast<object>();
        }

        public Tree Get(string path)
        {
            XElement xelt = Content.XPathSelectElement("//" + path);
            return xelt == null ? null : new Tree(xelt);
        }

        public bool IsInteger()
        {
            bool ret = false;
            if (Type == "number")
            {
                decimal d = (decimal)Value();
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
}
