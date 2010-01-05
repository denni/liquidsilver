using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiquidSilver;
using Microsoft.SharePoint;

namespace Ex01_ListAndItems
{
	class Program
	{
		const string SiteUrl = "http://your-server-url";
		const string ListName = "Actors";

		static void Main(string[] args)
		{
			using (var site = new SPSite(SiteUrl))
			{
				using (var web = site.OpenWeb())
				{
					var list = web.Lists[ListName];

					var sw = new Stopwatch();
					sw.Start();

					ClearItems(list);
					AddItems(list);
					UpdateItems(list);

					sw.Stop();

					Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds.ToString());
					Console.WriteLine("Press ENTER to continue.");
					Console.ReadLine();
				}
			}
		}

		private static void AddItems(SPList list)
		{
			var theList = new HgList<HgListItem>(list);

			for (int i = 0; i < 3; i++)
			{
				var tom = theList.AddItem();
				tom.Title = "Tom Cruise";
				tom.SetDate("Dob", new DateTime(1962, 7, 3));
				tom.Update();

				var katie = theList.AddItem();
				katie.Title = "Katie Holmes";
				katie.SetDate("Dob", new DateTime(1978, 12, 18));
				katie.SetLookup("Spouse", tom.ID, tom.Title);
				katie.Update();

				tom.SetLookup("Spouse", katie.ID, katie.Title);
				tom.Update();
			}
		}

		private static void ClearItems(SPList list)
		{
			new HgList(list).ClearItems(true);
		}

		private static void UpdateItems(SPList list)
		{
			// Prepare a list of HgBatchItemDictionary objects.
			var batchItems = new List<HgBatchItemDictionary>();

			var items = list.Items;
			foreach (SPListItem item in items)
			{
				// The HgBatchItemDictionary constructor takes the item's ID as the
				// parameter.
				var batchItem = new HgBatchItemDictionary(item.ID);

				// HgBatchItemDictionary uses key-value pairs to specify the field and
				// the field's value of the item.
				batchItem["Title"] = item.Title.ToUpper();

				batchItems.Add(batchItem);
			}

			// Continue the batch process even though there is an error.
			bool continueOnError = true;

			new HgList(list)
			  .BatchUpdate(continueOnError, batchItems.ToArray());
		}
	}
}