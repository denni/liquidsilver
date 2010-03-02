using System;
using System.Globalization;
using System.Reflection;

namespace LiquidSilver
{
	/// <summary>
	/// Provides a base for creating Singletons. To derive from this, the
	/// singleton sub classes must have a private constructor.
	/// </summary>
	/// <typeparam name="T">The class type to be defined as a singleton.</typeparam>
	public abstract class SingletonBase<T> where T : class
	{
		#region Constructors

		/// <summary>
		/// A protected constructor which is accessible only to the sub classes.
		/// </summary>
		protected SingletonBase() { }

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static T Instance
		{
			get { return SingletonFactory.Instance; }
		}

		#endregion Properties

		#region Inner Classes

		/// <summary>
		/// The singleton class factory to create the singleton instance.
		/// </summary>
		class SingletonFactory
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
				"CA1810:InitializeReferenceTypeStaticFieldsInline")]
			static SingletonFactory() { }

			// Prevent the compiler from generating a default constructor.
			SingletonFactory() { }

			internal static readonly T Instance = GetInstance();

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
				"CA2001:AvoidCallingProblematicMethods", MessageId = "System.Type.InvokeMember")]
			static T GetInstance()
			{
				var theType = typeof(T);

				T inst;

				try
				{
					inst = (T)theType
						.InvokeMember(theType.Name,
							BindingFlags.CreateInstance | BindingFlags.Instance
							| BindingFlags.NonPublic,
							null, null, null,
							CultureInfo.InvariantCulture);
				}
				catch (MissingMethodException ex)
				{
					throw new TypeLoadException(string.Format(
						CultureInfo.CurrentCulture,
						"The type '{0}' must have a private constructor to " +
						"be used in the Singleton pattern.", theType.FullName)
						, ex);
				}

				return inst;
			}
		}

		#endregion Inner Classes
	}
}