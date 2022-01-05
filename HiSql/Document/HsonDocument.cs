using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// Hone 语法文档
    /// </summary>
    public class HsonDocument :IComparable<HsonDocument> , IEnumerable<HsonElement>, IEquatable<HsonDocument>
    {

        private readonly List<HsonElement> _elements = new List<HsonElement>();
        public HsonDocument()
        { }
        //public HsonDocument(HsonElement element)
        //{
        //    Add(element);
        //}
        //public HsonDocument(IEnumerable<HsonElement> elements)
        //{
        //    AddRange(elements);

        //}
        //public HsonDocument(IDictionary dictionary)
        //{
        //    Add(dictionary);
        //}
        //public HsonDocument(IEnumerable<KeyValuePair<string, object>> dictionary)
        //{
        //    AddRange(dictionary);
        //}
        //public HsonDocument(params HsonElement[] elements)
        //{
        //    Add(elements);
        //}
        //public HsonDocument(Dictionary<string, object> dictionary)
        //{
        //    AddRange(dictionary);
        //}
        
        public virtual HsonDocument Add(HsonElement element)
        {
            var isDuplicate = _elements.Where(n => n.Name == element.Name).Count()>0 ? true : false;
            if (isDuplicate )
            {
                var message = string.Format("Duplicate element name '{0}'.", element.Name);
                throw new InvalidOperationException(message);
            }
            else
            {
                _elements.Add(element);
                
            }

            return this;
        }
        public virtual HsonDocument AddRange(IEnumerable<HsonElement> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            foreach (var element in elements)
            {
                Add(element);
            }

            return this;
        }
        public virtual HsonDocument Add(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return Add(new HsonElement(name, value));


        }
        
        public virtual HsonDocument Add(IDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key == null)
                {
                    throw new ArgumentException("A key passed to BsonDocument.AddRange is null.", "keys");
                }
                if (entry.Key.GetType() != typeof(string))
                {
                    throw new ArgumentOutOfRangeException("dictionary", "One or more keys in the dictionary passed to BsonDocument.AddRange is not a string.");
                }
                Add((string)entry.Key,  entry.Value);
            }

            return this;
        }
        public virtual HsonDocument AddRange(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (var entry in dictionary)
            {
                Add(entry.Key,entry.Value);
            }

            return this;
        }
        public virtual HsonDocument Add(params HsonElement[] elements)
        {
            return AddRange((IEnumerable<HsonElement>)elements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public virtual IEnumerator<HsonElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
        public bool Equals(HsonDocument obj)
        {
            return Equals((object)obj); // handles obj == null correctly
        }
        public virtual int CompareTo(HsonDocument rhs)
        {
            if (rhs == null) { return 1; }

            // lhs and rhs might be subclasses of BsonDocument
            using (var lhsEnumerator = Elements.GetEnumerator())
            using (var rhsEnumerator = rhs.Elements.GetEnumerator())
            {
                while (true)
                {
                    var lhsHasNext = lhsEnumerator.MoveNext();
                    var rhsHasNext = rhsEnumerator.MoveNext();
                    if (!lhsHasNext && !rhsHasNext) { return 0; }
                    if (!lhsHasNext) { return -1; }
                    if (!rhsHasNext) { return 1; }

                    var lhsElement = lhsEnumerator.Current;
                    var rhsElement = rhsEnumerator.Current;
                    var result = lhsElement.Name.CompareTo(rhsElement.Name);
                    if (result != 0) { return result; }
                    result = lhsElement.Value.ToString().CompareTo(rhsElement.Value);
                    if (result != 0) { return result; }
                }
            }
        }
        public virtual IEnumerable<HsonElement> Elements
        {
            get { return _elements; }
        }
    }
}
