using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Provides an elevated context for <see cref="SPSite"/> and
	/// <see cref="SPWeb"/> objects. Operations on the objects will be under
	/// the System Account credential with the Full Control access.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgElevatedContext : DisposableBase, IDisposable
	{
		#region Constructors

		/// <summary>
		/// Creates an elevated context from the current
		/// <see cref="SPContext"/> object.
		/// </summary>
		/// <exception cref="System.NullReferenceException">
		/// Thrown when the current <see cref="SPContext"/> object is null.
		/// </exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgElevatedContext() : this(SPContext.Current.Web.Url) { }

		/// <summary>
		/// Creates an elevated context from a specified <see cref="SPSite"/>
		/// object.
		/// </summary>
		/// <param name="site">The <see cref="SPSite"/> object to get the
		///		context from.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgElevatedContext(SPSite site) : this(site.Url) { }

		/// <summary>
		/// Creates an elevated context from a specified <see cref="SPWeb"/>
		/// object.
		/// </summary>
		/// <param name="web">The <see cref="SPWeb"/> object to get the
		///		context from.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgElevatedContext(SPWeb web) : this(web.Url) { }

		/// <summary>
		/// Creates an elevated context from a specified URL.
		/// </summary>
		/// <param name="siteUrl">The site's URL to get the context from.</param>
		///	<exception cref="System.IO.FileNotFoundException">
		///	Thrown when the Site could not be found at the specified URL.
		///	</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgElevatedContext(string siteUrl)
		{
			Site = HgSecurity.GetElevatedSite(siteUrl);
			Web = Site.OpenWeb();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the <see cref="SPSite"/> object from the elevated context.
		/// </summary>
		public SPSite Site { get; private set; }

		/// <summary>
		/// Gets the <see cref="SPWeb"/> object from the elevated context.
		/// </summary>
		public SPWeb Web { get; private set; }

		#endregion Properties

		#region Delegates

		/// <summary>
		/// Executes the passed code in an elevated context with the Full
		/// Control access.
		/// </summary>
		/// <param name="code">The code to execute.</param>
		public static void Execute(CodeToRun code)
		{
			using (var context = new HgElevatedContext())
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in an elevated context with the Full
		/// Control access.
		/// </summary>
		/// <param name="site">The <see cref="SPSite"/> object to get the
		///		elevated context from.</param>
		/// <param name="code">The code to execute.</param>
		public static void Execute(SPSite site, CodeToRun code)
		{
			using (var context = new HgElevatedContext(site))
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in an elevated context with the Full
		/// Control access.
		/// </summary>
		/// <param name="web">The <see cref="SPWeb"/> object to get the
		///		elevated context from.</param>
		/// <param name="code">The code to execute.</param>
		public static void Execute(SPWeb web, CodeToRun code)
		{
			using (var context = new HgElevatedContext(web))
			{
				code(context.Site, context.Web);
			}
		}

		/// <summary>
		/// Executes the passed code in an elevated context with the Full
		/// Control access.
		/// </summary>
		/// <param name="web">The site's URL to get the elevated context
		///		from.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
		public static void Execute(string siteUrl, CodeToRun code)
		{
			using (var context = new HgElevatedContext(siteUrl))
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
	///  Represents methods that are passed to
	///  <see cref="HgElevatedContext.Execute"/>, so that the methods can run
	///  with the Full Control access.
	/// </summary>
	/// <param name="site">The <see cref="SPSite"/> object from the elevated
	///		context.</param>
	/// <param name="web">The <see cref="SPWeb"/> object from the elevated
	///		context.</param>
	public delegate void CodeToRun(SPSite site, SPWeb web);
}