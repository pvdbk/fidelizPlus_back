using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace fidelizPlus_back
{
    public class FiltersHandler
    {
        public FiltersHandler()
        { }

        private T TypedError<T>() => throw new AppException("Bad filter");

        private void Error() => TypedError<object>();

        private Func<object, bool> GetTestForBool(Tree filter) => (filter.Type == "boolean")
            ? value => !((bool)value ^ (bool)filter.Value)
            : TypedError<Func<object, bool>>();

        private Func<object, bool> GetTestForString(Tree filter)
        {
            Func<object, bool> ret = null;
            if (filter.Type == "string")
            {
                Regex regex = new Regex((string)filter.Value);
                ret = value => regex.IsMatch((string)value);
            }
            else
            {
                Error();
            }
            return ret;
        }

        private Func<object, bool> GetTestForInt(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            if (minTree != null && maxTree != null)
            {
                if (minTree.IsInteger && maxTree.IsInteger)
                {
                    int min = minTree.IntValue;
                    int max = maxTree.IntValue;
                    ret = value => (int)value >= min && (int)value <= max;
                }
            }
            else if (filter.IsInteger)
            {
                int filterValue = filter.IntValue;
                ret = value => (int)value == filterValue;
            }
            if (ret == null)
            {
                Error();
            }
            return ret;
        }

        private Func<object, bool> GetTestForDecimal(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            if (minTree != null && maxTree != null)
            {
                if (minTree.Type == "number" && maxTree.Type == "number")
                {
                    decimal min = (decimal)minTree.Value;
                    decimal max = (decimal)maxTree.Value;
                    ret = value => (int)(1000 * ((decimal)value - min)) >= 0 && (int)(1000 * (max - (decimal)value)) >= 0;
                }
            }
            else if (filter.Type == "number")
            {
                decimal filterValue = (decimal)filter.Value;
                ret = value => (int)(1000 * ((decimal)value - filterValue)) == 0;
            }
            if (ret == null)
            {
                Error();
            }
            return ret;
        }

        private Func<object, bool> GetTestForDateTime(Tree filter)
        {
            Func<object, bool> ret = null;
            Tree minTree = filter.Get("min");
            Tree maxTree = filter.Get("max");
            try
            {
                if (minTree != null && maxTree != null)
                {
                    if (minTree.Type == "string" && maxTree.Type == "string")
                    {
                        DateTime min = DateTime.Parse((string)minTree.Value);
                        DateTime max = DateTime.Parse((string)maxTree.Value);
                        ret = value => (DateTime)value >= min && (DateTime)value <= max;
                    }
                }
                else if (filter.Type == "string")
                {
                    DateTime filterValue = DateTime.Parse((string)filter.Value);
                    ret = value => (DateTime)value == filterValue;
                }
            }
            catch { }
            if (ret == null)
            {
                Error();
            }
            return ret;
        }

        private Func<object, bool> GetTestForObject(Tree filters, Type filteredType)
        {
            if (filters.Type != "object")
            {
                Error();
            }
            IEnumerable<PropertyInfo> props = filteredType.GetProps();
            var tests = new List<Func<object, bool>>();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                Tree filter = filters.Get(name.FirstToLower());
                if (filter != null)
                {
                    Func<object, bool> test = TreeToTest(filter, prop.PropertyType);
                    tests.Add(toFilter => test(prop.GetValue(toFilter)));
                }
            }
            return x => tests.All(test => test(x));
        }

        public Func<object, bool> TreeToTest(Tree filter, Type type)
        {
            return
                (filter.Value == null) ? x => x == null :
                (type == typeof(string)) ? GetTestForString(filter) :
                (type == typeof(int) || type == typeof(int?)) ? GetTestForInt(filter) :
                (type == typeof(decimal) || type == typeof(decimal?)) ? GetTestForDecimal(filter) :
                (type == typeof(bool) || type == typeof(bool?)) ? GetTestForBool(filter) :
                (type == typeof(DateTime)) ? GetTestForDateTime(filter) :
                GetTestForObject(filter, type);
        }
    }
}
