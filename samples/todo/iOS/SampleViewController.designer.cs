// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Sample
{
	[Register ("SampleViewController")]
	partial class SampleViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView ItemsTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem AddButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem RefreshButton { get; set; }

		[Action ("Add:")]
		partial void Add (MonoTouch.Foundation.NSObject sender);

		[Action ("Refresh:")]
		partial void Refresh (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ItemsTable != null) {
				ItemsTable.Dispose ();
				ItemsTable = null;
			}

			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}

			if (RefreshButton != null) {
				RefreshButton.Dispose ();
				RefreshButton = null;
			}
		}
	}
}
