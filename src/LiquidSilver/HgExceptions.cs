using System;
using System.Runtime.Serialization;

namespace LiquidSilver
{
	/// <summary>
	/// The exception that is thrown when an attempt to create a new file fails
	/// because another file with the same name already exists.
	/// </summary>
	[Serializable]
	public class HgFileExistsException : Exception
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the
		///		<see cref="HgFileExistsException"/> class.
		/// </summary>
		public HgFileExistsException() : base() { }

		/// <summary>
		/// Initializes a new instance of the
		///		<see cref="HgFileExistsException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason
		///		for the exception.</param>
		public HgFileExistsException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the
		///		<see cref="HgFileExistsException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason
		///		for the exception.</param>
		/// <param name="innerException">The exception that is the cause of
		///		the current exception.</param>
		public HgFileExistsException(string message, Exception innerException)
			: base(message, innerException) { }

		protected HgFileExistsException(SerializationInfo info,
			StreamingContext context)
			: base(info, context) { }

		#endregion Constructors
	}
}