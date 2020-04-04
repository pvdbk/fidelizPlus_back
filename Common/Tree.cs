using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace fidelizPlus_back
{
    public class Tree
    {
        private XElement content;
        private int? intValue;

        private Tree(XElement content)
        {
            this.content = content;
            this.intValue = null;
        }

        public Tree(string s) : this(
            XElement.Load(
                JsonReaderWriterFactory.CreateJsonReader(
                    Encoding.UTF8.GetBytes(s),
                    new System.Xml.XmlDictionaryReaderQuotas()
                )
            )
        )
        {
        }

        public int IntValue => this.intValue == null
            ? AppException.Cast<int>("Unallowed use of the Tree.IntValue method")
            : (int)this.intValue;

        public string Type => this.content.Attribute("type").Value;

        public object Value()
        {
            string type = this.Type;
            string value = this.content.Value;
            return
                type == "object" ? this :
                type == "string" ? value :
                type == "number" ? Utils.DecimalParse(value) :
                type == "boolean" ? value == "true" :
                type == "null" ? null :
                AppException.Cast<object>($"Unhandled type : {type}");
        }

        public Tree Get(string path)
        {
            XElement xelt = this.content.XPathSelectElement("//" + path);
            return xelt == null ? null : new Tree(xelt);
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
