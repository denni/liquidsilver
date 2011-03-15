using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Microsoft.SharePoint.Utilities;

namespace LiquidSilver.Tests
{
	/// <summary>
	///This is a test class for HgListTest and is intended
	///to contain all HgListTest Unit Tests
	///</summary>
	[TestClass()]
	public class HgListTest
	{
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

		private string actorsListUrl;
		private string testDocumentsListUrl;

		private const string TestFileContent = "test";

		private Stream GetTestFileStream()
		{
			return new MemoryStream(GetTestFileContent());
		}

		private byte[] GetTestFileContent()
		{
			return Encoding.Default.GetBytes(TestFileContent);
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
			this.actorsListUrl = TestSettings.ActorsListUrl;
			this.testDocumentsListUrl = TestSettings.DocumentsListUrl;
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
		///A test for HgList Constructor
		///</summary>
		[TestMethod()]
		public void HgListConstructorTest3()
		{
			TestListContext.Execute(this.actorsListUrl, (site, web, list) =>
			{
				HgList target = new HgList(web, list.ID);
				Assert.AreEqual(list.ID, target.List.ID);
			});
		}

		/// <summary>
		///A test for HgList Constructor
		///</summary>
		[TestMethod()]
		public void HgListConstructorTest2()
		{
			TestListContext.Execute(this.actorsListUrl, (site, web, list) =>
			{
				HgList target = new HgList(web, list.Title);
				Assert.AreEqual(list.ID, target.List.ID);
			});
		}

		/// <summary>
		///A test for HgList Constructor
		///</summary>
		[TestMethod()]
		public void HgListConstructorTest1()
		{
			if (SPContext.Current == null || SPContext.Current.Web == null || SPContext.Current.List == null)
			{
				// Cannot execute this test when the context is null.
				return;
			}

			HgList target = new HgList();
			Assert.AreEqual(SPContext.Current.ListId, target.List.ID);
		}

		/// <summary>
		///A test for HgList Constructor
		///</summary>
		[TestMethod()]
		public void HgListConstructorTest()
		{
			TestListContext.Execute(this.actorsListUrl, (site, web, list) =>
			{
				HgList target = new HgList(list);
				Assert.AreEqual(list.ID, target.List.ID);
			});
		}

		/// <summary>
		///A test for AddFile
		///</summary>
		[TestMethod()]
		public void AddFileTest()
		{
			TestListContext.Execute(this.testDocumentsListUrl, (site, web, list) =>
			{
				string fileName = Guid.NewGuid().ToString("N");
				byte[] fileContent = GetTestFileContent();
				SPFolder parentFolder = list.RootFolder;
				bool overwrite = true;
				var target = new HgList(list);

				SPFile file = target.AddFile(fileName, fileContent, parentFolder, overwrite);

				Assert.IsTrue(file.Exists);

				if (file != null && file.Exists)
				{
					file.Delete();
					file = null;
				}

				using (Stream fileStream = GetTestFileStream())
				{
					file = target.AddFile(fileName, fileStream, parentFolder, overwrite);
				}

				Assert.IsTrue(file.Exists);

				if (file != null && file.Exists)
				{
					file.Delete();
					file = null;
				}
			});
		}

		/// <summary>
		///A test for AddFolder
		///</summary>
		[TestMethod()]
		public void AddFolderTest()
		{
			TestListContext.Execute(this.testDocumentsListUrl, (site, web, list) =>
			{
				string folderName = Guid.NewGuid().ToString("N");
				SPFolder parentFolder = list.RootFolder;
				var target = new HgList(list);

				SPFolder folder = target.AddFolder(folderName, parentFolder);

				Assert.AreEqual(folderName, folder.Name);

				if (folder != null && folder.Exists)
					folder.Delete();
			});
		}

		/// <summary>
		///A test for AddFolderStructure
		///</summary>
		[TestMethod()]
		public void AddFolderStructureTest()
		{
			TestListContext.Execute(this.testDocumentsListUrl, (site, web, list) =>
			{
				string path = "Hello/World";
				SPFolder parentFolder = list.RootFolder;
				var target = new HgList(list);

				SPFolder folder = target.AddFolderStructure(path);

				Assert.AreEqual(list.Title + "/" + path, folder.Url);

				if (folder != null && folder.Exists)
				{
				}
			});
		}

		/// <summary>
		///A test for AddItem
		///</summary>
		[TestMethod()]
		public void AddItemTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			SPListItem expected = null; // TODO: Initialize to an appropriate value
			SPListItem actual;
			actual = target.AddItem();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for BatchDelete
		///</summary>
		[TestMethod()]
		public void BatchDeleteTest1()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			bool continueOnError = false; // TODO: Initialize to an appropriate value
			string[] itemIds = null; // TODO: Initialize to an appropriate value
			target.BatchDelete(continueOnError, itemIds);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for BatchDelete
		///</summary>
		[TestMethod()]
		public void BatchDeleteTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			bool continueOnError = false; // TODO: Initialize to an appropriate value
			int[] itemIds = null; // TODO: Initialize to an appropriate value
			target.BatchDelete(continueOnError, itemIds);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for BatchUpdate
		///</summary>
		[TestMethod()]
		public void BatchUpdateTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			bool continueOnError = false; // TODO: Initialize to an appropriate value
			HgBatchItemDictionary[] items = null; // TODO: Initialize to an appropriate value
			target.BatchUpdate(continueOnError, items);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ClearItems
		///</summary>
		[TestMethod()]
		public void ClearItemsTest1()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			bool continueOnError = false; // TODO: Initialize to an appropriate value
			target.ClearItems(continueOnError);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ClearItems
		///</summary>
		[TestMethod()]
		public void ClearItemsTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			bool continueOnError = false; // TODO: Initialize to an appropriate value
			uint batchSize = 0; // TODO: Initialize to an appropriate value
			target.ClearItems(continueOnError, batchSize);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for GetAllItems
		///</summary>
		[TestMethod()]
		public void GetAllItemsTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			IList<SPListItem> expected = null; // TODO: Initialize to an appropriate value
			IList<SPListItem> actual;
			actual = target.GetAllItems();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for UpdateFile
		///</summary>
		[TestMethod()]
		public void UpdateFileTest1()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			SPFile file = null; // TODO: Initialize to an appropriate value
			Stream fileStream = null; // TODO: Initialize to an appropriate value
			target.UpdateFile(file, fileStream);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for UpdateFile
		///</summary>
		[TestMethod()]
		public void UpdateFileTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			SPFile file = null; // TODO: Initialize to an appropriate value
			byte[] fileContent = null; // TODO: Initialize to an appropriate value
			target.UpdateFile(file, fileContent);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ItemsGenerator
		///</summary>
		[TestMethod()]
		[DeploymentItem("LiquidSilver.dll")]
		public void ItemsGeneratorTest()
		{
			HgList_Accessor target = new HgList_Accessor(); // TODO: Initialize to an appropriate value
			SPListItemCollection actual;
			actual = target.ItemsGenerator;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for List
		///</summary>
		[TestMethod()]
		public void ListTest()
		{
			HgList target = new HgList(); // TODO: Initialize to an appropriate value
			SPList expected = null; // TODO: Initialize to an appropriate value
			SPList actual;
			target.List = expected;
			actual = target.List;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Web
		///</summary>
		[TestMethod()]
		[DeploymentItem("LiquidSilver.dll")]
		public void WebTest()
		{
			HgList_Accessor target = new HgList_Accessor(); // TODO: Initialize to an appropriate value
			SPWeb expected = null; // TODO: Initialize to an appropriate value
			SPWeb actual;
			target.Web = expected;
			actual = target.Web;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
