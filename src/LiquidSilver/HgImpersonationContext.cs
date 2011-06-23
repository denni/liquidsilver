using System;
using System.Security.Principal;

namespace LiquidSilver
{
	/// <summary>
	/// Impersonates the application pool account.
	/// </summary>
	/// <example>
	/// using (var context = new HgImpersonationContext())
	/// {
	///		// Add code to run under the application pool account context.
	/// }
	/// </example>
	public class HgImpersonationContext : DisposableBase, IDisposable
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="HgImpersonationContext"/>.
		/// </summary>
		public HgImpersonationContext()
		{
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			if (identity == null || !identity.IsSystem)
			{
				this.context = WindowsIdentity.Impersonate(IntPtr.Zero);
			}
		}

		#endregion Constructors

		#region Private Fields

		private WindowsImpersonationContext context;

		#endregion Private Fields

		#region DisposableBase Members

		protected override void DisposeManagedResources()
		{
			if (this.context != null)
			{
				this.context.Undo();
				this.context.Dispose();
				this.context = null;
			}

			base.DisposeManagedResources();
		}

		#endregion DisposableBase Members

		#region Delegates

		/// <summary>
		/// Executes the passed code under the application pool account context.
		/// </summary>
		/// <param name="code">The code to execute.</param>
		public static void Execute(Action code)
		{
			using (var context = new HgImpersonationContext())
			{
				code();
			}
		}

		#endregion Delegates
	}
}
