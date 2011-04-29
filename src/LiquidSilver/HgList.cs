using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;

namespace LiquidSilver
{
	/// <summary>
	/// Wraps and extends the <see cref="SPList"/> class.
	/// </summary>
	public class HgList
	{
		#region Constants

		/// <summary>
		/// By default, 2000 items will be retrieved each time for batch
		/// operations. Querying more than 2000 items at once in SharePoint
		/// will hinder the performance.
		/// </summary>
		const uint DefaultBatchSize = 1000;

		#endregion Constants

		#region Constructors

		/// <summary>
		/// Creates a new unbound instance of the <see cref="HgList"/> class.
		/// </summary>
		public HgList() { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="list">The list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPList list)
		{
			_list = list;
			Web = list.ParentWeb;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="web">The Web where the list resides.</param>
		/// <param name="listId">The ID of the list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPWeb web, Guid listId)
		{
			_list = web.Lists[listId];
			Web = web;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="web">The Web where the list resides.</param>
		/// <param name="listName">The name of the list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPWeb web, string listName)
		{
			_list = web.Lists[listName];
			Web = web;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the bound list.
		/// </summary>
		public virtual SPList List
		{
			get { return _list; }

			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set
			{
				if (_list == null)
				{
					_list = value;
					Web = _list.ParentWeb;
				}
				else
					throw new InvalidOperationException(
						"The List property can only be set once.");
			}
		}
		private SPList _list;

		/// <summary>
		/// Gets the parent Web of the bound list.
		/// </summary>
		public virtual SPWeb Web { get; private set; }

		/// <summary>
		/// Gets an empty <see cref="SPListItemCollection"/> object to generate
		/// new items. Use this instead of <see cref="SPListItem"/>.Items which
		/// is not optimal.
		/// </summary>
		protected virtual SPListItemCollection ItemsGenerator
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get
			{
				if (_itemsGenerator == null)
				{
					var query = new SPQuery { Query = "0" };
					_itemsGenerator = List.GetItems(query);
				}

				return _itemsGenerator;
			}
		}
		private SPListItemCollection _itemsGenerator;

		#endregion Properties

		#region Public Methods

		/// <summary>
		/// Adds a new file to the document library.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="fileContent">A byte array of the file content.</param>
		/// <param name="parentFolder">The parent folder for the file.</param>
		/// <param name="overwrite">If <code>true</code>, overwrite the
		///		existing file.</param>
		/// <returns>A new <see cref="SPFile" /> instance.</returns>
		/// <exception cref="HgFileExistsException">will be thrown if
		///		<paramref name="overwrite"/> is <code>false</code> and there is
		///		a file with the same name already exists.</exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFile AddFile(string fileName, byte[] fileContent,
			SPFolder parentFolder, bool overwrite)
		{
			var fileUrl = parentFolder.Url + "/" + fileName;

			if (!overwrite && Web.GetFile(fileUrl).Exists)
				throw new HgFileExistsException();

			return parentFolder.Files.Add(fileUrl, fileContent, overwrite);
		}

		/// <summary>
		/// Adds a new file to the document library.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="fileStream">The file content's stream.</param>
		/// <param name="parentFolder">The parent folder for the file.</param>
		/// <param name="overwrite">If <code>true</code>, overwrite the
		///		existing file.</param>
		/// <returns>A new <see cref="SPFile" /> instance.</returns>
		/// <exception cref="HgFileExistsException">will be thrown if
		///		<paramref name="overwrite"/> is <code>false</code> and there is
		///		a file with the same name already exists.</exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFile AddFile(string fileName, Stream fileStream,
			SPFolder parentFolder, bool overwrite)
		{
			var fileUrl = parentFolder.Url + "/" + fileName;

			if (!overwrite && Web.GetFile(fileUrl).Exists)
				throw new HgFileExistsException();

			return parentFolder.Files.Add(fileUrl, fileStream, overwrite);
		}

		/// <summary>
		/// Prepares to add a new folder to the list. Call the
		///		<code>Update()</code> method to actually add the folder.
		/// </summary>
		/// <param name="folderName">The folder name to add.</param>
		/// <param name="parentFolder">The parent folder of the new folder.
		///		</param>
		/// <returns>A new <see cref="SPFolder" /> instance.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFolder AddFolder(string folderName, SPFolder parentFolder)
		{
			var query = new SPQuery { Query = "0" };
			var item = List.GetItems(query).Add(parentFolder.ServerRelativeUrl,
				SPFileSystemObjectType.Folder, folderName);
			item.Update();
			return item.Folder;
		}

		/// <summary>
		/// Adds a complete folder structure to the list's root folder. The
		/// path must be separated by forward slashes (/), not backslashes (\).
		/// </summary>
		/// <param name="path">The complete path of the folder structure.</param>
		/// <returns>The last folder in the structure.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFolder AddFolderStructure(string path)
		{
			path = path.Trim().Trim('/');

			var rootFolder = List.RootFolder;

			if (string.IsNullOrEmpty(path))
				return rootFolder;

			var folder = Web.GetFolder(path);

			if (folder.Exists)
				return folder;

			if (!List.EnableFolderCreation)
				throw new InvalidOperationException(string.Format(
					CultureInfo.CurrentCulture,
					@"The list ""{0}"" does not allow folder creation.",
					SPUrlUtility.CombineUrl(Web.Url, rootFolder.Url)));

			var parentFolder = rootFolder;

			var folderPaths = path.Split('/');
			var folderPath = rootFolder.Url;

			foreach (var s in folderPaths)
			{
				folderPath += "/" + s;
				folder = Web.GetFolder(folderPath);

				if (!folder.Exists)
					folder = AddFolder(s, parentFolder);

				parentFolder = folder;
			}

			if (!folder.Exists)
				throw new InvalidOperationException(string.Format(
					CultureInfo.CurrentCulture,
					@"Error creating the ""{0}"" folder under ""{1}"".",
					path,
					SPUrlUtility.CombineUrl(Web.Url, rootFolder.Url)));

			return folder;
		}

		/// <summary>
		/// Prepares to add a new item to the list. Call the
		/// <code>Update()</code> method to actually add the item.
		/// </summary>
		/// <returns>A new <see cref="SPListItem" /> instance.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPListItem AddItem()
		{
			return ItemsGenerator.Add();
		}

		/// <summary>
		/// Gets all items from the list with paging to improve performance.
		/// </summary>
		/// <returns>A list of items.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1024:UsePropertiesWhereAppropriate"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IList<SPListItem> GetAllItems()
		{
			var result = new List<SPListItem>();

			var query = new SPQuery();
			query.RowLimit = DefaultBatchSize;

			do
			{
				var items = List.GetItems(query);
				result.AddRange(items.Cast<SPListItem>());

				query.ListItemCollectionPosition =
					items.ListItemCollectionPosition;
			} while (query.ListItemCollectionPosition != null);

			return result;
		}

		/// <summary>
		/// Updates a file in a document library.
		/// </summary>
		/// <param name="file">The file to update.</param>
		/// <param name="fileContent">A byte array of the file content.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void UpdateFile(SPFile file, byte[] fileContent)
		{
			if (file.InDocumentLibrary)
				file.CheckOut();

			AddFile(file.Name, fileContent, file.ParentFolder, true);

			if (file.InDocumentLibrary)
				file.CheckIn("");
		}

		/// <summary>
		/// Updates a file in a document library.
		/// </summary>
		/// <param name="file">The file to update.</param>
		/// <param name="fileStream">The file content's stream.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void UpdateFile(SPFile file, Stream fileStream)
		{
			if (file.InDocumentLibrary)
				file.CheckOut();

			AddFile(file.Name, fileStream, file.ParentFolder, true);

			if (file.InDocumentLibrary)
				file.CheckIn("");
		}

		#endregion Public Methods

		#region Batch Methods

		/// <summary>
		/// Performs a batch delete operation.
		/// </summary>
		/// <param name="continueOnError">If true, the operation will continue
		///		even there is an error.</param>
		/// <param name="itemIds">The list of IDs from the items to delete.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void BatchDelete(bool continueOnError,
			params int[] itemIds)
		{
			var ids = new List<string>();

			foreach (int i in itemIds)
			{
				ids.Add(i.ToString(CultureInfo.InvariantCulture));
			}

			BatchDelete(continueOnError, ids.ToArray());
		}

		/// <summary>
		/// Performs a batch delete operation.
		/// </summary>
		/// <param name="continueOnError">If true, the operation will continue
		///		even though there is an error.</param>
		/// <param name="itemIds">The list of IDs from the items to delete.
		/// </param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void BatchDelete(bool continueOnError,
			params string[] itemIds)
		{
			var sb = new StringBuilder();

			sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>")
				.AppendFormat(@"<ows:Batch OnError=""{0}"">",
					continueOnError ? "Continue" : "Return")
				.AppendLine();

			string listID = List.ID.ToString();

			foreach (string id in itemIds)
			{
				sb.AppendFormat(@"<Method ID=""{0}"">", id)
					.AppendLine()
					.AppendFormat(@"<SetList Scope=""Request"">{0}</SetList>", listID)
					.AppendLine()
					.AppendLine(@"<SetVar Name=""Cmd"">Delete</SetVar>")
					.AppendFormat(@"<SetVar Name=""ID"">{0}</SetVar>", id)
					.AppendLine()
					.AppendLine(@"</Method>");
			}

			sb.AppendLine(@"</ows:Batch>");

			Web.ProcessBatchData(sb.ToString());
		}

		/// <summary>
		/// Performs a batch update operation.
		/// </summary>
		/// <param name="continueOnError">If true, the operation will continue
		///		even though there is an error.</param>
		/// <param name="items">The list of <see cref="HgBatchItemDictionary"/>
		///		values to update.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void BatchUpdate(bool continueOnError,
			params HgBatchItemDictionary[] items)
		{
			const string XmlSchema = "urn:schemas-microsoft-com:office:office";

			var sb = new StringBuilder();

			sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>")
				.AppendFormat(@"<ows:Batch OnError=""{0}"">",
					continueOnError ? "Continue" : "Return")
				.AppendLine();

			var listID = List.ID.ToString();

			foreach (var item in items)
			{
				var id = item.ItemId.ToString(CultureInfo.InvariantCulture);

				sb.AppendFormat(@"<Method ID=""{0}"">", id)
					.AppendLine()
					.AppendFormat(@"<SetList Scope=""Request"">{0}</SetList>", listID)
					.AppendLine()
					.AppendLine(@"<SetVar Name=""Cmd"">Save</SetVar>")
					.AppendFormat(@"<SetVar Name=""ID"">{0}</SetVar>", id)
					.AppendLine();

				foreach (var kvp in item)
				{
					sb.AppendFormat(@"<SetVar Name=""{0}#{1}"">{2}</SetVar>",
							XmlSchema, kvp.Key, kvp.Value)
						.AppendLine();
				}

				sb.AppendLine(@"</Method>")
					.AppendLine();
			}

			sb.AppendLine(@"</ows:Batch>");

			Web.ProcessBatchData(sb.ToString());
		}

		/// <summary>
		/// Clears all items in the list using the batch operation with the
		/// default batch size.
		/// </summary>
		/// <param name="continueOnError">If true, the operation will continue
		///		even there is an error.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void ClearItems(bool continueOnError)
		{
			ClearItems(continueOnError, DefaultBatchSize);
		}

		/// <summary>
		/// Clears all items in the list using the batch operation with the
		/// specified batch size.
		/// </summary>
		/// <param name="continueOnError">If true, the operation will continue
		///		even there is an error.</param>
		/// <param name="batchSize">The number of items to delete in each
		///		batch.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void ClearItems(bool continueOnError, uint batchSize)
		{
			var query = new SPQuery()
			{
				ViewFields = "<FieldRef Name='ID' />",
				RowLimit = (batchSize > 0) ? batchSize : DefaultBatchSize
			};

			if (batchSize > 0)
				query.RowLimit = batchSize;

			do
			{
				var ids = new List<string>();
				var items = List.GetItems(query);

				foreach (SPListItem item in items)
				{
					ids.Add(item.ID.ToString(CultureInfo.InvariantCulture));
				}

				BatchDelete(continueOnError, ids.ToArray());

				query.ListItemCollectionPosition = items.ListItemCollectionPosition;
			} while (query.ListItemCollectionPosition != null);
		}

		#endregion Batch Methods
	}

	/// <summary>
	/// Wraps and extends the <see cref="SPList"/> class.
	/// </summary>
	/// <typeparam name="T">The type of element in the list.</typeparam>
	public class HgList<T> : HgList where T : HgListItem, new()
	{
		#region Constructors

		/// <summary>
		/// Creates a new unbound instance of the <see cref="HgList"/> class.
		/// </summary>
		public HgList() : base() { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="list">The list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPList list) : base(list) { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="web">The Web where the list resides.</param>
		/// <param name="listId">The ID of the list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPWeb web, Guid listId) : base(web, listId) { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgList"/> class.
		/// </summary>
		/// <param name="web">The Web where the list resides.</param>
		/// <param name="listName">The name of the list to bind.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgList(SPWeb web, string listName) : base(web, listName) { }

		#endregion Constructors

		#region CS1911 Helper Methods

		/// <summary>
		/// Prevents unverifiable code. Check <c>Compiler Warning (level 1)
		/// CS1911</c> for more information.
		/// </summary>
		private IList<SPListItem> BaseGetAllItems()
		{
			return base.GetAllItems();
		}

		#endregion CS1911 Helper Methods

		#region HgList Members

		/// <summary>
		/// Prepares to add a new item to the list. Call the
		/// <code>Update()</code> method to actually add the item.
		/// </summary>
		/// <returns>A new item.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual new T AddItem()
		{
			return new T() { ListItem = base.AddItem() };
		}

		/// <summary>
		/// Gets all items from the list.
		/// </summary>
		/// <returns>A series of items.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1024:UsePropertiesWhereAppropriate")]
		public virtual new IEnumerable<T> GetAllItems()
		{
			var items = BaseGetAllItems();

			foreach (var item in items)
			{
				yield return new T() { ListItem = item };
			}
		}

		/// <summary>
		/// Gets an item of the list.
		/// </summary>
		/// <param name="id">The ID of the item to retrieve.</param>
		/// <returns>An item.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual T GetItem(int id)
		{
			return new T() { ListItem = List.GetItemById(id) };
		}

		#endregion HgList Members
	}
}