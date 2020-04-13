using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back
{
    public class Utils
    {
        private readonly IEnumerable<Type> atomicTypes = new Type[] {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(string),
            typeof(DateTime),
            typeof(DateTime?)
        };

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

        public int SetBit(int value, int bitPosition, bool valBit)
        {
            int bitMask = 1 << bitPosition;
            return valBit ? value | bitMask : value & ~bitMask;
        }

        public bool GetBit(int value, int bitPosition)
        {
            return (value & (1 << bitPosition)) != 0;
        }

        public IEnumerable<PropertyInfo> GetProps(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public IEnumerable<PropertyInfo> GetProps<T>()
        {
            return GetProps(typeof(T));
        }

        public IEnumerable<PropertyInfo> GetAtomicProps<T>()
        {
            return GetProps<T>().Where(prop => atomicTypes.Contains(prop.PropertyType));
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
