using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// A helper class for SharePoint Security functionalities.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public static class HgSecurity
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

		#region Public Methods

		/// <summary>
		/// Gets the elevated context of the current site.
		/// </summary>
		/// <returns>The elevated site context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPSite GetElevatedSite()
		{
			return GetElevatedSite(SPContext.Current.Site);
		}

		/// <summary>
		/// Gets the elevated context of a site.
		/// </summary>
		/// <param name="site">The site to be elevated.</param>
		/// <returns>The elevated site context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPSite GetElevatedSite(SPSite site)
		{
			return new SPSite(site.ID, GetSystemAccountToken(site));
		}

		/// <summary>
		/// Gets the elevated context of a site.
		/// </summary>
		/// <param name="siteId">The ID of the site to be elevated.</param>
		/// <returns>The elevated site context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPSite GetElevatedSite(Guid siteId)
		{
			using (SPSite site = new SPSite(siteId))
			{
				return GetElevatedSite(site);
			}
		}

		/// <summary>
		/// Gets the elevated context of a site.
		/// </summary>
		/// <param name="siteUrl">The URL of the site to be elevated.</param>
		/// <returns>The elevated site context.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPSite GetElevatedSite(string siteUrl)
		{
			using (SPSite site = new SPSite(siteUrl))
			{
				// Need to maintain the site URL
				return new SPSite(siteUrl, GetSystemAccountToken(site));
			}
		}

		/// <summary>
		/// Gets the elevated context of the current Web.
		/// </summary>
		/// <returns>The elevated Web context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPWeb GetElevatedWeb()
		{
			return GetElevatedWeb(SPContext.Current.Web);
		}

		/// <summary>
		/// Gets the elevated context of a Web.
		/// </summary>
		/// <param name="web">The Web to be elevated.</param>
		/// <returns>The elevated Web context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPWeb GetElevatedWeb(SPWeb web)
		{
			using (SPSite site = GetElevatedSite(web.Url))
			{
				return site.OpenWeb();
			}
		}

		/// <summary>
		/// Gets the elevated context of a list.
		/// </summary>
		/// <param name="list">The list to be elevated.</param>
		/// <returns>The elevated list context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPList GetElevatedList(SPList list)
		{
			using (SPWeb web = GetElevatedWeb(list.ParentWeb))
			{
				return web.Lists[list.ID];
			}
		}

		/// <summary>
		/// Gets the elevated context of a list item.
		/// </summary>
		/// <param name="item">The list item to be elevated.</param>
		/// <returns>The elevated list item context.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static SPListItem GetElevatedItem(SPListItem item)
		{
			return GetElevatedList(item.ParentList).GetItemById(item.ID);
		}

		#endregion Public Methods
	}
}