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

		private Func<object, bool> GetTestForBool(Tree filter) =>
			value => !((bool)value ^ filter.BoolValue);

		private Func<object, bool> GetTestForString(Tree filter)
		{
			Regex regex = new Regex(filter.StringValue);
			return value => regex.IsMatch((string)value);
		}

		private Func<object, bool> GetTestForInt(Tree filter)
		{
			Func<object, bool> ret;
			Tree minTree = filter.Get("min");
			Tree maxTree = filter.Get("max");
			if (minTree != null && maxTree != null)
			{
				int min = minTree.IntValue;
				int max = maxTree.IntValue;
				ret = value => (int)value >= min && (int)value <= max;
			}
			else
			{
				int filterValue = filter.IntValue;
				ret = value => (int)value == filterValue;
			}
			return ret;
		}

		private Func<object, bool> GetTestForDecimal(Tree filter)
		{
			Func<object, bool> ret;
			Tree minTree = filter.Get("min");
			Tree maxTree = filter.Get("max");
			if (minTree != null && maxTree != null)
			{
				decimal min = minTree.DecimalValue;
				decimal max = maxTree.DecimalValue;
				ret = value => (int)(1000 * ((decimal)value - min)) >= 0 && (int)(1000 * (max - (decimal)value)) >= 0;
			}
			else
			{
				decimal filterValue = filter.DecimalValue;
				ret = value => (int)(1000 * ((decimal)value - filterValue)) == 0;
			}
			return ret;
		}

		private Func<object, bool> GetTestForDateTime(Tree filter)
		{
			Func<object, bool> ret;
			Func<Tree, DateTime> dateTimeValue = tree => DateTime.Parse(tree.StringValue);
			Tree minTree = filter.Get("min");
			Tree maxTree = filter.Get("max");
			if (minTree != null && maxTree != null)
			{
				DateTime min = dateTimeValue(minTree);
				DateTime max = dateTimeValue(maxTree);
				ret = value => (DateTime)value >= min && (DateTime)value <= max;
			}
			else
			{
				DateTime filterValue = dateTimeValue(filter);
				ret = value => (DateTime)value == filterValue;
			}
			return ret;
		}

		private Func<object, bool> GetTestForObject(Tree filters, Type filteredType)
		{
			if (filters.Type != "object")
			{
				throw new Exception();
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
			try
			{
				return
					(filter.Type == "null") ? x => x == null :
					(type == typeof(string)) ? GetTestForString(filter) :
					(type == typeof(int) || type == typeof(int?)) ? GetTestForInt(filter) :
					(type == typeof(decimal) || type == typeof(decimal?)) ? GetTestForDecimal(filter) :
					(type == typeof(bool) || type == typeof(bool?)) ? GetTestForBool(filter) :
					(type == typeof(DateTime)) ? GetTestForDateTime(filter) :
					GetTestForObject(filter, type);
			}
			catch
			{
				throw new Break("Bad filter: " + filter.Json, BreakCode.ErrFilter);
			}
		}
	}
}
