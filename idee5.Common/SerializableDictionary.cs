// ==========================================================================
// Projekt: idee5.Licensing
// Autor: idee5 Erstellt am: 17.07.2013
// Beschreibung: übernommen von http://www.jankowskimichal.pl/en/2010/10/serializabledictionary/ und
//               der wiederum von : http://weblogs.asp.net/pwelter34/archive/2006/05/03/444961.aspx

using idee5.Common.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;

namespace idee5.Common;
/// <summary>
/// A serializable <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TVal"></typeparam>
[Serializable]
public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable {
    #region Private Properties

    /// <summary>
    /// Serialize the entry's value.
    /// </summary>
    protected XmlSerializer ValueSerializer {
        get { return _valueSerializer ?? (_valueSerializer = new XmlSerializer(typeof(TVal))); }
    }

    private XmlSerializer KeySerializer {
        get { return _keySerializer ?? (_keySerializer = new XmlSerializer(typeof(TKey))); }
    }

    #endregion Private Properties

    #region Private Members

    private XmlSerializer _keySerializer;
    private XmlSerializer _valueSerializer;

    #endregion Private Members

    #region Constructors

    /// <summary>
    /// Create a new <see cref="SerializableDictionary{TKey, TVal}"/>.
    /// </summary>
    public SerializableDictionary() {
    }

    /// <summary>
    /// Create a new <see cref="SerializableDictionary{TKey, TVal}"/> from
    /// an <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public SerializableDictionary(IDictionary<TKey, TVal> dictionary) : base(dictionary) {
    }

    /// <summary>Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"></see> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"></see>.</summary>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"></see> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}"></see> for the type of the key.</param>
    public SerializableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) {
    }

    /// <summary>Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"></see> class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.</summary>
    /// <param name="capacity">The initial number of elements that the <see cref="SerializableDictionary{TKey, TValue}"></see> can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity">capacity</paramref> is less than 0.</exception>
    public SerializableDictionary(int capacity) : base(capacity) {
    }

    /// <summary>Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"></see> class that contains elements copied from the specified <see cref="IDictionary{TKey, TValue}"></see> and uses the specified <see cref="IEqualityComparer{T}"></see>.</summary>
    /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"></see> whose elements are copied to the new <see cref="Dictionary{TKey, TValue}"></see>.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"></see> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}"></see> for the type of the key.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary">dictionary</paramref> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="dictionary">dictionary</paramref> contains one or more duplicate keys.</exception>
    public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) {
    }

    /// <summary>Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"></see> class that is empty, has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"></see>.</summary>
    /// <param name="capacity">The initial number of elements that the <see cref="Dictionary{TKey, TValue}"></see> can contain.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"></see> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}"></see> for the type of the key.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity">capacity</paramref> is less than 0.</exception>
    public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) {
    }

    #endregion Constructors

    #region ISerializable Members

    protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        int itemCount = info.GetInt32(name: "itemsCount");
        for (int i = 0; i < itemCount; i++) {
            var kvp = (KeyValuePair<TKey, TVal>)info.GetValue(String.Format(CultureInfo.InvariantCulture, format: "Item{0}", args: new object[] { i }), typeof(KeyValuePair<TKey, TVal>));
            Add(kvp.Key, kvp.Value);
        }
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue(name: "itemsCount", value: Count);
        int itemIdx = 0;
        foreach (KeyValuePair<TKey, TVal> kvp in this) {
            info.AddValue(String.Format(CultureInfo.InvariantCulture, format: "Item{0}", args: new object[] { itemIdx }), kvp, typeof(KeyValuePair<TKey, TVal>));
            itemIdx++;
        }
    }

    #endregion ISerializable Members

    #region IXmlSerializable Members

    void IXmlSerializable.WriteXml(XmlWriter writer) {
        foreach (KeyValuePair<TKey, TVal> kvp in this) {
            writer.WriteStartElement(localName: "item");
            writer.WriteStartElement(localName: "key");
            KeySerializer.Serialize(writer, kvp.Key);
            writer.WriteEndElement();
            writer.WriteStartElement(localName: "value");
            ValueSerializer.Serialize(writer, kvp.Value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    void IXmlSerializable.ReadXml(XmlReader reader) {
        if (reader.IsEmptyElement)
            return;
        // Move past container
        if (reader.NodeType == XmlNodeType.Element && !reader.Read())
            throw new XmlException(String.Format(CultureInfo.InvariantCulture, Resources.ErrorInDeserializationOf, typeof(SerializableDictionary<,>).Name));
        while (reader.NodeType != XmlNodeType.EndElement) {
            reader.ReadStartElement(name: "item");
            reader.ReadStartElement(name: "key");
            var key = (TKey)KeySerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement(name: "value");
            var value = (TVal)ValueSerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadEndElement();
            Add(key, value);
            reader.MoveToContent();
        }
        // Move past container
        if (reader.NodeType == XmlNodeType.EndElement) {
            reader.ReadEndElement();
        } else {
            throw new XmlException(String.Format(
               CultureInfo.InvariantCulture,
               Resources.ErrorInDeserializationOf,
               typeof(SerializableDictionary<,>).Name));
        }
    }

    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
        return null;
    }

    #endregion IXmlSerializable Members
}