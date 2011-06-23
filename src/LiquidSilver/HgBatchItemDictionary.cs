using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LiquidSilver
{
	/// <summary>
	/// A dictionary of item's fields to be updated using the
	/// <see cref="HgList"/>.<see cref="BatchUpdate()"/> method.
	/// </summary>
	[Serializable]
	public class HgBatchItemDictionary : Dictionary<string, string>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="HgBatchItemDictionary"/> class.
		/// </summary>
		/// <param name="itemId">The ID of the associated item.</param>
		public HgBatchItemDictionary(int itemId)
		{
			ItemId = itemId;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the item's ID.
		/// </summary>
		public int ItemId { get; private set; }

		#endregion Properties

		#region ISerializable Members

		protected HgBatchItemDictionary(SerializationInfo info,
			StreamingContext context)
			: base(info, context) { }

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info,
			StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		#endregion ISerializable Members
	}
}