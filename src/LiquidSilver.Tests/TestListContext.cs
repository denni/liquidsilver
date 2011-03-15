using System;
using Microsoft.SharePoint;

namespace LiquidSilver.Tests
{
	public class TestListContext
	{
		public static void Execute(string listUrl, Action<SPSite, SPWeb, SPList> code)
		{
			HgContext.Execute(TestSettings.SiteUrl, false, (site, web) =>
			{
				SPList list = web.GetList(listUrl);
				code(site, web, list);
			});
		}
	}
}