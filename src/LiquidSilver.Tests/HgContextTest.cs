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
		private TestContext testContextInstance;
		private string siteUrl;

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
		///A test for HgContext Constructor
		///</summary>
		[TestMethod()]
		public void HgContextConstructorTest4()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					using (var web = site.OpenWeb())
					{
						using (var target = new HgContext(web, false))
						{
							Assert.AreEqual(web.ID, target.Web.ID);
							Assert.AreEqual(web.CurrentUser.LoginName, target.Web.CurrentUser.LoginName);
						}

						using (var target = new HgContext(web, true))
						{
							Assert.AreEqual(web.ID, target.Web.ID);

							string saLoginName = site.SystemAccount.LoginName;
							if (web.CurrentUser.LoginName != saLoginName)
								Assert.AreEqual(saLoginName, target.Web.CurrentUser.LoginName);
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for HgContext Constructor
		///</summary>
		[TestMethod()]
		public void HgContextConstructorTest3()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					string saLoginName = site.SystemAccount.LoginName;

					using (var web = site.OpenWeb())
					{
						using (var target = new HgContext(this.siteUrl, false))
						{
							Assert.AreEqual(web.ID, target.Web.ID);
							Assert.AreEqual(web.CurrentUser.LoginName, target.Web.CurrentUser.LoginName);
						}

						using (var target = new HgContext(this.siteUrl, true))
						{
							Assert.AreEqual(web.ID, target.Web.ID);
							Assert.AreEqual(site.SystemAccount.LoginName, target.Web.CurrentUser.LoginName);
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for HgContext Constructor
		///</summary>
		[TestMethod()]
		public void HgContextConstructorTest2()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					SPWeb web = site.RootWeb;

					using (var target = new HgContext(site, false))
					{
						Assert.AreEqual(web.ID, target.Web.ID);
						Assert.AreEqual(web.CurrentUser.LoginName, target.Web.CurrentUser.LoginName);
					}

					using (var target = new HgContext(site, true))
					{
						Assert.AreEqual(web.ID, target.Web.ID);

						string saLoginName = site.SystemAccount.LoginName;
						if (web.CurrentUser.LoginName != saLoginName)
							Assert.AreEqual(saLoginName, target.Web.CurrentUser.LoginName);
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for HgContext Constructor
		///</summary>
		[TestMethod()]
		public void HgContextConstructorTest1()
		{
			if (SPContext.Current == null || SPContext.Current.Web == null)
			{
				// Cannot execute this test when the context is null.
				return;
			}

			using (var target = new HgContext())
			{
				Assert.IsNotNull(target.Site);
				Assert.IsTrue(target.Web.Exists);
			}
		}

		/// <summary>
		///A test for HgContext Constructor
		///</summary>
		[TestMethod()]
		public void HgContextConstructorTest()
		{
			if (SPContext.Current == null || SPContext.Current.Web == null)
			{
				// Cannot execute this test when the context is null.
				return;
			}

			using (var target = new HgContext(false))
			{
				Assert.IsNotNull(target.Site);
				Assert.IsTrue(target.Web.Exists);
			}

			using (var target = new HgContext(true))
			{
				Assert.IsNotNull(target.Site);
				Assert.IsTrue(target.Web.Exists);
				Assert.AreEqual(target.Site.SystemAccount.LoginName,
					target.Web.CurrentUser.LoginName);
			}
		}

		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		public void ExecuteTest3()
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
		public void ExecuteTest2()
		{
			try
			{
				using (var site = new SPSite(this.siteUrl))
				{
					string saLoginName = site.SystemAccount.LoginName;

					using (var web = site.OpenWeb())
					{
						using (var target = new HgContext(TestSettings.SiteUrl, false))
						{
							Assert.AreEqual(web.ID, target.Web.ID);
							Assert.AreEqual(web.CurrentUser.LoginName, target.Web.CurrentUser.LoginName);
						}

						using (var target = new HgContext(TestSettings.SiteUrl, true))
						{
							Assert.AreEqual(web.ID, target.Web.ID);
							Assert.AreEqual(site.SystemAccount.LoginName, target.Web.CurrentUser.LoginName);
						}
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
		public void ExecuteTest1()
		{
			if (SPContext.Current == null || SPContext.Current.Web == null)
			{
				// Cannot execute this test when the context is null.
				return;
			}

			HgContext.Execute((site, web) =>
			{
				Assert.IsNotNull(site);
				Assert.IsTrue(web.Exists);
			});
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

		/// <summary>
		///A test for Execute
		///</summary>
		[TestMethod()]
		public void ExecuteTest4()
		{
			if (SPContext.Current == null || SPContext.Current.Web == null)
			{
				// Cannot execute this test when the context is null.
				return;
			}

			HgContext.Execute(false, (site, web) =>
			{
				Assert.IsNotNull(site);
				Assert.IsTrue(web.Exists);
			});

			HgContext.Execute(true, (site, web) =>
			{
				Assert.IsNotNull(site);
				Assert.IsTrue(web.Exists);
				Assert.AreEqual(site.SystemAccount.LoginName, web.CurrentUser.LoginName);
			});
		}
	}
}
