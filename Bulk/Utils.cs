using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back
{
    public class Utils
    {
        private IEnumerable<Type> AtomicTypes { get; }
        private FiltersHandler FiltersHandler { get; }

        public Utils()
        {
            FiltersHandler = new FiltersHandler(this);
            AtomicTypes = new Type[] {
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
        }

        public string Quote(string s) => s == null
            ? null
            : "\"" + s + "\"";

        public string FirstToLower(string s) => s == "" || s == null
            ? s
            : s.Substring(0, 1).ToLower() + s.Substring(1);

        public int SetBit(int value, int bitPosition, bool valBit)
        {
            int bitMask = 1 << bitPosition;
            return valBit ? value | bitMask : value & ~bitMask;
        }

        public bool GetBit(int value, int bitPosition) =>
            (value & (1 << bitPosition)) != 0;

        public IEnumerable<PropertyInfo> GetProps(Type type) =>
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public IEnumerable<PropertyInfo> GetProps<T>() => GetProps(typeof(T));

        public IEnumerable<PropertyInfo> GetAtomicProps<T>() => GetProps<T>()
            .Where(prop => AtomicTypes.Contains(prop.PropertyType));

        public TTarget Cast<TTarget, TSource>(TSource source) where TTarget : new()
        {
            TTarget ret = new TTarget();
            var commonProps = new List<(PropertyInfo, PropertyInfo)>();
            IEnumerable<PropertyInfo> targetProps = GetProps<TTarget>();
            IEnumerable<PropertyInfo> sourceProps = GetProps<TSource>();
            foreach (PropertyInfo targetProp in targetProps)
            {
                PropertyInfo sourceProp = sourceProps
                    .Where(prop => prop.Name == targetProp.Name && prop.PropertyType == targetProp.PropertyType)
                    .FirstOrDefault();
                if (sourceProp != null)
                {
                    commonProps.Add((targetProp, sourceProp));
                }
            }
            foreach ((PropertyInfo targetProp, PropertyInfo sourceProp) in commonProps)
            {
                targetProp.SetValue(ret, sourceProp.GetValue(source));
            }
            return ret;
        }

        public Func<T, bool> HandleFilter<T>(string filter) where T : class
        {
            Func<T, bool> ret = x => true;
            if (filter != null)
            {
                try
                {
                    Tree tree = new Tree(filter);
                    if (tree.Type != "object")
                    {
                        throw new AppException("Bad filter parameter, object type expected");
                    }
                    ret = FiltersHandler.TreeToTest(tree, typeof(T));
                }
                catch (AppException e)
                {
                    throw new AppException(e.Content, 400);
                }
            }
            return ret;
        }
    }
}
