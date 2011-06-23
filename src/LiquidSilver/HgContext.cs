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
	public static class HgContext
	{
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
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="site">The <see cref="SPSite"/> object to get the
		///		new context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static void Execute(SPSite site, bool elevateContext,
			Action<SPSite, SPWeb> code)
		{
			Execute(site.RootWeb, elevateContext, code);
		}

		/// <summary>
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="web">The <see cref="SPWeb"/> object to get the
		///		new context from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static void Execute(SPWeb web, bool elevateContext,
			Action<SPSite, SPWeb> code)
		{
			SPSite contextSite = null;
			try
			{
				if (elevateContext)
					contextSite = new SPSite(web.Url, GetSystemAccountToken(web.Site));
				else
					contextSite = new SPSite(web.Url);

				using (SPWeb contextWeb = contextSite.OpenWeb())
				{
					code(contextSite, contextWeb);
				}
			}
			finally
			{
				if (contextSite != null)
					contextSite.Dispose();
			}
		}

		/// <summary>
		/// Executes the passed code in a new context.
		/// </summary>
		/// <param name="siteUrl">The Site URL to get the new context
		///		from.</param>
		/// <param name="elevateContext">If true, the context will be
		///		elevated.</param>
		/// <param name="code">The code to execute.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true),
		System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),]
		public static void Execute(string siteUrl, bool elevateContext,
			Action<SPSite, SPWeb> code)
		{
			SPSite contextSite = null;
			try
			{
				contextSite = new SPSite(siteUrl);

				if (elevateContext)
				{
					SPSite site = new SPSite(siteUrl, GetSystemAccountToken(contextSite));
					contextSite.Dispose();
					contextSite = site;
				}

				using (SPWeb contextWeb = contextSite.OpenWeb())
				{
					code(contextSite, contextWeb);
				}
			}
			finally
			{
				if (contextSite != null)
					contextSite.Dispose();
			}
		}

		#endregion Delegates
	}
}