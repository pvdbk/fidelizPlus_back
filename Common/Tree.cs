using System;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace fidelizPlus_back
{
    using Errors;

    public class Tree
    {
        private Error error;
        private XElement content;
        private int? intValue;

        private Tree(XElement content, Error error)
        {
            this.error = error;
            this.content = content;
            this.intValue = null;
        }

        public Tree(string s, Error error) : this(
            XElement.Load(
                JsonReaderWriterFactory.CreateJsonReader(
                    Encoding.UTF8.GetBytes(s),
                    new System.Xml.XmlDictionaryReaderQuotas()
                )
            ),
            error
        )
        {
        }

        public int IntValue => this.intValue == null
            ? this.error.TypedThrow<int>("Unallowed use of the Tree.IntValue method")
            : (int)this.intValue;

        public string Type => this.content.Attribute("type").Value;

        public object Value()
        {
            string type = this.Type;
            string value = this.content.Value;
            return
                type == "object" ? this :
                type == "string" ? value :
                type == "number" ? Decimal.Parse(value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US")) :
                type == "boolean" ? value == "true" :
                type == "null" ? null :
                this.error.TypedThrow<object>($"Unhandled type : {type}");
        }

        public Tree Get(string path)
        {
            XElement xelt = this.content.XPathSelectElement("//" + path);
            return xelt == null ? null : new Tree(xelt, this.error);
        }

        public bool IsInteger()
        {
            bool ret = false;
            if (this.Type == "number")
            {
                decimal d = (decimal)this.Value();
                decimal x = (int)d;
                ret = (int)(1000 * (d - x)) == 0;
                if (ret)
                {
                    this.intValue = (int)d;
                }
            }
            return ret;
        }
    }
}
