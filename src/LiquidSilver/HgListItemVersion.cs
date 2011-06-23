using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Wraps and extends the <see cref="SPListItemVersion"/> class.
	/// </summary>
	public class HgListItemVersion
	{
		#region Constructors

		/// <summary>
		/// Creates a new unbound instance of the <see cref="HgListItemVersion"/>
		///	class.
		/// </summary>
		public HgListItemVersion() { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgListItemVersion"/> class.
		/// </summary>
		/// <param name="itemVersion">Specifiy the <c>SPListItemVersion</c>
		///		object to bind.</param>
		public HgListItemVersion(SPListItemVersion itemVersion)
		{
			_listItemVersion = itemVersion;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the bound list item version.
		/// </summary>
		public virtual SPListItemVersion ListItemVersion
		{
			get { return _listItemVersion; }

			set
			{
				if (_listItemVersion == null)
					_listItemVersion = value;
				else
					throw new InvalidOperationException(
						"The ListItemVersion property can only be set once.");
			}
		}
		private SPListItemVersion _listItemVersion;

		/// <summary>
		/// Gets or sets the associated item.
		/// </summary>
		public virtual HgListItem Item
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get
			{
				if (_item == null)
					_item = new HgListItem(ListItemVersion.ListItem);

				return _item;
			}

			set
			{
				if (_item == null)
					_item = value;
				else
					throw new InvalidOperationException(
						"The Item property can only be set once.");
			}
		}
		private HgListItem _item;

		/// <summary>
		/// Gets the creation date of the last version.
		/// </summary>
		public DateTime ModifiedOn
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItemVersion.Created; }
		}

		/// <summary>
		/// Gets the user that creates the last version.
		/// </summary>
		public SPUser Editor
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItemVersion.CreatedBy.User; }
		}

		#endregion Properties

		#region Field Parser Methods

		/// <summary>
		/// Gets a <see cref="bool"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
			"CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual bool? GetBool(string fieldName)
		{
			return (bool?)ListItemVersion[fieldName];
		}

		/// <summary>
		/// Gets a calculated value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetCalculated(string fieldName)
		{
			var field = (SPFieldCalculated)ListItemVersion.Fields[fieldName];
			return (field == null) ? null
				: (field.GetFieldValueAsText(GetString(fieldName)));
		}

		/// <summary>
		/// Gets a <see cref="DateTime"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual DateTime? GetDate(string fieldName)
		{
			if (ListItemVersion[fieldName] == null)
				return null;

			return DateTime.Parse(ListItemVersion[fieldName].ToString(),
				CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets a <see cref="double"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual double? GetDouble(string fieldName)
		{
			return (double?)ListItemVersion[fieldName];
		}

		/// <summary>
		/// Gets an <see cref="int"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual int? GetInt(string fieldName)
		{
			var value = ListItemVersion[fieldName];
			return (value == null) ? null
				: (int?)Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValue GetLookup(string fieldName)
		{
			var s = GetString(fieldName);
			return string.IsNullOrEmpty(s) ? null : new SPFieldLookupValue(s);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValueCollection"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1002:DoNotExposeGenericLists"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValueCollection GetMultipleLookup(
			string fieldName)
		{
			return (SPFieldLookupValueCollection)ListItemVersion[fieldName];
		}

		/// <summary>
		/// Gets an <see cref="SPPrincipal"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPPrincipal GetPrincipal(string fieldName)
		{
			var s = GetString(fieldName);
			if (string.IsNullOrEmpty(s))
				return null;

			var uv = new SPFieldUserValue(ListItemVersion.ListItem.Web, s);

			return (SPPrincipal)uv.User
				?? ListItemVersion.ListItem.Web.SiteGroups[uv.LookupValue];
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> GetPrincipals(string fieldName)
		{
			var principals = new List<SPPrincipal>();

			var s = GetString(fieldName);

			if (string.IsNullOrEmpty(s))
				return principals;

			var userValues = new SPFieldUserValueCollection(ListItemVersion.ListItem.Web, s);

			foreach (SPFieldUserValue uv in userValues)
			{
				principals.Add((SPPrincipal)uv.User
					?? ListItemVersion.ListItem.Web.SiteGroups[uv.LookupValue]);
			}

			return principals;
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field
		/// as a comma-separated values.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The comma-separated principal values.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetPrincipalsAsCsv(string fieldName)
		{
			var sb = new StringBuilder();

			var principals = GetPrincipals(fieldName);
			foreach (var p in principals)
			{
				if (sb.Length > 0)
					sb.Append(",");

				sb.Append(p.Name);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Gets a <see cref="string"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetString(string fieldName)
		{
			return (string)ListItemVersion[fieldName];
		}

		/// <summary>
		/// Gets an <see cref="SPFieldUrlValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldUrlValue GetUrl(string fieldName)
		{
			var s = GetString(fieldName);
			return string.IsNullOrEmpty(s) ? null : new SPFieldUrlValue(s);
		}

		#endregion Field Parser Methods
	}
}