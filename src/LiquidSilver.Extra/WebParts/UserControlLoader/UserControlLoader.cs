using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;

namespace LiquidSilver.Extra.WebParts
{
	/// <summary>
	/// A type of <see cref="UserControlLoader<T>"/> that loads any type of
	/// <see cref="UserControl"/>.
	/// </summary>
	public class UserControlLoader : UserControlLoader<UserControl>
	{
	}

	/// <summary>
	/// Provides a base class for a <see cref="WebPart"/> that loads a
	/// <see cref="UserControl"/>.
	/// It accepts a URL and a flag to indicate the URL relativity.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="UserControl"/> to load.</typeparam>
	public abstract class UserControlLoader<T>
		: Microsoft.SharePoint.WebPartPages.WebPart, IUserControlLoader
		where T : UserControl
	{
		#region Events

		public event Action<object, UserControlLoaderEventArgs<T>> UserControlCreated;

		protected virtual void OnUserControlCreated(UserControlLoaderEventArgs<T> e)
		{
			if (UserControlCreated != null)
				UserControlCreated(this, e);
		}

		#endregion Events

		#region Protected Methods

		protected void SetError(Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException(ex.GetType().Name);

			string message = new StringBuilder()
				.AppendLine(ex.Message)
				.AppendLine("<!--")
				.AppendLine(ex.ToString())
				.AppendLine("-->")
				.Replace(Environment.NewLine, "<br />" + Environment.NewLine)
				.ToString();

			SetError(message);
		}

		protected void SetError(string message)
		{
			if (string.IsNullOrEmpty(message))
				throw new ArgumentNullException(message.GetType().Name);

			Controls.Add(new LiteralControl(new StringBuilder()
				.AppendFormat(@"<div class=""ms-formvalidation"">{0}</div>",
					message)
				.AppendLine()
				.ToString()));
		}

		#endregion Protected Methods

		#region WebPart Members

		protected override void CreateChildControls()
		{
			try
			{
				if (string.IsNullOrEmpty(UserControlUrl))
					throw new ArgumentNullException(UserControlUrl.GetType().Name);

				string url = SPUrlUtility.CombineUrl(
					IsSiteRelative ? SPContext.Current.Site.ServerRelativeUrl
					: SPContext.Current.Web.ServerRelativeUrl,
					UserControlUrl);

				UserControl = (T)Page.LoadControl(url);

				Controls.Add(UserControl);

				OnUserControlCreated(new UserControlLoaderEventArgs<T>(UserControl));
			}
			catch (Exception ex)
			{
				SetError(ex);
			}

			base.CreateChildControls();
		}

		public override ToolPart[] GetToolParts()
		{
			List<ToolPart> parts = new List<ToolPart>(base.GetToolParts());
			parts.Insert(0, new UserControlLoaderToolPart());
			return parts.ToArray();
		}

		#endregion WebPart Members

		#region IUserControlLoader Members

		[Category("Settings"),
		WebBrowsable(false),
		Personalizable(PersonalizationScope.Shared),
		WebDisplayName("Is Site Relative"),
		Description("Specify whether the URL is relative to the site collection.")]
		public bool IsSiteRelative { get; set; }

		[Category("Settings"),
		WebBrowsable(false),
		Personalizable(PersonalizationScope.Shared),
		WebDisplayName("User Control URL"),
		Description("The URL of the user control to load.")]
		public string UserControlUrl { get; set; }

		protected T UserControl { get; private set; }

		#endregion IUserControlLoader Members
	}

	/// <summary>
	/// Provides the event data for <see cref="UserControlLoader"/> events.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class UserControlLoaderEventArgs<T> : EventArgs
		where T : UserControl
	{
		public UserControlLoaderEventArgs(T userControl)
		{
			this.UserControl = userControl;
		}

		public T UserControl { get; private set; }
	}
}
