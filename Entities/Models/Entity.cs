using Entities.LinkModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
    {
        private readonly Dictionary<string, object> _dictionary = new();

        public object this[string key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<string> Keys => _dictionary.Keys;
        public ICollection<object> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(string key, object value) => _dictionary.Add(key, value);
        public void Add(KeyValuePair<string, object> item) => _dictionary.Add(item.Key, item.Value);
        public void Clear() => _dictionary.Clear();
        public bool Contains(KeyValuePair<string, object> item) => _dictionary.ContainsKey(item.Key) && _dictionary[item.Key].Equals(item.Value);
        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var item in _dictionary)
            {
                array[arrayIndex++] = item;
            }
        }
        public bool Remove(string key) => _dictionary.Remove(key);
        public bool Remove(KeyValuePair<string, object> item) => _dictionary.ContainsKey(item.Key) && _dictionary.Remove(item.Key);
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        // XML Serialization
        public XmlSchema? GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) return;

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement();
                string key = reader.Name;
                string value = reader.ReadString();
                _dictionary[key] = value;
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var kvp in _dictionary)
            {
                writer.WriteStartElement(kvp.Key);
                writer.WriteString(kvp.Value.ToString());
                writer.WriteEndElement();
            }
        }
        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);
            if (value.GetType() == typeof(List<Link>))
            {
                foreach (var val in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteLinksToXml(nameof(val.Href), val.Href, writer);
                    WriteLinksToXml(nameof(val.Method), val.Method, writer);
                    WriteLinksToXml(nameof(val.Rel), val.Rel, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }
            writer.WriteEndElement();
        }
    }
}