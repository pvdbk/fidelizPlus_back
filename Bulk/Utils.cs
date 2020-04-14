using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace fidelizPlus_back
{
    public class Utils
    {
        private IEnumerable<Type> AtomicTypes { get; }
        public FiltersHandler FiltersHandler { get; }

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

        public string Quote(string s)
        {
            return s == null ? null : "\"" + s + "\"";
        }

        public string FirstToLower(string s)
        {
            return s == "" || s == null
                ? s
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
            return GetProps<T>().Where(prop => AtomicTypes.Contains(prop.PropertyType));
        }

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

        public Tree ToTree(object o)
        {
            return
                o is null ? null :
                o is Tree ? (Tree)o :
                o is string ? new Tree((string)o) :
                new Tree(JsonSerializer.Serialize(o));
        }

        public Tree ExtractTree<T>(object source, string name = null)
        {
            Tree ret = null;
            if (source != null)
            {
                IEnumerable<Tree> childs = ToTree(source).Childs;
                var childsDic = new Dictionary<string, Tree>();
                foreach (Tree child in childs)
                {
                    childsDic[child.Name] = child;
                }
                IEnumerable<Tree> toTake = GetAtomicProps<T>()
                    .Select(prop => FirstToLower(prop.Name))
                    .Where(key => childsDic.ContainsKey(key))
                    .Select(key => childsDic[key]);
                ret = new Tree();
                foreach (Tree tree in toTake)
                {
                    ret.Add(tree.Copy);
                }
                if (name != null)
                {
                    ret.Name = name;
                }
            }
            return ret;
        }

        public Tree ExtractTree<T1, T2>(object source, string name = null)
        {
            return ExtractTree<T1>(ExtractTree<T2>(source), name);
        }

        public Func<T, bool> TreeToTest<T>(Tree tree) where T : class
        {
            return tree == null ? x => true : FiltersHandler.TreeToTest(tree, typeof(T));
        }
    }
}
