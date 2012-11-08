using System.Json;
using System.Security.AccessControl;
using System;
using Microsoft.WindowsAzure.MobileServices;

namespace Windows.Data.Json
{
	internal static class WrappedJsonExtensions
	{
		public static IJsonValue AsValue (this System.Json.JsonValue self)
		{
			if (self == null)
				return JsonExtensions.Null();

			switch (self.JsonType)
			{
				case JsonType.Array:
					return new JsonArray ((System.Json.JsonArray)self);
				case JsonType.Object:
					return new JsonObject ((System.Json.JsonObject)self);
				default:
					return new JsonValue (self);
			}
		}

		public static System.Json.JsonValue AsValue (this IJsonValue self)
		{
			if (self == null)
				return JsonExtensions.Null().AsValue();

			var value = self as JsonValue;
			if (value != null)
				return value.ToSystemValue();

			var array = self as JsonArray;
			if (array != null)
				return array.ToSystemArray();

			var obj = self as JsonObject;
			if (obj != null)
				return obj.ToSystemObject();

			throw new ArgumentException();
		}
	}
}