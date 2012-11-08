using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Windows.Data.Json
{
	public class JsonObject
		: IJsonValue, IDictionary<string, IJsonValue>
	{
		public JsonObject()
		{
			this.values = new Dictionary<string, IJsonValue>();
		}

		internal JsonObject (System.Json.JsonObject obj)
		{
			this.values = obj.ToDictionary (kvp => kvp.Key, kvp => kvp.Value.AsValue());
		}

		private readonly IDictionary<string, IJsonValue> values;

		public int Count
		{
			get { return this.values.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IJsonValue this [string key]
		{
			get { return this.values[key]; }
			set { this.values[key] = value; }
		}

		public ICollection<string> Keys
		{
			get { return this.values.Keys; }
		}

		public ICollection<IJsonValue> Values
		{
			get { return this.values.Values; }
		}

		public JsonValueType ValueType
		{
			get { return JsonValueType.Object; }
		}

		public JsonValue GetNamedValue (string name)
		{
			return (JsonValue)this[name];
		}

		public void SetNamedValue (string name, IJsonValue value)
		{
			this[name] = value;
		}

		public JsonArray GetNamedArray (string name)
		{
			return (JsonArray)this[name];
		}

		public JsonArray GetArray ()
		{
			throw new NotSupportedException();
		}

		public bool GetNamedBoolean (string name)
		{
			return this[name].GetBoolean();
		}

		public bool GetBoolean ()
		{
			throw new NotSupportedException();
		}

		public double GetNamedNumber (string name)
		{
			return this[name].GetNumber();
		}

		public double GetNumber ()
		{
			throw new NotSupportedException();
		}

		public JsonObject GetNamedObject (string name)
		{
			return this[name].GetObject();
		}

		public JsonObject GetObject ()
		{
			return this;
		}

		public string GetNamedString (string name)
		{
			return this[name].GetString();
		}

		public string GetString ()
		{
			throw new NotSupportedException();
		}

		public string Stringify ()
		{
			return ToSystemObject().ToString();
		}

		public IEnumerator<KeyValuePair<string, IJsonValue>> GetEnumerator ()
		{
			return this.values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public void Add (KeyValuePair<string, IJsonValue> item)
		{
			this.values.Add (item);
		}

		public void Clear ()
		{
			this.values.Clear();
		}

		public bool Contains (KeyValuePair<string, IJsonValue> item)
		{
			return this.values.Contains (item);
		}

		public void CopyTo (KeyValuePair<string, IJsonValue>[] array, int arrayIndex)
		{
			this.values.CopyTo (array, arrayIndex);
		}

		public bool Remove (KeyValuePair<string, IJsonValue> item)
		{
			return this.values.Remove (item);
		}

		public void Add (string key, IJsonValue value)
		{
			this.values.Add (key, value);
		}

		public bool ContainsKey (string key)
		{
			return this.values.ContainsKey (key);
		}

		public bool Remove (string key)
		{
			return this.values.Remove (key);
		}

		public bool TryGetValue (string key, out IJsonValue value)
		{
			return this.values.TryGetValue (key, out value);
		}

		internal System.Json.JsonObject ToSystemObject()
		{
			return new System.Json.JsonObject (this.values.Select (
				kvp => new KeyValuePair<string, System.Json.JsonValue> (kvp.Key, kvp.Value.AsValue())));
		}
	}
}