using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace LiquidSilver
{
	/// <summary>
	/// Manages the EventReceivers collection.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Naming",
		"CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hg")]
	public class HgEventReceiver
	{
		#region Properties

		/// <summary>
		/// Gets the EventReceivers collection which is being managed.
		/// </summary>
		public SPEventReceiverDefinitionCollection EventReceivers { get; private set; }

		/// <summary>
		/// Gets or sets the name of any new event receiver,
		/// the default value is empty.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the sequence number of any new event receiver,
		/// the default value is 10000.
		/// </summary>
		public int SequenceNumber { get; set; }

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Instantiates a new SPEventReceiverManager object.
		/// </summary>
		/// <param name="eventReceivers">The EventReceivers collection to manage.</param>
		public HgEventReceiver(SPEventReceiverDefinitionCollection eventReceivers)
		{
			EventReceivers = eventReceivers;
			Name = "";
			SequenceNumber = 10000;
		}

		#endregion Constructors

		#region Private Methods

		private bool DoesEventReceiverExist(SPEventReceiverDefinition eventReceiver)
		{
			foreach (SPEventReceiverDefinition erd in EventReceivers)
			{
				if (erd.Assembly.Equals(eventReceiver.Assembly)
					&& erd.Class.Equals(eventReceiver.Class)
					&& erd.Type == eventReceiver.Type)
					return true;
			}

			return false;
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Adds an event receiver to the EventReceivers collection.
		/// It will not add a duplicate event receiver.
		/// </summary>
		/// <param name="receiverType">The event receiver type</param>
		/// <param name="assembly">The full assembly name and version</param>
		/// <param name="className">The full class name including namespace</param>

		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgEventReceiver Add(SPEventReceiverType receiverType,
			string assembly, string className)
		{
			SPEventReceiverDefinition erd = EventReceivers.Add();
			erd.Type = receiverType;
			erd.Assembly = assembly;
			erd.Class = className;
			erd.Name = this.Name;
			erd.SequenceNumber = this.SequenceNumber;

			if (!DoesEventReceiverExist(erd))
				erd.Update();

			return this;
		}

		/// <summary>
		/// Deletes event receivers from the EventReceivers collection that
		/// match the specified type, assembly, and class name.</summary>
		/// <param name="receiverType">The event receiver type</param>
		/// <param name="assembly">The full assembly name and version</param>
		/// <param name="className">The full class name including namespace</param>
		[SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
		public HgEventReceiver Delete(SPEventReceiverType receiverType,
			string assembly, string className)
		{
			for (int i = EventReceivers.Count - 1; i >= 0; i--)
			{
				SPEventReceiverDefinition erd = EventReceivers[i];

				if (erd.Assembly.Equals(assembly)
					&& erd.Class.Equals(className)
					&& erd.Type == receiverType)
					erd.Delete();
			}

			return this;
		}

		#endregion Public Methods
	}
}
