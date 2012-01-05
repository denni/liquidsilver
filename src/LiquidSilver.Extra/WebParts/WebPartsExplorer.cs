using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.WebControls;
using System.Globalization;

namespace LiquidSilver.Extra.WebParts
{
	public class WebPartsExplorer : WebPart
	{
		#region Constructors

		public WebPartsExplorer()
		{
		}

		#endregion Constructors

		#region Properties

		[Category("Custom"),
		Personalizable(PersonalizationScope.Shared),
		WebBrowsable(true),
		WebDisplayName("Include lists"),
		WebDescription("Indicates whether to include lists in the search "
			+ "result, otherwise only document libraries will be included")]
		public bool IncludeLists { get; set; }

		[Category("Custom"),
		Personalizable(PersonalizationScope.Shared),
		WebBrowsable(true),
		WebDisplayName("Include sub sites"),
		WebDescription("Indicates whether the sub sites should be included")]
		public bool IncludeSubSites { get; set; }

		[Category("Custom"),
		Personalizable(PersonalizationScope.Shared),
		WebBrowsable(true),
		WebDisplayName("Show Web part's full name"),
		WebDescription("Indicates whether to show the Web part's full name")]
		public bool ShowWebPartFullName { get; set; }

		[Category("Custom"),
		Personalizable(PersonalizationScope.Shared),
		WebBrowsable(true),
		WebDisplayName("Site URL"),
		WebDescription("The absolute URL of the site to search for the Web "
			+ "parts. If not specified, the current site will be searched.")]
		public string SiteUrl { get; set; }

		#endregion Properties

		#region Private Methods

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		private IEnumerable<WebPartInfo> GetResult()
		{
			var list = new WebPartInfoList();

			if (string.IsNullOrEmpty(SiteUrl))
			{
				list.AddRange(GetResult(SPContext.Current.Web));
			}
			else
			{
				using (var site = new SPSite(SiteUrl))
				{
					using (var web = site.OpenWeb())
					{
						list.AddRange(GetResult(web));
					}
				}
			}

			return list;
		}

		private IEnumerable<WebPartInfo> GetResult(SPWeb web)
		{
			var wpList = new WebPartInfoList();

			wpList.AddRange(GetResult(web.Files));

			foreach (SPList list in web.Lists)
			{
				if (!(IncludeLists || list is SPDocumentLibrary))
					continue;

				wpList.AddRange(GetResult(list.Items));
			}

			if (IncludeSubSites)
			{
				foreach (SPWeb subWeb in web.Webs)
				{
					try
					{
						wpList.AddRange(GetResult(subWeb));
					}
					finally
					{
						subWeb.Dispose();
					}
				}
			}

			return wpList;
		}

		private IEnumerable<WebPartInfo> GetResult(IEnumerable<SPFile> files)
		{
			var wpList = new WebPartInfoList();

			SPWeb web = null;

			foreach (var file in files)
			{
				if (!file.Name.EndsWith(".aspx",
					StringComparison.OrdinalIgnoreCase))
					continue;

				if (web == null)
					web = file.ParentFolder.ParentWeb;

				Microsoft.SharePoint.WebPartPages.SPLimitedWebPartManager wpMan;

				try
				{
					wpMan = web.GetLimitedWebPartManager(file.Url,
						PersonalizationScope.Shared);
				}
				catch (SPException)
				{
					continue;
				}

				foreach (WebPart wp in wpMan.WebParts)
				{
					var wpType = wp.GetType();
					wpList.Add(ShowWebPartFullName ? wpType.FullName : wpType.Name,
						file.ServerRelativeUrl);
				}
			}

			return wpList;
		}

		private IEnumerable<WebPartInfo> GetResult(SPFileCollection files)
		{
			return GetResult(files.Cast<SPFile>());
		}

		private IEnumerable<WebPartInfo> GetResult(SPListItemCollection items)
		{
			var files = items.Cast<SPListItem>()
				.Where(x => x.File != null)
				.Select(x => x.File);

			return GetResult(files);
		}

		private DataTable GetResultAsDataTable()
		{
			var dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;

			var columns = dt.Columns;

			columns.Add("No.", typeof(int));
			columns.Add("Web Part", typeof(string));
			columns.Add("Page URL", typeof(string));

			var rows = dt.Rows;
			var i = 1;
			foreach (var wpInfo in GetResult())
			{
				rows.Add(i++, wpInfo.ClassName, wpInfo.PageUrl);
			}

			return dt;
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		private Control GetResultAsGrid()
		{
			var grid = new SPGridView()
			{
				AutoGenerateColumns = false,
				EnableViewState = true
			};

			var dt = GetResultAsDataTable();
			var columns = dt.Columns;
			for (int i = 0; i < columns.Count; i++)
			{
				var columnName = columns[i].ColumnName;

				var field = new SPBoundField()
				{
					DataField = columnName,
					HeaderText = columnName
				};

				grid.Columns.Add(field);
			}

			grid.DataSource = dt;
			grid.DataBind();

			return grid;
		}

		#endregion Private Methods

		#region WebPart Members

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			if (WebPartManager.DisplayMode != WebPartManager.BrowseDisplayMode)
				return;

			if (!Page.IsPostBack)
			{
				var grid = GetResultAsGrid();
				Controls.Add(grid);
			}
		}

		#endregion WebPart Members

		#region Inner Classes

		internal class WebPartInfo
		{
			public string ClassName { get; set; }
			public string PageUrl { get; set; }
		}

		internal class WebPartInfoList : List<WebPartInfo>
		{
			public WebPartInfoList Add(string className, string pageUrl)
			{
				Add(new WebPartInfo()
				{
					ClassName = className,
					PageUrl = pageUrl
				});

				return this;
			}
		}

		#endregion Inner Classes
	}
}