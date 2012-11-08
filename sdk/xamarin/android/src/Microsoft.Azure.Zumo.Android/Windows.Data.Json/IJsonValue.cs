namespace Windows.Data.Json
{
	public interface IJsonValue
	{
		JsonValueType ValueType { get; }

		JsonArray GetArray ();
		bool GetBoolean ();
		double GetNumber ();
		JsonObject GetObject ();
		string GetString ();
		string Stringify ();
	}
}