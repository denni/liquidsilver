using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Wraps and extends the <see cref="SPListItem"/> class.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgListItem
	{
		#region Constructors

		/// <summary>
		/// Creates a new unbound instance of the <see cref="HgListItem"/>
		///	class.
		/// </summary>
		public HgListItem() { }

		/// <summary>
		/// Creates a new instance of the <see cref="HgListItem"/> class.
		/// </summary>
		/// <param name="item"></param>
		public HgListItem(SPListItem item)
		{
			_listItem = item;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the bound list item.
		/// </summary>
		public virtual SPListItem ListItem
		{
			get { return _listItem; }

			set
			{
				if (_listItem == null)
					_listItem = value;
				else
					throw new InvalidOperationException(
						"The ListItem property can only be set once.");
			}
		}
		private SPListItem _listItem;

		/// <summary>
		/// Gets the item's ID.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
			"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
		public virtual int ID
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItem.ID; }
		}

		/// <summary>
		/// Gets the item's unique ID.
		/// </summary>
		public virtual Guid UniqueId
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItem.UniqueId; }
		}

		/// <summary>
		/// Gets or sets the item's title.
		/// </summary>
		public virtual string Title
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItem.Title; }
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set { SetString(SPBuiltInFieldId.Title, value); }
		}

		/// <summary>
		/// Gets or sets the item's author.
		/// </summary>
		public virtual SPUser Author
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return (SPUser)GetPrincipal(SPBuiltInFieldId.Author); }
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set { SetPrincipal(SPBuiltInFieldId.Author, value); }
		}

		/// <summary>
		/// Gets or sets the item's last editor.
		/// </summary>
		public virtual SPUser Editor
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return (SPUser)GetPrincipal(SPBuiltInFieldId.Editor); }
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set { SetPrincipal(SPBuiltInFieldId.Editor, value); }
		}

		/// <summary>
		/// Gets the item's creation date.
		/// </summary>
		public virtual DateTime CreatedOn
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return GetDate(SPBuiltInFieldId.Created) ?? default(DateTime); }
		}

		/// <summary>
		/// Gets the item's last modification date.
		/// </summary>
		public virtual DateTime ModifiedOn
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return GetDate(SPBuiltInFieldId.Modified) ?? default(DateTime); }
		}

		/// <summary>
		/// Gets or sets the item's content type name.
		/// </summary>
		public virtual string ContentTypeName
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return GetString(SPBuiltInFieldId.ContentType); }

			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set { ContentTypeId = ListItem.ParentList.ContentTypes[value].Id; }
		}

		/// <summary>
		/// Gets or sets the item's content type ID.
		/// </summary>
		public virtual SPContentTypeId ContentTypeId
		{
			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			get { return ListItem.ContentType.Id; }

			[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
			set { ListItem[SPBuiltInFieldId.ContentTypeId] = value; }
		}

		#endregion Properties

		#region Field Parser Methods

		#region Boolean Field

		/// <summary>
		/// Gets a <see cref="bool"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
			"CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual bool? GetBool(Guid fieldId)
		{
			return (bool?)ListItem[fieldId];
		}

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
			return GetBool(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a <see cref="bool"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetBool(Guid fieldId, bool? value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets a <see cref="bool"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetBool(string fieldName, bool? value)
		{
			SetBool(GetFieldId(fieldName), value);
		}

		#endregion Boolean Field

		#region Calculated Field

		/// <summary>
		/// Gets a calculated value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetCalculated(Guid fieldId)
		{
			var field = (SPFieldCalculated)ListItem.Fields[fieldId];
			return (field == null) ? null
				: (field.GetFieldValueAsText(GetString(fieldId)));
		}

		/// <summary>
		/// Gets a calculated value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetCalculated(string fieldName)
		{
			return GetCalculated(GetFieldId(fieldName));
		}

		#endregion Calculated Field

		#region DateTime Field

		/// <summary>
		/// Gets a <see cref="DateTime"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual DateTime? GetDate(Guid fieldId)
		{
			if (ListItem[fieldId] == null)
				return null;

			return DateTime.Parse(ListItem[fieldId].ToString(),
				CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets a <see cref="DateTime"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual DateTime? GetDate(string fieldName)
		{
			return GetDate(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a <see cref="DateTime"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetDate(Guid fieldId, DateTime? value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets a <see cref="DateTime"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetDate(string fieldName, DateTime? value)
		{
			SetDate(GetFieldId(fieldName), value);
		}

		#endregion DateTime Field

		#region Double Field

		/// <summary>
		/// Gets a <see cref="double"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual double? GetDouble(Guid fieldId)
		{
			return (double?)ListItem[fieldId];
		}

		/// <summary>
		/// Gets a <see cref="double"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual double? GetDouble(string fieldName)
		{
			return GetDouble(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a <see cref="double"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetDouble(Guid fieldId, double? value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets a <see cref="double"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetDouble(string fieldName, double? value)
		{
			SetDouble(GetFieldId(fieldName), value);
		}

		#endregion Double Field

		#region Int Field

		/// <summary>
		/// Gets an <see cref="int"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual int? GetInt(Guid fieldId)
		{
			var value = ListItem[fieldId];
			return (value == null) ? null :
				(int?)Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets an <see cref="int"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual int? GetInt(string fieldName)
		{
			return GetInt(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets an <see cref="int"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetInt(Guid fieldId, int? value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets an <see cref="int"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetInt(string fieldName, int? value)
		{
			SetInt(GetFieldId(fieldName), value);
		}

		#endregion Int Field

		#region Lookup Field

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValue"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValue GetLookup(Guid fieldId)
		{
			var s = GetString(fieldId);
			return string.IsNullOrEmpty(s) ? null : new SPFieldLookupValue(s);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValue GetLookup(string fieldName)
		{
			return GetLookup(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValue"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(Guid fieldId, SPFieldLookupValue value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValue"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(string fieldName, SPFieldLookupValue value)
		{
			SetLookup(GetFieldId(fieldName), value);
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValue"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="lookupId">The lookup ID to set.</param>
		/// <param name="lookupValue">The lookup value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(Guid fieldId,
			int lookupId, string lookupValue)
		{
			SetLookup(fieldId, new SPFieldLookupValue(lookupId, lookupValue));
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValue"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="lookupId">The lookup ID to set.</param>
		/// <param name="lookupValue">The lookup value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(string fieldName,
			int lookupId, string lookupValue)
		{
			SetLookup(GetFieldId(fieldName), lookupId, lookupValue);
		}

		/// <summary>
		/// Sets a lookup value to a field. This method scans the lookup
		///		value in the lookup list.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="lookupValue">The lookup value to set.</param>
		/// <exception cref="System.ArgumentException">Thrown when the
		///		specified field is not a valid lookup field.</exception>
		///	<exception cref="System.ArgumentOutOfRangeException">Thrown when
		///		the specified lookup value cannot be found in the lookup list.
		///	</exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(Guid fieldId, string lookupValue)
		{
			var lookupField = ListItem.Fields[fieldId] as SPFieldLookup;

			if (lookupField == null)
				throw new ArgumentException(
					"The field is not a valid lookup field.");

			var lookupList = ListItem.Web.Lists[new Guid(lookupField.LookupList)];

			var query = new SPQuery()
			{
				Query = string.Format(CultureInfo.InvariantCulture,
					@"<Where><Eq>
						<FieldRef Name='{0}' />
						<Value Type='Text'>{1}</Value>
					</Eq></Where>",
				  lookupField.LookupField,
				  lookupValue),
				ViewFields = @"<FieldRef Name='ID' />",
				RowLimit = 1
			};

			var item = lookupList.GetItems(query)
				.Cast<SPListItem>().FirstOrDefault();

			if (item == null)
				throw new ArgumentOutOfRangeException("lookupValue", lookupValue,
					string.Format(CultureInfo.InvariantCulture,
						@"Could not find the specified lookup value ""{0}"" in the lookup list.",
						lookupValue));

			SetLookup(fieldId, item.ID, lookupValue);
		}

		/// <summary>
		/// Sets a lookup value to a field. This method scans the lookup
		///		value in the lookup list.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="lookupValue">The lookup value to set.</param>
		/// <exception cref="System.ArgumentException">Thrown when the
		///		specified field is not a valid lookup field.</exception>
		///	<exception cref="System.ArgumentOutOfRangeException">Thrown when
		///		the specified lookup value cannot be found in the lookup list.
		///	</exception>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetLookup(string fieldName, string lookupValue)
		{
			SetLookup(GetFieldId(fieldName), lookupValue);
		}

		#endregion Lookup Field

		#region Multiple Lookup Field

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValueCollection"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1002:DoNotExposeGenericLists"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValueCollection GetMultipleLookup(
			Guid fieldId)
		{
			return (SPFieldLookupValueCollection)ListItem[fieldId];
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
			return GetMultipleLookup(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="lookupIds">The lookup IDs to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(Guid fieldId,
			params int[] lookupIds)
		{
			var lookups = new SPFieldLookupValueCollection();
			foreach (var i in lookupIds)
			{
				lookups.Add(new SPFieldLookupValue(i, ""));
			}

			ListItem[fieldId] = lookups;
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="lookupIds">The lookup IDs to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(string fieldName,
			params int[] lookupIds)
		{
			SetMultipleLookup(GetFieldId(fieldName), lookupIds);
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="lookupValues">The lookup values to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(Guid fieldId,
			params string[] lookupValues)
		{
			var lookups = new SPFieldLookupValueCollection();
			foreach (var s in lookupValues)
			{
				lookups.Add(new SPFieldLookupValue(s));
			}

			ListItem[fieldId] = lookups;
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="lookupValues">The lookup values to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(string fieldName,
			params string[] lookupValues)
		{
			SetMultipleLookup(GetFieldId(fieldName), lookupValues);
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="lookupValues">The lookup values to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(Guid fieldId,
			params SPFieldLookupValue[] lookupValues)
		{
			var lookups = new SPFieldLookupValueCollection();
			lookups.AddRange(lookupValues);
			ListItem[fieldId] = lookups;
		}

		/// <summary>
		/// Sets an <see cref="SPFieldLookupValueCollection"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="lookupValues">The lookup values to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetMultipleLookup(string fieldName,
			params SPFieldLookupValue[] lookupValues)
		{
			SetMultipleLookup(GetFieldId(fieldName), lookupValues);
		}

		#endregion Multiple Lookup Field

		#region Principal Field

		/// <summary>
		/// Gets an <see cref="SPPrincipal"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPPrincipal GetPrincipal(Guid fieldId)
		{
			var s = GetString(fieldId);
			if (string.IsNullOrEmpty(s))
				return null;

			var uv = new SPFieldUserValue(ListItem.Web, s);

			return (SPPrincipal)uv.User
				?? ListItem.Web.SiteGroups[uv.LookupValue];
		}

		/// <summary>
		/// Gets an <see cref="SPPrincipal"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPPrincipal GetPrincipal(string fieldName)
		{
			return GetPrincipal(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets an <see cref="SPPrincipal"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="principal">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipal(Guid fieldId, SPPrincipal principal)
		{
			ListItem[fieldId] = principal;
		}

		/// <summary>
		/// Sets an <see cref="SPPrincipal"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="principal">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipal(string fieldName, SPPrincipal principal)
		{
			SetPrincipal(GetFieldId(fieldName), principal);
		}

		#endregion Principal Field

		#region Multiple Principal Field

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> GetPrincipals(Guid fieldId)
		{
			var principals = new List<SPPrincipal>();

			var s = GetString(fieldId);

			if (string.IsNullOrEmpty(s))
				return principals;

			var userValues = new SPFieldUserValueCollection(ListItem.Web, s);

			foreach (SPFieldUserValue uv in userValues)
			{
				principals.Add((SPPrincipal)uv.User
					?? ListItem.Web.SiteGroups[uv.LookupValue]);
			}

			return principals;
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> GetPrincipals(string fieldName)
		{
			return GetPrincipals(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a collection of <see cref="SPPrincipal"/> values to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="principals">The value to set.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1002:DoNotExposeGenericLists"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipals(Guid fieldId,
			SPFieldUserValueCollection principals)
		{
			ListItem[fieldId] = principals;
		}

		/// <summary>
		/// Sets a collection of <see cref="SPPrincipal"/> values to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="principals">The value to set.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1002:DoNotExposeGenericLists"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipals(string fieldName,
			SPFieldUserValueCollection principals)
		{
			SetPrincipals(GetFieldId(fieldName), principals);
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field
		/// as a comma-separated values.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The comma-separated principal values.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetPrincipalsAsCsv(Guid fieldId)
		{
			var sb = new StringBuilder();

			var principals = GetPrincipals(fieldId);
			foreach (var p in principals)
			{
				if (sb.Length > 0)
					sb.Append(",");

				sb.Append(p.Name);
			}

			return sb.ToString();
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
			return GetPrincipalsAsCsv(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a comma-separated <see cref="SPPrincipal"/> values to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="principalsAsCsv">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipalsFromCsv(Guid fieldId,
			string principalsAsCsv)
		{
			var web = ListItem.Web;

			var userValues = new SPFieldUserValueCollection();
			var principals = principalsAsCsv.Split(',');

			foreach (string s in principals)
			{
				string principalName = s.Trim();

				if (principalName.Length == 0)
					continue;

				SPFieldUserValue uv;

				try
				{
					SPGroup group = web.SiteGroups[principalName];
					uv = new SPFieldUserValue(web, group.ID, group.Name);
				}
				catch (SPException)
				{
					//Group not found, so it could be a user instead of group.
					try
					{
						var user = web.EnsureUser(principalName);
						uv = new SPFieldUserValue(web, user.ID, user.LoginName);
					}
					catch (SPException)
					{
						//User not found, must be a garbage data.
						continue;
					}
				}

				userValues.Add(uv);
			}

			ListItem[fieldId] = userValues;
		}

		/// <summary>
		/// Sets a comma-separated <see cref="SPPrincipal"/> values to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="principalsAsCsv">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetPrincipalsFromCsv(string fieldName,
			string principalsAsCsv)
		{
			SetPrincipalsFromCsv(GetFieldId(fieldName), principalsAsCsv);
		}

		#endregion Multiple Principal Field

		#region String Field

		/// <summary>
		/// Gets a <see cref="string"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetString(Guid fieldId)
		{
			return (string)ListItem[fieldId];
		}

		/// <summary>
		/// Gets a <see cref="string"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetString(string fieldName)
		{
			return GetString(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets a <see cref="string"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetString(Guid fieldId, string value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets a <see cref="string"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetString(string fieldName, string value)
		{
			SetString(GetFieldId(fieldName), value);
		}

		#endregion String Field

		#region URL Field

		/// <summary>
		/// Gets an <see cref="SPFieldUrlValue"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldUrlValue GetUrl(Guid fieldId)
		{
			var s = GetString(fieldId);

			return string.IsNullOrEmpty(s) ? null : new SPFieldUrlValue(s);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldUrlValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldUrlValue GetUrl(string fieldName)
		{
			return GetUrl(GetFieldId(fieldName));
		}

		/// <summary>
		/// Sets an <see cref="SPFieldUrlValue"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="url">The URL to set.</param>
		/// <param name="description">The description of the URL.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "1#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetUrl(Guid fieldId, string url, string description)
		{
			ListItem[fieldId] = new SPFieldUrlValue()
			{
				Description = description,
				Url = url
			};
		}

		/// <summary>
		/// Sets an <see cref="SPFieldUrlValue"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <param name="url">The URL to set.</param>
		/// <param name="description">The description of the URL.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1054:UriParametersShouldNotBeStrings", MessageId = "1#"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetUrl(string fieldName, string url, string description)
		{
			SetUrl(GetFieldId(fieldName), url, description);
		}

		#endregion URL Field

		#endregion Field Parser Methods

		#region Field Helper Methods

		/// <summary>
		/// Gets the field's ID from the name.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The ID of the field.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual Guid GetFieldId(string fieldName)
		{
			try
			{
				return ListItem.Fields.GetFieldByInternalName(fieldName).Id;
			}
			catch (ArgumentException)
			{
				try
				{
					return ListItem.Fields[fieldName].Id;
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(string.Format(
						CultureInfo.InvariantCulture,
						"Could not find the field with internal name or display name of '{0}'.",
						fieldName), ex);
				}
			}
		}

		/// <summary>
		/// Gets the field's internal name from the ID.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's name.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetFieldInternalName(Guid fieldId)
		{
			return ListItem.Fields[fieldId].InternalName;
		}

		#endregion Field Helper Methods

		#region Principals Helper Methods

		/// <summary>
		/// Compares whether two principals are equal.
		/// </summary>
		/// <param name="fieldId">The ID of the field containing the
		///		principal.</param>
		/// <param name="principalFieldValue">The principal's field value to
		///		compare.</param>
		/// <returns><code>true</code> if the two principals are equal.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual bool DoesPrincipalsEqual(Guid fieldId,
			string principalFieldValue)
		{
			var principals1 = GetPrincipals(fieldId).ToList();

			if (principalFieldValue.Length == 0 && principals1.Count == 0)
				return true;
			else if (principalFieldValue.Length == 0 && principals1.Count != 0)
				return false;
			else if (principalFieldValue.Length != 0 && principals1.Count == 0)
				return false;

			var principals2 =
				ExtractPrincipalsFromString(principalFieldValue).ToList();

			if (principals1.Count != principals2.Count)
				return false;

			principals1.Sort((x, y) => x.ID.CompareTo(y.ID));
			principals2.Sort((x, y) => x.ID.CompareTo(y.ID));

			for (int i = 0; i < principals1.Count; i++)
			{
				if (principals1[i].ID != principals2[i].ID)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Compares whether two principals are equal.
		/// </summary>
		/// <param name="fieldName">The name of the field containing the
		///		principal.</param>
		/// <param name="principalFieldValue">The principal's field value to
		///		compare.</param>
		/// <returns><code>true</code> if the two principals are equal.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual bool DoesPrincipalsEqual(string fieldName,
			string principalFieldValue)
		{
			return DoesPrincipalsEqual(GetFieldId(fieldName),
				principalFieldValue);
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a string.
		/// </summary>
		/// <param name="principalFieldValue">The string containing the
		///		principals.</param>
		/// <returns>A collection of <see cref="SPPrincipal"/> values.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> ExtractPrincipalsFromString(
			string principalFieldValue)
		{
			var principals = new List<SPPrincipal>();

			var userValues = new SPFieldUserValueCollection(ListItem.Web,
				principalFieldValue);

			foreach (SPFieldUserValue uv in userValues)
			{
				if (uv.User != null)
					principals.Add(uv.User);
				else
					principals.Add(ListItem.Web.SiteGroups[uv.LookupValue]);
			}

			return principals;
		}

		#endregion Principals Helper Methods

		#region Security Context Methods

		private SPListItem _oldItemContext;

		/// <summary>
		/// A flag that tells whether the current security context is elevated.
		/// </summary>
		public virtual bool IsElevated { get; private set; }

		/// <summary>
		/// Elevates the current security context so operations can be done as
		/// the System Account.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void ElevateContext()
		{
			//TODO: implements a routine to handle the internal references of
			//SPRequest objects to avoid memory leak.

			if (_oldItemContext == null)
			{
				_oldItemContext = ListItem;
				_listItem = HgSecurity.GetElevatedItem(ListItem);
				IsElevated = true;
			}
		}

		/// <summary>
		/// Restores the current security context to use the current user's
		/// credential instead of the System Account's.
		/// </summary>
		public virtual void RestoreContext()
		{
			if (_oldItemContext != null)
			{
				_listItem = _oldItemContext;
				_oldItemContext = null;
				IsElevated = false;
			}
		}

		#endregion Security Context Methods

		#region Miscellaneous Helper Methods

		/// <summary>
		/// Deletes the current item.
		/// </summary>
		/// <param name="recycle">If <code>true</code> then move the item to
		///		the recycle bin.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void Delete(bool recycle)
		{
			if (recycle)
				ListItem.Recycle();
			else
				ListItem.Delete();
		}

		/// <summary>
		/// Reloads the current item's state and discards the pending changes.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void Reload()
		{
			_listItem = ListItem.ParentList.GetItemById(ListItem.ID);
		}

		/// <summary>
		/// Updates the database with changes that are made to the item,
		/// without effecting the changes in the <see cref="Editor"/> and
		/// <see cref="ModifiedOn"/> fields.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SystemUpdate()
		{
			ListItem.SystemUpdate();
		}

		/// <summary>
		/// Updates the database with changes that are made to the item,
		/// without effecting the changes in the <see cref="Editor"/> and
		/// <see cref="ModifiedOn"/> fields, or optionally, the item version.
		/// </summary>
		/// <param name="incrementListItemVersion"><code>true</code> to
		///		increment the item version; otherwise, <code>false</code>.
		///	</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SystemUpdate(bool incrementListItemVersion)
		{
			ListItem.SystemUpdate(incrementListItemVersion);
		}

		/// <summary>
		/// Updates the database with changes that are made to the item.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void Update()
		{
			ListItem.Update();
		}

		#endregion Miscellaneous Helper Methods
	}
}