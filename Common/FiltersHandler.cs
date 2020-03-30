using System;
using System.Text.RegularExpressions;

namespace fidelizPlus_back
{
    public static class FiltersHandler
    {
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
                (type == typeof(DateTime)) ? GetTestForDateTime(filter) :
                AppException.Cast<Func<object, bool>>($"Unhandled type : {type.Name}");
        }
    }
}
