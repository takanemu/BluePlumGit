
namespace BluePlumGit
{
    using System.Collections;
    using System.Collections.Generic;

    public class LinqList<T> : IEnumerable<T>, IEnumerable
    {
        IEnumerable items;

        internal LinqList(IEnumerable items)
        {
            this.items = items;
        }

        #region IEnumerable<DataRow> Members
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T item in items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<T> ie = this;
            return ie.GetEnumerator();
        }
        #endregion
    }
}
