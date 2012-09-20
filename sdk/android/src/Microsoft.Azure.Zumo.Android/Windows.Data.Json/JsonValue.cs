using System;
using System.Json;

namespace Windows.Data.Json
{
	public sealed class JsonValue
		: IJsonValue
	{
		internal JsonValue (System.Json.JsonValue value)
		{
			this.value = value;
		}

		public JsonValueType ValueType
		{
			get
			{
				if (this.value == null)
					return JsonValueType.Null;

				switch (this.value.JsonType)
				{
					case JsonType.Boolean:
						return JsonValueType.Boolean;
					case JsonType.Object:
						return JsonValueType.Object;
					case JsonType.Array:
						return JsonValueType.Array;
					case JsonType.Number:
						return JsonValueType.Number;
					case JsonType.String:
						return JsonValueType.String;
					default:
						return JsonValueType.Null;
				}
			}
		}

		public JsonArray GetArray ()
		{
			return new JsonArray ((System.Json.JsonArray)this.value);
		}

		public bool GetBoolean ()
		{
			return this.value;
		}

		public double GetNumber ()
		{
			return this.value;
		}

		public JsonObject GetObject ()
		{
			return new JsonObject ((System.Json.JsonObject)this.value);
		}

		public string GetString ()
		{
			return this.value;
		}

		public string Stringify ()
		{
			return this.value.ToString();
		}

		internal System.Json.JsonValue ToSystemValue()
		{
			return this.value;
		}

		public static JsonValue Parse (string text)
		{
			return new JsonValue (System.Json.JsonValue.Parse (text));
		}

		public static bool TryParse (string text, out JsonValue value)
		{
			value = null;

			try
			{
				value = JsonValue.Parse (text);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static JsonValue CreateStringValue (string value)
		{
			return new JsonValue (value);
		}

		public static JsonValue CreateNumberValue (double value)
		{
			return new JsonValue (value);
		}

		public static JsonValue CreateBooleanValue (bool value)
		{
			return new JsonValue (value);
		}

		private readonly System.Json.JsonValue value;
	}
}