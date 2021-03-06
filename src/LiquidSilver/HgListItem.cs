﻿using System;
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
		public virtual int Id
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

		private static bool? GetBool(object value)
		{
			return (bool?)value;
		}

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
			return GetBool(ListItem[fieldId]);
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
			return GetBool(ListItem[fieldName]);
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
			ListItem[fieldName] = value;
		}

		#endregion Boolean Field

		#region Calculated Field

		private static string GetCalculated(SPField field, object value)
		{
			var f = field as SPFieldCalculated;

			return (f == null || value == null) ? null
				: (f.GetFieldValueAsText((string)value));
		}

		/// <summary>
		/// Gets a calculated value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetCalculated(Guid fieldId)
		{
			return GetCalculated(ListItem.Fields[fieldId], ListItem[fieldId]);
		}

		/// <summary>
		/// Gets a calculated value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetCalculated(string fieldName)
		{
			return GetCalculated(ListItem.Fields[fieldName], ListItem[fieldName]);
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
			return (DateTime?)ListItem[fieldId];
		}

		/// <summary>
		/// Gets a <see cref="DateTime"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual DateTime? GetDate(string fieldName)
		{
			return (DateTime?)ListItem[fieldName];
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
			ListItem[fieldName] = value;
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
			return (double?)ListItem[fieldName];
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
			ListItem[fieldName] = value;
		}

		#endregion Double Field

		#region Guid Field

		private static Guid? GetGuid(object value)
		{
			return (value == null) ? null :
				(Guid?)new Guid((string)value);
		}

		/// <summary>
		/// Gets an <see cref="Guid"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual Guid? GetGuid(Guid fieldId)
		{
			return GetGuid(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets an <see cref="Guid"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual Guid? GetGuid(string fieldName)
		{
			return GetGuid(ListItem[fieldName]);
		}

		/// <summary>
		/// Sets an <see cref="Guid"/> value to a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetGuid(Guid fieldId, Guid? value)
		{
			ListItem[fieldId] = value;
		}

		/// <summary>
		/// Sets an <see cref="Guid"/> value to a field.
		/// </summary>
		/// <param name="fieldName">The field's ID.</param>
		/// <param name="value">The value to set.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual void SetGuid(string fieldName, Guid? value)
		{
			ListItem[fieldName] = value;
		}

		#endregion Guid Field

		#region Int Field

		private static int? GetInt(object value)
		{
			return (value == null) ? null :
				(int?)Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets an <see cref="int"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual int? GetInt(Guid fieldId)
		{
			return GetInt(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets an <see cref="int"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual int? GetInt(string fieldName)
		{
			return GetInt(ListItem[fieldName]);
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
			ListItem[fieldName] = value;
		}

		#endregion Int Field

		#region Lookup Field

		private static SPFieldLookupValue GetLookup(object value)
		{
			var s = (string)value;
			return string.IsNullOrEmpty(s) ? null : new SPFieldLookupValue(s);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValue"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValue GetLookup(Guid fieldId)
		{
			return GetLookup(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldLookupValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldLookupValue GetLookup(string fieldName)
		{
			return GetLookup(ListItem[fieldName]);
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
			ListItem[fieldName] = value;
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
			SetLookup(fieldName, new SPFieldLookupValue(lookupId, lookupValue));
		}

		private SPListItem GetLookupItem(SPField field, string lookupValue)
		{
			var lookupField = field as SPFieldLookup;

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

			return item;
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
			var lookupItem = GetLookupItem(ListItem.Fields[fieldId],
				lookupValue);

			SetLookup(fieldId, lookupItem.ID, lookupValue);
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
			var lookupItem = GetLookupItem(ListItem.Fields[fieldName],
				lookupValue);

			SetLookup(fieldName, lookupItem.ID, lookupValue);
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
			return (SPFieldLookupValueCollection)ListItem[fieldName];
		}

		private static SPFieldLookupValueCollection
			GetFieldLookupValueCoolection(int[] lookupIds)
		{
			var lookups = new SPFieldLookupValueCollection();
			foreach (var i in lookupIds)
			{
				lookups.Add(new SPFieldLookupValue(i, ""));
			}
			return lookups;
		}

		private static SPFieldLookupValueCollection
			GetFieldLookupValueCoolection(string[] lookupValues)
		{
			var lookups = new SPFieldLookupValueCollection();
			foreach (var s in lookupValues)
			{
				lookups.Add(new SPFieldLookupValue(s));
			}
			return lookups;
		}

		private static SPFieldLookupValueCollection GetFieldLookupValueCoolection(
			SPFieldLookupValue[] lookupValues)
		{
			var lookups = new SPFieldLookupValueCollection();
			lookups.AddRange(lookupValues);
			return lookups;
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
			ListItem[fieldId] = GetFieldLookupValueCoolection(lookupIds);
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
			ListItem[fieldName] = GetFieldLookupValueCoolection(lookupIds);
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
			ListItem[fieldId] = GetFieldLookupValueCoolection(lookupValues);
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
			ListItem[fieldName] = GetFieldLookupValueCoolection(lookupValues);
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
			ListItem[fieldId] = GetFieldLookupValueCoolection(lookupValues);
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
			ListItem[fieldName] = GetFieldLookupValueCoolection(lookupValues);
		}

		#endregion Multiple Lookup Field

		#region Principal Field

		private SPPrincipal GetPrincipal(object value)
		{
			var s = (string)value;
			if (string.IsNullOrEmpty(s))
				return null;

			var uv = new SPFieldUserValue(ListItem.Web, s);

			return (SPPrincipal)uv.User
				?? ListItem.Web.SiteGroups[uv.LookupValue];
		}

		/// <summary>
		/// Gets an <see cref="SPPrincipal"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPPrincipal GetPrincipal(Guid fieldId)
		{
			return GetPrincipal(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets an <see cref="SPPrincipal"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPPrincipal GetPrincipal(string fieldName)
		{
			return GetPrincipal(ListItem[fieldName]);
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
			ListItem[fieldName] = principal;
		}

		#endregion Principal Field

		#region Multiple Principal Field

		private IEnumerable<SPPrincipal> GetPrincipals(object value)
		{
			var s = (string)value;

			if (string.IsNullOrEmpty(s))
				yield break;

			var userValues = new SPFieldUserValueCollection(ListItem.Web, s);

			foreach (SPFieldUserValue uv in userValues)
			{
				yield return (SPPrincipal)uv.User
					?? ListItem.Web.SiteGroups[uv.LookupValue];
			}
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> GetPrincipals(Guid fieldId)
		{
			return GetPrincipals(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets a collection of <see cref="SPPrincipal"/> values from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual IEnumerable<SPPrincipal> GetPrincipals(string fieldName)
		{
			return GetPrincipals(ListItem[fieldName]);
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
			ListItem[fieldName] = principals;
		}

		private static string GetPrincipalsAsCsv(IEnumerable<SPPrincipal> principals)
		{
			var sb = new StringBuilder();

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
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The comma-separated principal values.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual string GetPrincipalsAsCsv(Guid fieldId)
		{
			return GetPrincipalsAsCsv(GetPrincipals(fieldId));
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
			return GetPrincipalsAsCsv(GetPrincipals(fieldName));
		}

		private SPFieldUserValueCollection GetPrincipalsFromCsv(
			string principalsAsCsv)
		{
			var web = ListItem.Web;

			var userValues = new SPFieldUserValueCollection();
			var principals = principalsAsCsv.Split(',');

			foreach (string s in principals)
			{
				var principalName = s.Trim();

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

			return userValues;
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

			ListItem[fieldId] = GetPrincipalsFromCsv(principalsAsCsv);
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
			ListItem[fieldName] = GetPrincipalsFromCsv(principalsAsCsv);
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
			return (string)ListItem[fieldName];
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
			ListItem[fieldName] = value;
		}

		#endregion String Field

		#region URL Field

		private static SPFieldUrlValue GetUrl(object value)
		{
			var s = (string)value;
			return string.IsNullOrEmpty(s) ? null : new SPFieldUrlValue(s);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldUrlValue"/> value from a field.
		/// </summary>
		/// <param name="fieldId">The field's ID.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldUrlValue GetUrl(Guid fieldId)
		{
			return GetUrl(ListItem[fieldId]);
		}

		/// <summary>
		/// Gets an <see cref="SPFieldUrlValue"/> value from a field.
		/// </summary>
		/// <param name="fieldName">The field's name.</param>
		/// <returns>The field's value.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual SPFieldUrlValue GetUrl(string fieldName)
		{
			return GetUrl(ListItem[fieldName]);
		}

		private static SPFieldUrlValue GetUrlValue(string url, string description)
		{
			return new SPFieldUrlValue()
			{
				Description = description,
				Url = url
			};
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
			ListItem[fieldId] = GetUrlValue(url, description);
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
			ListItem[fieldName] = GetUrlValue(url, description);
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

		private bool DoesPrincipalsEqual(IEnumerable<SPPrincipal> principals,
			string principalFieldValue)
		{
			var principals1 = principals.ToList();

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
		/// <param name="fieldId">The ID of the field containing the
		///		principal.</param>
		/// <param name="principalFieldValue">The principal's field value to
		///		compare.</param>
		/// <returns><code>true</code> if the two principals are equal.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public virtual bool DoesPrincipalsEqual(Guid fieldId,
			string principalFieldValue)
		{
			return DoesPrincipalsEqual(GetPrincipals(fieldId), principalFieldValue);
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
			return DoesPrincipalsEqual(GetPrincipals(fieldName), principalFieldValue);
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