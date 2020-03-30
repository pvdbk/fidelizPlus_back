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

        public static decimal DecimalParse(string s)
        {
            return Decimal.Parse(
                s,
                NumberStyles.AllowDecimalPoint,
                new CultureInfo("en-US")
            );
        }

        public static IEnumerable<PropertyInfo> GetProps<T>() where T : new()
        {
            Type type = typeof(T);
            T obj = new T();
            IEnumerable<PropertyInfo> ret = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (obj is Models.Entity)
            {
                IEnumerable<string> fields = (IEnumerable<string>)ret.Where(prop => prop.Name == "Fields").First().GetValue(obj);
                ret = ret.Where(prop => fields.Contains(prop.Name));
            }
            return ret;
        }

        public static IEnumerable<(PropertyInfo, Func<object, bool>)> GetPropsTests<T>(
            Tree filtersTree,
            string[] toExclude = null
        ) where T : new()
        {
            IEnumerable<PropertyInfo> props = GetProps<T>();
            var ret = new List<(PropertyInfo, Func<object, bool>)>();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                Tree filter = filtersTree.Get(FirstToLower(name));
                if (filter != null && (toExclude == null || !toExclude.Contains(name)))
                {
                    Func<object, bool> test = FiltersHandler.GetTest(prop.PropertyType, filter);
                    ret.Add((prop, test));
                }
            }
            return ret;
        }

        public static IEnumerable<T> ApplyFilter<T, U>(
            IEnumerable<T> list,
            Tree filtersTree,
            Func<T, object> delegFilter = null,
            string[] propsToExclude = null
        ) where U : new()
        {
            IEnumerable<(PropertyInfo, Func<object, bool>)> propsTests = GetPropsTests<U>(filtersTree, propsToExclude);
            Func<T, bool> filter = (delegFilter == null)
                ? (Func<T, bool>)(entity => propsTests.All(propTest =>
                {
                    var (prop, test) = propTest;
                    return test(prop.GetValue(entity));
                }))
                : (Func<T, bool>)(entity => propsTests.All(propTest =>
                {
                    var (prop, test) = propTest;
                    return test(prop.GetValue(delegFilter(entity)));
                }));
            return list.Where(filter);
        }
    }
}
