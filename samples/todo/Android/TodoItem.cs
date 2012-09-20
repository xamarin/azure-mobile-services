using System.Runtime.Serialization;

namespace Sample
{
	public class TodoItem
	{
		public int Id
		{
			get;
			set;
		}

		[DataMember (Name = "text")]
		public string Text
		{
			get;
			set;
		}

		[DataMember (Name = "complete")]
		public bool Complete
		{
			get;
			set;
		}
	}
}