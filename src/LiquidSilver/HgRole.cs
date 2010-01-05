using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Manages an SPRoleAssignmentCollection object.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgRole
	{
		#region Properties

		/// <summary>
		/// Gets the parent SPWeb object of the managed object.
		/// </summary>
		public SPWeb ParentWeb { get; private set; }

		/// <summary>
		/// Gets the SPRoleAssignmentCollection object which is being managed.
		/// </summary>
		public SPRoleAssignmentCollection RoleAssignments { get; private set; }

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Instantiates a new SPRoleAssignmentManager object to manage the
		/// specified SPRoleAssignmentCollection object.
		/// </summary>
		/// <param name="roleAssignments">The SPRoleAssignmentCollection object
		///		to manage.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole(
			SPRoleAssignmentCollection roleAssignments)
		{
			Init(roleAssignments);
		}

		#endregion Constructors

		#region Private Methods

		/// <summary>
		/// Gets an array of SPRoleDefinition objects specified by the names.
		/// </summary>
		/// <param name="roleDefinitionNames">The names of the
		///		SPRoleDefinition objects.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		private SPRoleDefinition[] GetRoleDefinitions(
			params string[] roleDefinitionNames)
		{
			var rds = new List<SPRoleDefinition>();

			foreach (string rd in roleDefinitionNames)
				rds.Add(ParentWeb.RoleDefinitions[rd]);

			return rds.ToArray();
		}

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		private void Init(SPRoleAssignmentCollection roleAssignments)
		{
			RoleAssignments = roleAssignments;

			var parent = roleAssignments.Parent;

			ParentWeb = parent as SPWeb;

			if (ParentWeb != null)
				return;

			var parentList = parent as SPList;
			if (parentList != null)
			{
				ParentWeb = parentList.ParentWeb;
				return;
			}

			var parentItem = parent as SPListItem;
			if (parentItem != null)
			{
				ParentWeb = parentItem.Web;
				return;
			}

			throw new ArgumentException(
				"Only SPRoleAssignmentCollection object which is a member " +
				"of SPWeb, SPList, or SPListItem is allowed.");
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Adds one or more permissions to the specified principal.
		/// </summary>
		/// <param name="principal">The principal to be given the
		///		permissions.</param>
		/// <param name="roleDefinitions">The list of role definitions
		///		having the permissions.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole AddPermissions(SPPrincipal principal,
			params SPRoleDefinition[] roleDefinitions)
		{
			SPRoleAssignment ra = null;

			try
			{
				ra = RoleAssignments.GetAssignmentByPrincipal(principal);
			}
			catch (ArgumentOutOfRangeException)
			{
				// Could not find the SPPrincipal object.
			}
			catch (ArgumentException)
			{
				/// The SPPrincipal object resides within a group and the
				/// ISecurableObject type is SPWeb.
			}

			if (ra == null)
			{
				ra = new SPRoleAssignment(principal);

				foreach (SPRoleDefinition rd in roleDefinitions)
				{
					ra.RoleDefinitionBindings.Add(rd);
				}

				RoleAssignments.Add(ra);
			}
			else
			{
				foreach (SPRoleDefinition rd in roleDefinitions)
				{
					if (!ra.RoleDefinitionBindings.Contains(rd))
						ra.RoleDefinitionBindings.Add(rd);
				}

				ra.Update();
			}

			return this;
		}

		/// <summary>
		/// Adds one or more permissions to the specified principal.
		/// </summary>
		/// <param name="principal">The principal to be given the
		///		permissions.</param>
		/// <param name="roleDefinitionNames">The list of role definitions
		///		names having the permissions.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole AddPermissions(SPPrincipal principal,
			params string[] roleDefinitionNames)
		{
			return AddPermissions(principal,
				GetRoleDefinitions(roleDefinitionNames));
		}

		/// <summary>
		/// Breaks the role inheritance from the parent object.
		/// </summary>
		/// <param name="copyRoleAssignments">If true, copy the role
		///		assignments of the parent object.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole BreakRoleInheritance(
			bool copyRoleAssignments)
		{
			var parent = RoleAssignments.Parent;

			if (parent.HasUniqueRoleAssignments)
				return this;

			parent.BreakRoleInheritance(copyRoleAssignments);
			Init(parent.RoleAssignments);
			return this;
		}

		/// <summary>
		/// Removes all permissions.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole RemovePermissions()
		{
			var count = RoleAssignments.Count;

			for (int i = 0; i < count; i++)
				RoleAssignments.Remove(0);

			return this;
		}

		/// <summary>
		/// Removes permissions of the specified principal.
		/// </summary>
		/// <param name="principal">The principal to delete the permissions
		///		from.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole RemovePermissions(SPPrincipal principal)
		{
			RoleAssignments.Remove(principal);
			return this;
		}

		/// <summary>
		/// Removes a set of permissions of the specified principal.
		/// </summary>
		/// <param name="principal">The principal to delete the permissions
		///		from.</param>
		///	<param name="roleDefinitions">The list of role definitions having
		///		the permissions to delete.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole RemovePermissions(SPPrincipal principal,
			params SPRoleDefinition[] roleDefinitions)
		{
			SPRoleAssignment ra = null;

			try
			{
				ra = RoleAssignments.GetAssignmentByPrincipal(principal);
			}
			catch (ArgumentOutOfRangeException)
			{
				// Could not find the SPPrincipal object.
			}
			catch (ArgumentException)
			{
				/// The SPPrincipal object resides within a group and the
				/// ISecurableObject type is SPWeb.
			}

			if (ra == null)
				return this;

			var bindings = ra.RoleDefinitionBindings;

			foreach (SPRoleDefinition rd in roleDefinitions)
				bindings.Remove(rd);

			ra.Update();

			return this;
		}

		/// <summary>
		/// Removes a set of permissions of the specified principal.
		/// </summary>
		/// <param name="principal">The principal to delete the permissions
		///		from.</param>
		///	<param name="roleDefinitionNames">The list of role definitions
		///		names having the permissions to delete.</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole RemovePermissions(SPPrincipal principal,
			params string[] roleDefinitionNames)
		{
			return RemovePermissions(principal,
				GetRoleDefinitions(roleDefinitionNames));
		}

		/// <summary>
		/// Updates all permissions changes.
		/// </summary>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgRole Update()
		{
			var parent = RoleAssignments.Parent;

			var web = parent as SPWeb;
			if (web != null)
			{
				web.Update();
				return this;
			}

			var list = parent as SPList;
			if (list != null)
			{
				list.Update();
				return this;
			}

			var item = parent as SPListItem;
			if (item != null)
			{
				item.SystemUpdate(false);
				return this;
			}

			return this;
		}

		#endregion Public Methods

		#region Public Static Methods

		/// <summary>
		/// Checks if a list of principals contains a specific user.
		/// </summary>
		/// <param name="principals">The list of principals to check from.</param>
		/// <param name="user">The user to search.</param>
		/// <returns>True if the list contains the user, false otherwise.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1800:DoNotCastUnnecessarily"),
		SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static bool DoesPrincipalsContainUser(IList<SPPrincipal> principals,
			SPUser user)
		{
			foreach (SPPrincipal principal in principals)
			{
				if (principal is SPUser)
				{
					if (((SPUser)principal).LoginName.Equals(user.LoginName,
						StringComparison.CurrentCultureIgnoreCase))
						return true;
				}
				else if (principal is SPGroup)
				{
					string groupName = ((SPGroup)principal).Name;

					foreach (SPGroup userGroup in user.Groups)
					{
						if (userGroup.Name.Equals(groupName))
							return true;
					}
				}
			}

			return false;
		}

		#endregion Public Static Methods
	}
}