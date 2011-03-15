using System.IO;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiquidSilver.Tests
{
	/// <summary>
	///This is a test class for HgContextTest and is intended
	///to contain all HgContextTest Unit Tests
	///</summary>
	[TestClass()]
	public class HgContextTest
	{
		private string siteUrl;

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		[TestInitialize()]
		public void MyTestInitialize()
		{
			this.siteUrl = TestSettings.SiteUrl;
		}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		public void ExecuteTest2()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					SPWeb web = site.RootWeb;

					HgContext.Execute(web, false, (s, w) =>
					{
						Assert.AreEqual(web.ID, w.ID);
						Assert.AreEqual(web.CurrentUser.LoginName, w.CurrentUser.LoginName);
					});

					HgContext.Execute(web, true, (s, w) =>
					{
						Assert.AreEqual(web.ID, w.ID);

						string saLoginName = site.SystemAccount.LoginName;
						if (web.CurrentUser.LoginName != saLoginName)
							Assert.AreEqual(saLoginName, w.CurrentUser.LoginName);
					});
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		public void ExecuteTest1()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					using (var web = site.OpenWeb())
					{
						HgContext.Execute(web, false, (s, w) =>
						{
							Assert.AreEqual(web.ID, w.ID);
							Assert.AreEqual(web.CurrentUser.LoginName, w.CurrentUser.LoginName);
						});

						HgContext.Execute(web, true, (s, w) =>
						{
							Assert.AreEqual(web.ID, w.ID);

							string saLoginName = site.SystemAccount.LoginName;
							if (web.CurrentUser.LoginName != saLoginName)
								Assert.AreEqual(saLoginName, w.CurrentUser.LoginName);
						});
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		public void ExecuteTest()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					SPWeb web = site.RootWeb;

					HgContext.Execute(site, false, (s, w) =>
					{
						Assert.AreEqual(web.ID, w.ID);
						Assert.AreEqual(web.CurrentUser.LoginName, w.CurrentUser.LoginName);
					});

					HgContext.Execute(site, true, (s, w) =>
					{
						Assert.AreEqual(web.ID, w.ID);

						string saLoginName = site.SystemAccount.LoginName;
						if (web.CurrentUser.LoginName != saLoginName)
							Assert.AreEqual(saLoginName, w.CurrentUser.LoginName);
					});
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}
