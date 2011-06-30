using Microsoft.SharePoint.WebPartPages;

namespace LiquidSilver.Extra.WebParts
{
	/// <summary>
	/// A custom <see cref="ToolPart"/> to manage the options of
	///	<see cref="UserControlLoader"/>.
	/// </summary>
	public class UserControlLoaderToolPart : ToolPart
	{
		#region Constructors

		public UserControlLoaderToolPart()
		{
			Title = "Settings";
		}

		#endregion Constructors

		#region Controls

		private UrlSelector UrlSelector;

		#endregion Controls

		#region Properties

		private IUserControlLoader WebPart
		{
			get
			{
				if (_webPart == null)
					_webPart = (IUserControlLoader)
						ParentToolPane.SelectedWebPart;

				return _webPart;
			}
		}
		private IUserControlLoader _webPart = null;

		#endregion Properties

		#region ToolPart Members

		public override void ApplyChanges()
		{
			EnsureChildControls();
			WebPart.IsSiteRelative = UrlSelector.IsSiteRelative;
			WebPart.UserControlUrl = UrlSelector.SelectedUrl;
		}

		protected override void CreateChildControls()
		{
			UrlSelector = new UrlSelector();
			UrlSelector.SelectedUrl = WebPart.UserControlUrl;
			UrlSelector.IsSiteRelative = WebPart.IsSiteRelative;
			Controls.Add(UrlSelector);
			base.CreateChildControls();
		}

		#endregion ToolPart Members
	}
}
