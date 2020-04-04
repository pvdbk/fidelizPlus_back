using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace fidelizPlus_back
{
    public static class FiltersHandler
    {
        public static IEnumerable<Func<object, bool>> TreeToTests(
            Type filteredType,
            Tree filtersTree,
            string[] toExclude = null
        )
        {
            if (filtersTree.Type != "object")
            {
                throw new AppException("Bad filter for object", 400);
            }
            IEnumerable<PropertyInfo> props = Utils.GetProps(filteredType);
            var ret = new List<Func<object, bool>>();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                Tree filter = filtersTree.Get(Utils.FirstToLower(name));
                if (filter != null && (toExclude == null || !toExclude.Contains(name)))
                {
                    Func<object, bool> propTest = GetTest(prop.PropertyType, filter);
                    ret.Add(toFilter => propTest(prop.GetValue(toFilter)));
                }
            }
            return ret;
        }

        public static IEnumerable<TItem> Apply<TItem, TFiltered>(
            IEnumerable<TItem> list,
            Tree filtersTree,
            Func<TItem, TFiltered> delegFilter,
            string[] propsToExclude = null
        )
        {
            IEnumerable<Func<object, bool>> tests = TreeToTests(typeof(TFiltered), filtersTree, propsToExclude);
            Func<TItem, bool> filter = x => tests.All(test => test(delegFilter(x)));
            return list.Where(filter);
        }

        public static IEnumerable<T> Apply<T>(
            IEnumerable<T> list,
            Tree filtersTree,
            string[] propsToExclude = null
        )
        {
            IEnumerable<Func<object, bool>> tests = TreeToTests(typeof(T), filtersTree, propsToExclude);
            Func<T, bool> filter = x => tests.All(test => test(x));
            return list.Where(filter);
        }

        static public Func<object, bool> GetTestForObject(Type type, Tree filter)
        {
            IEnumerable<Func<object, bool>> tests = TreeToTests(type, filter);
            return x => tests.All(test => test(x));
        }

        static public Func<object, bool> GetTestForBool(Tree filter)
        {
            return (filter.Type == "boolean")
                ? value => !((bool)value ^ (bool)filter.Value())
                : AppException.Cast<Func<object, bool>>("Bad filter for bool", 400);
        }

        static public Func<object, bool> GetTestForString(Tree filter)
        {
            Func<object, bool> ret = null;
            if (filter.Type == "string")
            {
                Regex regex = new Regex((string)filter.Value());
                ret = value => regex.IsMatch((string)value);
            }
            else
            {
                throw new AppException("Bad filter for string", 400);
            }
            return ret;
        }

        public static Func<object, bool> GetTestForInt(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            if (minTree != null && maxTree != null)
            {
                if (minTree.IsInteger() && maxTree.IsInteger())
                {
                    int min = minTree.IntValue;
                    int max = maxTree.IntValue;
                    ret = value => (int)value >= min && (int)value <= max;
                }
            }
            else if (filter.IsInteger())
            {
                int filterValue = filter.IntValue;
                ret = value => (int)value == filterValue;
            }
            if (ret == null)
            {
                throw new AppException("Bad filter for int", 400);
            }
            return ret;
        }

        public static Func<object, bool> GetTestForDecimal(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            if (minTree != null && maxTree != null)
            {
                if (minTree.Type == "number" && maxTree.Type == "number")
                {
                    decimal min = (decimal)minTree.Value();
                    decimal max = (decimal)maxTree.Value();
                    ret = value => (int)(1000 * ((decimal)value - min)) >= 0 && (int)(1000 * (max - (decimal)value)) >= 0;
                }
            }
            else if (filter.Type == "number")
            {
                decimal filterValue = (decimal)filter.Value();
                ret = value => (int)(1000 * ((decimal)value - filterValue)) == 0;
            }
            if (ret == null)
            {
                throw new AppException("Bad filter for decimal", 400);
            }
            return ret;
        }

        public static Func<object, bool> GetTestForDateTime(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            if (minTree != null && maxTree != null)
            {
                if (minTree.Type == "string" && maxTree.Type == "string")
                {
                    try
                    {
                        DateTime min = DateTime.Parse((string)minTree.Value());
                        DateTime max = DateTime.Parse((string)maxTree.Value());
                        ret = value => (DateTime)value >= min && (DateTime)value <= max;
                    }
                    catch { }
                }
            }
            else if (filter.Type == "string")
            {
                try
                {
                    DateTime filterValue = DateTime.Parse((string)filter.Value());
                    ret = value => (DateTime)value == filterValue;
                }
                catch { }
            }
            if (ret == null)
            {
                throw new AppException("Bad filter for DateTime", 400);
            }
            return ret;
        }

        public static Func<object, bool> GetTest(Type type, Tree filter)
        {
            return
                (type == typeof(string)) ? GetTestForString(filter) :
                (type == typeof(int)) ? GetTestForInt(filter) :
                (type == typeof(decimal)) ? GetTestForDecimal(filter) :
                (type == typeof(bool)) ? GetTestForBool(filter) :
                (type == typeof(DateTime)) ? GetTestForDateTime(filter) :
                GetTestForObject(type, filter);
        }
    }
}
