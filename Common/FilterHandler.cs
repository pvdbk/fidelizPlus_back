using System;
using System.Text.RegularExpressions;

namespace fidelizPlus_back
{
    public static class FilterHandler
    {
        static public Func<object, bool> GetLambdaForString(object schema)
        {
            Regex regex = new Regex((string)schema);
            return value => regex.IsMatch((string)value);
        }

        static public Func<object, bool> GetLambdaForInt(object schema)
        {
            Func<object, bool> ret = value => true;
            if (schema is Range<int>)
            {
                int min = ((Range<int>)schema).Min;
                int max = ((Range<int>)schema).Max;
                ret = value => (int)value >= min && (int)value <= max;
            }
            else
            {
                ret = value => (int)value == (int)schema;
            }
            return ret;
        }

        static public Func<object, bool> GetLambdaForDecimal(object schema)
        {
            Func<object, bool> ret = value => true;
            if (schema is Range<decimal>)
            {
                decimal min = ((Range<decimal>)schema).Min;
                decimal max = ((Range<decimal>)schema).Max;
                ret = value => (int)(1000 * ((decimal)value - min)) >= 0 && (int)(1000 * (max - (decimal)value)) >= 0;
            }
            else
            {
                ret = value => (int)(1000 * ((decimal)value - (decimal)schema)) == 0;
            }
            return ret;
        }

        static public Func<object, bool> GetLambdaForDateTime(object schema)
        {
            Func<object, bool> ret = value => true;
            if (schema is Range<DateTime>)
            {
                DateTime min = ((Range<DateTime>)schema).Min;
                DateTime max = ((Range<DateTime>)schema).Max;
                ret = value => (DateTime)value >= min && (DateTime)value <= max;
            }
            else
            {
                ret = value => (DateTime)value == (DateTime)schema;
            }
            return ret;
        }

        static public Func<object, bool> GetLambda(Type type, object schema)
        {
            Func<object, bool> ret = x => true;
            return
                (type == typeof(string)) ? GetLambdaForString(schema) :
                (type == typeof(int)) ? GetLambdaForInt(schema) :
                (type == typeof(decimal)) ? GetLambdaForDecimal(schema) :
                (type == typeof(DateTime)) ? GetLambdaForDateTime(schema) :
                value => true;
        }
    }
}
