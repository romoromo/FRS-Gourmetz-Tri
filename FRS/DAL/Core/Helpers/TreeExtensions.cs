using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Core.Helpers
{
    public static class TreeExtensions
    {
        public interface ITree<T>
        {
            T Data { get; }
            ITree<T> Parent { get; }
            ICollection<ITree<T>> Children { get; }
            bool IsRoot { get; }
            bool IsLeaf { get; }
            int Level { get; }
            int Id { get; }
            string Name { get; }
            int? ParentId { get; }
        }

        /// <summary> Converts given collection to tree. </summary>
        /// <typeparam name="T">Custom data type to associate with tree node.</typeparam>
        /// <param name="items">The collection items.</param>
        /// <param name="parentSelector">Expression to select parent.</param>
        public static ITree<T> ToTree<T>(this IList<T> items, Func<T, T, bool> parentSelector)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var lookup = items.ToLookup(item => items.FirstOrDefault(parent => parentSelector(parent, item)),
                child => child);

            return Tree<T>.FromLookup(lookup);
        }

        /// <summary> Flatten tree to plain list of nodes </summary>
        public static IEnumerable<TNode> Flatten<TNode>(this IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> childrenSelector)
        {
            if (nodes == null)
            {
                nodes = Enumerable.Empty<TNode>() ;//throw new ArgumentNullException(nameof(nodes));
            }
            return nodes.SelectMany(c => childrenSelector(c).Flatten(childrenSelector)).Concat(nodes);
        }

        /// <summary> Internal implementation of <see cref="ITree{T}" /></summary>
        /// <typeparam name="T">Custom data type to associate with tree node.</typeparam>
        internal class Tree<T> : ITree<T>
        {
            public T Data { get; }

            public ITree<T> Parent { get; private set; }

            public ICollection<ITree<T>> Children { get; }

            public bool IsRoot => Parent == null;

            public bool IsLeaf => Children.Count == 0;

            public int Level => IsRoot ? 0 : Parent.Level + 1;

            public int Id {
                get
                {
                    if (Data != null && Data.GetType().GetProperties().Any(p => p.Name == "Id"))
                    {
                        var id = Data.GetType().GetProperty("Id").GetValue(Data, null);
                        return (int)id;
                    }
                    return 0;
                }
            }

            public string Name
            {
                get
                {
                    if (Data != null && Data.GetType().GetProperties().Any(p => p.Name == "Name"))
                    {
                        var name = Data.GetType().GetProperty("Name").GetValue(Data, null);
                        return (string)name;
                    }
                    return string.Empty;
                }
            }

            public int? ParentId
            {
                get
                {
                    if (Parent != null && Parent.GetType().GetProperties().Any(p => p.Name == "Id"))
                    {
                        var id = Data.GetType().GetProperty("Id").GetValue(Data, null);
                        return (int)id;
                    }
                    return 0;
                }
            }

            private Tree(T data)
            {
                Children = new LinkedList<ITree<T>>();
                Data = data;
            }

            public static Tree<T> FromLookup(ILookup<T, T> lookup)
            {
                var rootData = lookup.Count == 1 ? lookup.First().Key : default(T);
                var root = new Tree<T>(rootData);
                root.LoadChildren(lookup);
                return root;
            }

            private void LoadChildren(ILookup<T, T> lookup)
            {
                foreach (var data in lookup[Data])
                {
                    var child = new Tree<T>(data) { Parent = this };
                    Children.Add(child);
                    child.LoadChildren(lookup);
                }
            }
        }
    }
}
