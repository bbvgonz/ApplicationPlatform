using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Optical.Platform.Structs
{
    /// <summary>
    /// 木構造のデータ構造を提供する。
    /// </summary>
    public class TreeElement<T> : ICloneable
    {
        #region Constructors
        static TreeElement()
        {
            PathSeparator = "/";
        }

        /// <summary>
        /// 指定された要素名で、新しい<see cref="TreeElement{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="name">要素名</param>
        /// <param name="parent">親ノード</param>
        /// <exception cref="ArgumentNullException">要素名がNullまたは空文字の場合に発生します。</exception>
        /// <exception cref="ArgumentException">要素名に区切り文字("/")が使用されている場合に発生します。</exception>
        public TreeElement(object name, TreeElement<T> parent = null)
        {
            if (string.IsNullOrEmpty(name?.ToString()))
            {
                throw new ArgumentNullException("要素名がNullまたは空文字です。", nameof(name));
            }

            if (name.ToString().Contains(PathSeparator))
            {
                throw new ArgumentException("要素名に区切り文字は使用できません。", nameof(name));
            }

            Name = name;
            Parent = parent;
            Child = new ConcurrentDictionary<object, TreeElement<T>>();
            Value = new List<T>();
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 親要素を取得します。
        /// </summary>
        public TreeElement<T> Parent { get; }

        /// <summary>
        /// この要素がRootかどうかを表します。
        /// </summary>
        public bool IsRoot => (Parent == null);

        /// <summary>
        /// 子要素を所有しているかを表します。
        /// </summary>
        public bool HasChild => (Child.Count != 0);

        /// <summary>
        /// 要素名を取得・設定します。
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// 格納オブジェクトを取得・設定する
        /// </summary>
        public List<T> Value { get; }

        /// <summary>
        /// 要素パスの区切り文字を取得・設定する
        /// </summary>
        public static string PathSeparator { get; set; }

        /// <summary>
        /// 子要素を取得する
        /// </summary>
        public ConcurrentDictionary<object, TreeElement<T>> Child { get; }

        /// <summary>
        /// Rootからの深度を取得する
        /// </summary>
        public int Depth => countDepth(this);

        /// <summary>
        /// この要素までのパスを取得する
        /// </summary>
        public string GetPath => getPath();

        /// <summary>
        /// この要素より下の階層いる子要素の数を取得する
        /// </summary>
        public int CountAllChild => childNodeCount(this);
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 指定した要素名を持つ要素を取得します。
        /// </summary>
        /// <param name="name">要素名</param>
        /// <returns>指定した要素名を持つ要素。要素名が存在しない場合<see langword="null"/>が返ります。</returns>
        public TreeElement<T> this[string name] => Child.ContainsKey(name) ? Child[name] : null;
        #endregion // Indexers

        #region Methods
        /// <summary>
        /// Rootから指定要素までの深度を取得する
        /// </summary>
        private int countDepth(TreeElement<T> element)
        {
            int depth = 0;
            TreeElement<T> node = element;
            while (!node.IsRoot)
            {
                node = node.Parent;
                depth++;
            }

            return depth;
        }

        /// <summary>
        /// 指定した要素より下位の階層にあるすべての子要素の数を取得する
        /// </summary>
        /// <param name="node">カウントを開始する要素</param>
        private int childNodeCount(TreeElement<T> node)
        {
            int count = node.Child.Count;
            if (node.HasChild)
            {
                foreach (TreeElement<T> child in node.Child.Values)
                {
                    count += childNodeCount(child);
                }
            }

            return count;
        }

        /// <summary>
        /// 現要素までのパスを取得します。
        /// </summary>
        private string getPath()
        {
            string path = string.Empty;
            if (IsRoot)
            {
                path = PathSeparator;
            }
            else
            {
                path = Parent.getPath() + Name + PathSeparator;
            }

            return path;
        }

        /// <summary>
        /// 指定した要素パスに要素があるか検索し，その要素を返す
        /// </summary>
        /// <param name="path">検索する要素のパス</param>
        /// <returns></returns>
        private TreeElement<T> searchChild(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("要素パスがNullまたは空文字です。", nameof(path));
            }

            string[] childKey = path.Split(PathSeparator.ToCharArray());

            TreeElement<T> node = this;
            for (int index = 0; index < childKey.Length; index++)
            {
                if (string.IsNullOrEmpty(childKey[index]))
                {
                    continue;
                }

                if (!node.Child.ContainsKey(childKey[index]))
                {
                    return null;
                }

                node = node.Child[childKey[index]];
            }

            return node;
        }

        /// <summary>
        /// 指定する子要素を追加する
        /// </summary>
        /// <param name="data">この要素に追加するオブジェクト</param>
        public void Add(TreeElement<T> data)
        {
            if (data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(data.Name?.ToString()))
            {
                return;
            }

            Child[data.Name] = data;
        }

        /// <summary>
        /// 指定する子要素を追加する
        /// </summary>
        /// <param name="name">この要素直下に追加する要素名</param>
        /// <param name="value">格納オブジェクト（デフォルト null）</param>
        public void Add(object name, object value = null)
        {
            if (!Child.ContainsKey(name))
            {
                Child[name] = new TreeElement<T>(name, this);
            }

            if (value == null)
            {
                return;
            }

            Child[name].Value.Add((T)value);
        }

        /// <summary>
        /// 指定したパスの格納オブジェクトを設定する
        /// </summary>
        /// <param name="path">この要素からの要素パス</param>
        /// <param name="value">格納オブジェクト</param>
        public void AddValue(string path, object value)
        {
            TreeElement<T> node = searchChild(path);
            node.Value.Add((T)value);
        }

        /// <summary>
        /// すべての子要素を削除する
        /// </summary>
        public void Clear()
        {
            if (!HasChild)
            {
                return;
            }

            foreach (TreeElement<T> child in Child.Values)
            {
                child.Clear();
            }

            Child.Clear();
        }

        /// <summary>
        /// 要素のクローンを生成します。
        /// </summary>
        /// <returns>オブジェクトのコピー</returns>
        public object Clone()
        {
            var newNode = new TreeElement<T>(Name, Parent);
            newNode.Value.AddRange(Value);

            if (HasChild)
            {
                foreach (KeyValuePair<object, TreeElement<T>> child in Child)
                {
                    var childClone = (TreeElement<T>)Clone();
                    newNode.Add(childClone);
                }
            }

            return newNode;
        }

        /// <summary>
        /// 指定したパスの格納オブジェクトを取得する
        /// </summary>
        /// <param name="path">この要素からの要素パス</param>
        /// <returns>格納オブジェクト</returns>
        public object GetValue(string path)
        {
            TreeElement<T> node = searchChild(path);
            return node.Value;
        }

        /// <summary>
        /// 指定するパスに該当する要素を削除する
        /// </summary>
        /// <param name="path">この要素を起点とする要素パス</param>
        public void RemoveChild(string path)
        {
            TreeElement<T> removeNode = searchChild(path);
            if (removeNode == null)
            {
                return;
            }

            removeNode.Clear();
            if (removeNode.IsRoot)
            {
                return;
            }

            TreeElement<T> nodeParent = removeNode.Parent;
            nodeParent.Child.TryRemove(removeNode.Name, out TreeElement<T> value);
        }

        /// <summary>
        /// 指定した要素パスに要素があるか検索し，その要素を返す
        /// </summary>
        /// <param name="path">検索する要素のパス</param>
        /// <returns></returns>
        public TreeElement<T> SearchChild(string path)
        {
            return searchChild(path);
        }
        #endregion // Methods
    }
}