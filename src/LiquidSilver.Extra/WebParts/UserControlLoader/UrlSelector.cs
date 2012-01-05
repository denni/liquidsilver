using System.Web.UI;
using System.Web.UI.WebControls;

namespace LiquidSilver.Extra.WebParts
{
	/// <summary>
	/// A custom <see cref="WebControl"/> to manage the options of
	/// <see cref="UserControlLoader"/>.
	/// </summary>
	public class UrlSelector : WebControl
	{
		#region Properties

		public bool IsSiteRelative { get; set; }

		public string SelectedUrl { get; set; }

		#endregion Properties

		#region Private Methods

		private static Control CreateSection(Control head, Control body)
		{
			Panel headPanel = new Panel();
			headPanel.CssClass = "UserSectionHead";
			headPanel.Controls.Add(head);

			Panel groupPanel = new Panel();
			groupPanel.CssClass = "UserControlGroup";
			groupPanel.Controls.Add(body);

			Panel bodyPanel = new Panel();
			bodyPanel.CssClass = "UserSectionBody";
			bodyPanel.Controls.Add(groupPanel);

			PlaceHolder ph = new PlaceHolder();
			ph.Controls.Add(headPanel);
			ph.Controls.Add(bodyPanel);

			return ph;
		}

		#endregion Private Methods

		#region Control Members

		protected override void TrackViewState()
		{
			EnsureChildControls();
			base.TrackViewState();
		}

		protected override void CreateChildControls()
		{
			TextBox urlTextBox = new TextBox()
			{
				CssClass = "UserInput",
				Text = SelectedUrl,
			};

			urlTextBox.Load += (sender, e) =>
			{
				SelectedUrl = ((TextBox)sender).Text.Trim();
			};

			Label l = new Label()
			{
				Text = "Enter the URL",
				AssociatedControlID = urlTextBox.ID
			};

			Controls.Add(CreateSection(l, urlTextBox));

			RadioButtonList urlRelativenessList = new RadioButtonList()
			{
				RepeatLayout = RepeatLayout.Flow,
				RepeatDirection = RepeatDirection.Horizontal,
			};
			urlRelativenessList.Items.Add("Site collection");
			urlRelativenessList.Items.Add("Site");
			urlRelativenessList.SelectedIndex = IsSiteRelative ? 0 : 1;

			urlRelativenessList.Load += (sender, e) =>
			{
				IsSiteRelative = ((RadioButtonList)sender).SelectedIndex == 0;
			};

			l = new Label()
			{
				Text = "Relative to",
				AssociatedControlID = urlRelativenessList.ID
			};

			Controls.Add(CreateSection(l, urlRelativenessList));

			base.CreateChildControls();
		}

		#endregion Control Members
	}
}
