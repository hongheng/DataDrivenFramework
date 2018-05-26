using System;
using System.Collections.Generic;

namespace HH {
    /// <summary>
    /// 可分组字典。按组名和键值添加，按键值索引，按组名删除。
    /// </summary>
    public class GroupedDictionary<TGroup, TKey, TData> {

        protected struct HandlerData {
            public TGroup group;
            public TKey key;
            public TData data;
        }
        private class Datas : List<HandlerData> { };
        private class Cache : Dictionary<TKey, Datas> { };

        private Cache cache = new Cache();

        /// <summary>
        /// 添加带有组名和键值的数据
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public virtual void Add(TGroup group, TKey key, TData data) {
            Add(new HandlerData {
                group = group,
                key = key,
                data = data,
            });
        }

        protected void Add(HandlerData newData) {
            Datas datas;
            if (cache.TryGetValue(newData.key, out datas)) {
                datas.Add(newData);
            } else {
                datas = new Datas();
                datas.Add(newData);
                cache.Add(newData.key, datas);
            }
        }

        /// <summary>
        /// 用指定委托遍历指定键值的所有数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="callback">委托入参依次为组名和数据</param>
        public virtual void ForEach(TKey key, Action<TGroup, TData> callback) {
            Datas datas;
            if (cache.TryGetValue(key, out datas)) {
                foreach (var data in datas) {
                    callback(data.group, data.data);
                }
            }
        }

        /// <summary>
        /// 根据组名删除数据
        /// </summary>
        /// <param name="group">Group.</param>
        public virtual void Remove(TGroup group) {
            foreach (var datas in cache.Values) {
                datas.RemoveAll(d => group.Equals(d.group));
            }
        }
    }
}

