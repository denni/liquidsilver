using System;

namespace LiquidSilver
{
	/// <summary>
	/// An abstract base class that implements a proper Disposable pattern.
	/// </summary>
	public abstract class DisposableBase : IDisposable
	{
		#region Destructors

		/// <summary>
		/// Ensures that all disposable resources are released when this object
		/// is out of context.
		/// </summary>
		~DisposableBase()
		{
			Dispose(false);
		}

		#endregion Destructors

		#region Private Members

		private bool _disposed;

		/// <summary>
		/// Disposes all resources.
		/// </summary>
		/// <param name="disposing">If <c>true</c>, also dispose the managed
		///		resources, else only dispose the unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
				ManagedResourcesDisposing();

			UnmanagedResourcesDisposing();

			_disposed = true;
		}

		#endregion Private Members

		#region Protected Members

		/// <summary>
		/// Event that occurs before managed resources are disposed.
		/// </summary>
		protected virtual void ManagedResourcesDisposing() { }

		/// <summary>
		/// Event that occurs before unmanaged resources are disposed.
		/// </summary>
		protected virtual void UnmanagedResourcesDisposing() { }

		#endregion Protected Members

		#region Public Members

		/// <summary>
		/// Safely disposes an object; i.e., a null or already disposed object
		/// won't be disposed again.
		/// </summary>
		/// <typeparam name="T">The type of the object to dispose.</typeparam>
		/// <param name="obj">The object to dispose.</param>
		/// <returns><c>null</c></returns>
		/// <example>
		///		var site = new SPSite("http://siteurl/");
		///		// Do something with site
		///		site = SafeDispose(site); // disposes site and set it to null.
		/// </example>
		public virtual T SafeDispose<T>(T obj) where T : IDisposable
		{
			if (obj != null)
			{
				try
				{
					obj.Dispose();
				}
				catch (ObjectDisposedException) { }
			}

			return default(T);
		}

		#endregion Public Members

		#region IDisposable Members

		/// <summary>
		/// Disposes this object and releases all resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members
	}
}