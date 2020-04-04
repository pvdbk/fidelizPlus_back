using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back
{
    public static class Utils
    {
        public static string Quote(string s)
        {
            return "\"" + s + "\"";
        }

        public static string FirstToLower(string s)
        {
            return s == ""
                ? ""
                : s.Substring(0, 1).ToLower() + s.Substring(1);
        }

        public static string Join(IEnumerable<string> toJoin, string separator)
        {
            return toJoin.Aggregate("", (x, y) => x == "" ? y : x + separator + y);
        }

        public static int SetBit(int value, int bit, bool valBit)
        {
            int bitMask = 1 << bit;
            return valBit ? value | bitMask : value & ~bitMask;
        }

        public static bool GetBit(int value, int bit)
        {
            return (value & (1 << bit)) != 0;
        }

        public static decimal DecimalParse(string s)
        {
            return Decimal.Parse(
                s,
                NumberStyles.AllowDecimalPoint,
                new CultureInfo("en-US")
            );
        }

        public static IEnumerable<PropertyInfo> GetProps(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public static IEnumerable<PropertyInfo> GetProps<T>()
        {
            return GetProps(typeof(T));
        }

        public static TTarget Cast<TTarget, TSource>(TSource source, int? id = null) where TTarget : new()
        {
            TTarget ret = new TTarget();
            var propsDic = new Dictionary<string, PropertyInfo>();
            IEnumerable<PropertyInfo> retProps = Utils.GetProps<TTarget>();
            foreach (PropertyInfo prop in retProps)
            {
                propsDic.Add(prop.Name, prop);
            }
            IEnumerable<PropertyInfo> props = Utils.GetProps<TSource>().Where(prop => prop.Name != "Id" && propsDic.ContainsKey(prop.Name));
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

        public static IEnumerable<Func<TFiltered, bool>> TreeToTests<TFiltered>(
            Tree filtersTree,
            string[] toExclude = null
        )
        {
            IEnumerable<PropertyInfo> props = GetProps<TFiltered>();
            var ret = new List<Func<TFiltered, bool>>();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                Tree filter = filtersTree.Get(FirstToLower(name));
                if (filter != null && (toExclude == null || !toExclude.Contains(name)))
                {
                    Func<object, bool> propTest = FiltersHandler.GetTest(prop.PropertyType, filter);
                    ret.Add(toFilter => propTest(prop.GetValue(toFilter)));
                }
            }
            return ret;
        }

        public static string ListToMessage(string header, IEnumerable<string> list)
        {
            return (list.Count() == 0) ? "" : $"{header} :\n" + Join(list.Select(item => "   - " + item), "\n");
        }
    }
}
