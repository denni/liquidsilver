using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Provides various utility functions.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public static class HgUtility
	{
		/// <summary>
		/// Checks whether a collection of principals contains a specified user.
		/// </summary>
		/// <param name="principals">The collection of principals to search.</param>
		/// <param name="user">The user to search.</param>
		/// <returns><code>true</code> if the user was found in the collection
		///		of principals; otherwise, <code>false</code>.</returns>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public static bool DoesPrincipalsContainUser(
			IEnumerable<SPPrincipal> principals, SPUser user)
		{
			foreach (var principal in principals)
			{
				var usr = principal as SPUser;

				if (usr != null)
				{
					if (usr.LoginName.Equals(user.LoginName,
						StringComparison.CurrentCultureIgnoreCase))
						return true;
					else
						continue;
				}

				var grp = principal as SPGroup;

				if (grp != null)
				{
					var groupName = grp.Name;

					foreach (SPGroup group in user.Groups)
					{
						if (group.Name.Equals(groupName))
							return true;
					}
				}
			}

			return false;
		}
	}
}