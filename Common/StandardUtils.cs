using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back
{
    public class StandardUtils : Utils
    {
        public string Quote(string s)
        {
            return "\"" + s + "\"";
        }

        public string FirstToLower(string s)
        {
            return s == ""
                ? ""
                : s.Substring(0, 1).ToLower() + s.Substring(1);
        }

        public string Join(IEnumerable<string> toJoin, string separator)
        {
            return toJoin.Aggregate("", (x, y) => x == "" ? y : x + separator + y);
        }

        public string ListToMessage(string header, IEnumerable<string> list)
        {
            return (list.Count() == 0) ? "" : $"{header} :\n" + this.Join(list.Select(item => "   - " + item), "\n");
        }

        public int SetBit(int value, int bit, bool valBit)
        {
            int bitMask = 1 << bit;
            return valBit ? value | bitMask : value & ~bitMask;
        }

        public bool GetBit(int value, int bit)
        {
            return (value & (1 << bit)) != 0;
        }

        public IEnumerable<PropertyInfo> GetProps(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public IEnumerable<PropertyInfo> GetProps<T>()
        {
            return this.GetProps(typeof(T));
        }

        public TTarget Cast<TTarget, TSource>(TSource source, int? id = null) where TTarget : new()
        {
            TTarget ret = new TTarget();
            var propsDic = new Dictionary<string, PropertyInfo>();
            IEnumerable<PropertyInfo> retProps = this.GetProps<TTarget>();
            foreach (PropertyInfo prop in retProps)
            {
                propsDic.Add(prop.Name, prop);
            }
            IEnumerable<PropertyInfo> props = this.GetProps<TSource>().Where(prop => prop.Name != "Id" && propsDic.ContainsKey(prop.Name));
            foreach (PropertyInfo prop in props)
            {
                propsDic[prop.Name].SetValue(ret, prop.GetValue(source));
            }
            if (id != null)
            {
                propsDic["Id"].SetValue(ret, id);
            }
            return ret;
        }
    }
}
