using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back
{
    public class Utils
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
            return GetProps(typeof(T));
        }

        public TTarget Cast<TTarget, TSource>(TSource source) where TTarget : new()
        {
            TTarget ret = new TTarget();
            var propsDic = new Dictionary<string, PropertyInfo>();
            IEnumerable<PropertyInfo> retProps = GetProps<TTarget>();
            foreach (PropertyInfo prop in retProps)
            {
                propsDic.Add(prop.Name, prop);
            }
            IEnumerable<PropertyInfo> props = GetProps<TSource>().Where(prop =>
                propsDic.ContainsKey(prop.Name) &&
                prop.PropertyType == propsDic[prop.Name].PropertyType
            );
            foreach (PropertyInfo prop in props)
            {
                propsDic[prop.Name].SetValue(ret, prop.GetValue(source));
            }
            return ret;
        }
    }
}
