using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Windows.Data.Json
{
	public class JsonArray
		: IJsonValue, IList<IJsonValue>
	{
		public JsonArray()
		{
			this.values = new List<IJsonValue>();
		}

		internal JsonArray (System.Json.JsonArray array)
		{
			this.values = new List<IJsonValue> (array.Select (v => v.AsValue()));
		}

		private readonly List<IJsonValue> values;

		public IJsonValue this [int index]
		{
			get { return this.values[index]; }
			set { this.values[index] = value; }
		}

		public JsonValueType ValueType
		{
			get { return JsonValueType.Array; }
		}

		public int Count
		{
			get { return this.values.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public JsonArray GetArray ()
		{
			return this;
		}

		public bool GetBoolean ()
		{
			throw new NotSupportedException();
		}

		public double GetNumber ()
		{
			throw new NotSupportedException();
		}

		public JsonObject GetObject ()
		{
			throw new NotSupportedException();
		}

		public string GetString ()
		{
			throw new NotSupportedException();
		}

		public string Stringify ()
		{
			return ToSystemArray().ToString();
		}

		public IEnumerator<IJsonValue> GetEnumerator ()
		{
			return this.values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public void Add (IJsonValue item)
		{
			this.values.Add (item);
		}

		public void Clear ()
		{
			this.values.Clear();
		}

		public bool Contains (IJsonValue item)
		{
			return this.values.Contains (item);
		}

		public void CopyTo (IJsonValue[] array, int arrayIndex)
		{
			this.values.CopyTo (array, arrayIndex);
		}

		public bool Remove (IJsonValue item)
		{
			return this.values.Remove (item);
		}

		public int IndexOf (IJsonValue item)
		{
			return this.values.IndexOf (item);
		}

		public void Insert (int index, IJsonValue item)
		{
			this.values.Insert (index, item);
		}

		public void RemoveAt (int index)
		{
			this.values.RemoveAt (index);
		}

		internal System.Json.JsonArray ToSystemArray()
		{
			return new System.Json.JsonArray (this.values.Select (v => v.AsValue()));
		}
	}
}