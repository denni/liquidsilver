using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgElevatedContext : DisposableBase
	{
		#region Constructors

		public HgElevatedContext()
		{
		}

		#endregion Constructors

		#region Properties

		public SPSite Site { get; private set; }

		public SPWeb Web { get; private set; }

		public SPList List { get; private set; }

		public SPListItem ListItem { get; private set; }

		#endregion Properties

		#region Context Builder Methods

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromSite()
		{
			FromSite(SPContext.Current.Site);
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromSite(SPSite site)
		{
			Site = new SPSite(site.Url, GetSystemAccountToken(site));
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromSite(Guid siteId)
		{
			using (var site = new SPSite(siteId))
			{
				FromSite(site);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromSite(string siteUrl)
		{
			using (var site = new SPSite(siteUrl))
			{
				FromSite(site);
			}
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromWeb()
		{
			FromWeb(SPContext.Current.Web.Url);
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromWeb(SPWeb web)
		{
			FromWeb(web.Url);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "0#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromWeb(string webUrl)
		{
			FromSite(webUrl);
			Web = Site.OpenWeb();
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromList()
		{
			FromList(SPContext.Current.List);
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromList(SPList list)
		{
			FromWeb(list.ParentWebUrl);
			List = Web.Lists[list.ID];
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromItem()
		{
			FromItem(SPContext.Current.ListItem);
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public void FromItem(SPListItem item)
		{
			FromList(item.ParentList);
			ListItem = List.GetItemById(item.ID);
		}

		#endregion Context Builder Methods

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

		#region DisposableBase Members

		protected override void DisposeManagedResources()
		{
			Web = SafeDispose(Web);
			Site = SafeDispose(Site);
		}

		#endregion DisposableBase Members
	}
}