using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Provides a new SharePoint context from a given <see cref="SPSite"/>
	/// object, <see cref="SPWeb"/> object, or a URL. The new context can be
	/// elevated so operations within the context will be under the System
	/// Account credential with the Full Control access. Otherwise, they will
	/// be executed under the current user's credential.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgContext : DisposableBase, IDisposable
	{
		#region Constructors

		/// <summary>
		/// Creates a new non-elevated context from the current
		/// <see cref="SPContext"/> object.
		/// </summary>
		/// <exception cref="System.NullReferenceException">
		/// Thrown when the current <see cref="SPContext"/> object is null.
		/// </exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgContext() : this(SPContext.Current.Web.Url, false) { }

		/// <summary>
		/// Creates a new context from the current <see cref="SPContext"/>
		/// object.
		/// </summary>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <exception cref="System.NullReferenceException">
		/// Thrown when the current <see cref="SPContext"/> object is null.
		/// </exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgContext(bool elevateContext)
			: this(SPContext.Current.Web.Url, elevateContext) { }

		/// <summary>
		/// Creates a new context from a specified <see cref="SPSite"/> object.
		/// </summary>
		/// <param name="site">The <see cref="SPSite"/> object to get the
		///		context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgContext(SPSite site, bool elevateContext)
			: this(site.Url, elevateContext) { }

		/// <summary>
		/// Creates a new context from a specified <see cref="SPWeb"/> object.
		/// </summary>
		/// <param name="web">The <see cref="SPWeb"/> object to get the
		///		context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgContext(SPWeb web, bool elevateContext)
			: this(web.Url, elevateContext) { }

		/// <summary>
		/// Creates a new context from a specified URL.
		/// </summary>
		/// <param name="siteUrl">The site's URL to get the context
		///		from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		///	<exception cref="System.IO.FileNotFoundException">
		///	Thrown when the Site could not be found at the specified URL.
		///	</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgContext(string siteUrl, bool elevateContext)
		{
			if (elevateContext)
			{
				using (var site = new SPSite(siteUrl))
				{
					this.Site = new SPSite(siteUrl, GetSystemAccountToken(site));
				}
			}
			else
			{
				this.Site = new SPSite(siteUrl);
			}

			this.Web = this.Site.OpenWeb();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the <see cref="SPSite"/> object from the context.
		/// </summary>
		public SPSite Site { get; private set; }

		/// <summary>
		/// Gets the <see cref="SPWeb"/> object from the context.
		/// </summary>
		public SPWeb Web { get; private set; }

		#endregion Properties

		#region Private Methods

		/// <summary>
		/// Gets the System Account's user token.
		/// </summary>
		/// <param name="site">The site context.</param>
		/// <returns>The System Account's user token.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		private static SPUserToken GetSystemAccountToken(SPSite site)
		{
			site.CatchAccessDeniedException = false;
			try
			{
				return site.SystemAccount.UserToken;
			}
			catch (UnauthorizedAccessException)
			{
				SPUserToken token = null;

				SPSecurity.RunWithElevatedPrivileges(() =>
				{
					using (SPSite s = new SPSite(site.ID))
					{
						token = s.SystemAccount.UserToken;
					}
				});

				return token;
			}
		}

		#endregion Private Methods

		#region Delegates

		/// <summary>
		/// Executes the passed code in a new non-elevated context that is based on
		/// the current context.
		/// </summary>
		/// <param name="code">The code to execute.</param>
		public static void Execute(HgContextCode code)
		{
			using (var context = new HgContext())
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="site">The <see cref="SPSite"/> object to get the
		///		new context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		public static void Execute(SPSite site, bool elevateContext,
			HgContextCode code)
		{
			using (var context = new HgContext(site, elevateContext))
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="web">The <see cref="SPWeb"/> object to get the
		///		new context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		public static void Execute(SPWeb web, bool elevateContext,
			HgContextCode code)
		{
			using (var context = new HgContext(web, elevateContext))
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="web">The site's URL to get the new context
		///		from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
		public static void Execute(string siteUrl, bool elevateContext,
			HgContextCode code)
		{
			using (var context = new HgContext(siteUrl, elevateContext))
			{
				code(context.Site, context.Web);
			}
		}

		#endregion Delegates

		#region DisposableBase Members

		protected override void DisposeManagedResources()
		{
			Web = SafeDispose(Web);
			Site = SafeDispose(Site);

			base.DisposeManagedResources();
		}

		#endregion DisposableBase Members
	}

	/// <summary>
	///  Represents methods that are passed to <see cref="HgContext.Execute"/>,
	///  so that the methods can run within the context.
	/// </summary>
	/// <param name="site">The <see cref="SPSite"/> object from the context.</param>
	/// <param name="web">The <see cref="SPWeb"/> object from the context.</param>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public delegate void HgContextCode(SPSite site, SPWeb web);
}