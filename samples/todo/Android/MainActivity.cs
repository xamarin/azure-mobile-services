using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;

namespace Sample
{
	[Activity (Label = "Azure Mobile Services Sample", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private static readonly MobileServiceClient MobileService =
			new MobileServiceClient ("MOBILE SERVICE URL", "APPLICATION KEY");

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			this.refresh = FindViewById<Button> (Resource.Id.Refresh);
			this.refresh.Click += OnClickRefresh;

			this.newItem = FindViewById<EditText> (Resource.Id.NewItem);

			this.addItem = FindViewById<Button> (Resource.Id.AddItem);
			this.addItem.Click += OnClickAddItem;

			this.adapter = new TodoAdapter (MobileService.GetTable<TodoItem>(), this);
			this.adapter.IsUpdatingChanged += (s, e) =>
			{
				this.newItem.Enabled =
				this.addItem.Enabled =
				this.refresh.Enabled =
					!this.adapter.IsUpdating;
			};

			FindViewById<ListView> (Resource.Id.Items).Adapter = this.adapter;
		}

		private void OnClickAddItem (object sender, EventArgs eventArgs)
		{
			string text = this.newItem.Text;
			this.newItem.Text = null;

			this.adapter.Insert (new TodoItem { Text = text });
		}

		private void OnClickRefresh (object sender, EventArgs eventArgs)
		{
			this.adapter.RefreshAsync();
		}

		private TodoAdapter adapter;
		private EditText newItem;
		private Button addItem, refresh;
	}

	public class TodoAdapter
		: BaseAdapter<TodoItem>
	{
		public TodoAdapter (IMobileServiceTable<TodoItem> table, Context context)
		{
			this.inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			this.table = table;

			RefreshAsync();
		}

		public event EventHandler IsUpdatingChanged;

		public bool IsUpdating
		{
			get { return this.isUpdating; }
			private set
			{
				this.isUpdating = value;

				var changed = IsUpdatingChanged;
				if (changed != null)
					changed (this, EventArgs.Empty);
			}
		}

		public override bool HasStableIds
		{
			get { return true; }
		}

		public override int Count
		{
			get { return this.items.Count; }
		}

		public override TodoItem this [int position]
		{
			get { return this.items[position]; }
		}

		public override long GetItemId (int position)
		{
			return this.items[position].Id;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TodoItem item = this.items[position];

			View view = this.inflater.Inflate (Resource.Layout.TodoItem, null);
			
			CheckBox checkbox = view.FindViewById<CheckBox> (Resource.Id.Completed);
			checkbox.Text = item.Text;

			EventHandler<CompoundButton.CheckedChangeEventArgs> ev = null;
			ev = (o, e) =>
			{
				checkbox.CheckedChange -= ev;

				IsUpdating = true;
				
				item.Complete = true;
				table.UpdateAsync (item).ContinueWith (t =>
				{
					this.items.RemoveAt (position);
					NotifyDataSetChanged();
					IsUpdating = false;
				}, scheduler);
			};

			checkbox.CheckedChange += ev;

			return view;
		}

		public void RefreshAsync()
		{
			IsUpdating = true;
			this.table.Where (ti => !ti.Complete).ToListAsync()
				.ContinueWith (t =>
				{
					this.items = t.Result;
					NotifyDataSetChanged();
					IsUpdating = false;
				}, scheduler);
		}

		public void Insert (TodoItem item)
		{
			IsUpdating = true;
			this.items.Add (item);
			NotifyDataSetChanged();

			this.table.InsertAsync (item).ContinueWith (t =>
			{
				if (t.IsFaulted)
				{
					this.items.Remove (item);
					NotifyDataSetChanged();
				}
				
				IsUpdating = false;
			}, scheduler);
		}

		private List<TodoItem> items = new List<TodoItem>();

		private readonly LayoutInflater inflater;
		private readonly IMobileServiceTable<TodoItem> table;
		private readonly TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private bool isUpdating;
	}
}