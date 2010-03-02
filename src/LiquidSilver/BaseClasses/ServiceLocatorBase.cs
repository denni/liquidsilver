using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiquidSilver
{
	/// <summary>
	/// A base class for creating service locators. To derive from this, the
	/// service locator sub classes must have a private constructor.
	/// </summary>
	/// <typeparam name="T">The class type to be defined as a service locator.
	/// </typeparam>
	public abstract class ServiceLocatorBase<T> : SingletonBase<T>
		where T : class
	{
		#region Constructors

		/// <summary>
		/// A private constructor so the ServiceLocator cannot be instantiated
		/// by external classes.
		/// </summary>
		protected ServiceLocatorBase()
		{
			_serviceTypes = new Dictionary<Type, Type>();
			_instantiatedServices = new Dictionary<Type, object>();

			BuildServiceTypesMap();
		}

		#endregion Constructors

		#region Private Fields

		// a map between contracts -> concrete implementation classes
		private IDictionary<Type, Type> _serviceTypes;

		// a map containing references to concrete implementation already instantiated
		// (the service locator uses lazy instantiation).
		private IDictionary<Type, object> _instantiatedServices;

		#endregion Private Fields

		#region Protected Methods

		/// <summary>
		/// Adds a service type mapping.
		/// </summary>
		/// <typeparam name="TInterface">The interface type.</typeparam>
		/// <typeparam name="TImplementation">The implementation type.</typeparam>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter")]
		protected void AddTypeMap<TInterface, TImplementation>()
		{
			_serviceTypes.Add(typeof(TInterface), typeof(TImplementation));
		}

		#endregion Protected Methods

		#region Public Methods

		/// <summary>
		/// Gets an instantiated instance of the registered implementation
		/// of a requested service.
		/// </summary>
		/// <typeparam name="TInterface">The service type to get.</typeparam>
		/// <returns>An instance of the registered implementation of the
		/// service.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TInterface GetService<TInterface>()
		{
			return GetService<TInterface>(null);
		}

		/// <summary>
		/// Gets an instantiated instance of the registered implementation
		/// of a requested service.
		/// </summary>
		/// <typeparam name="TInterface">The service type to get.</typeparam>
		/// <returns>An instance of the registered implementation of the
		/// service.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TInterface GetService<TInterface>(params object[] parameters)
		{
			if (_instantiatedServices.ContainsKey(typeof(TInterface)))
			{
				return (TInterface)_instantiatedServices[typeof(TInterface)];
			}
			else
			{
				TInterface service = GetNewServiceInstance<TInterface>(parameters);

				// add the service to the ones that we have already instantiated
				_instantiatedServices.Add(typeof(TInterface), service);

				return service;
			}
		}

		/// <summary>
		/// Gets a new instance of the registered implementation of a
		/// requested service.
		/// </summary>
		/// <typeparam name="TInterface">The service type to get.</typeparam>
		/// <returns>A new instance of the registered implementation of the
		/// service.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TInterface GetNewServiceInstance<TInterface>()
		{
			return GetNewServiceInstance<TInterface>(null);
		}

		/// <summary>
		/// Gets a new instance of the registered implementation of a
		/// requested service.
		/// </summary>
		/// <typeparam name="TInterface">The service type to get.</typeparam>
		/// <returns>A new instance of the registered implementation of the
		/// service.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter")]
		public TInterface GetNewServiceInstance<TInterface>(params object[] parameters)
		{
			Type svcType;

			try
			{
				svcType = _serviceTypes[typeof(TInterface)];
			}
			catch (KeyNotFoundException ex)
			{
				throw new ArgumentException(
					"The type has not been registered: " + typeof(TInterface).FullName,
					ex);
			}

			ConstructorInfo constructor;

			if (parameters == null || parameters.Length == 0)
			{
				// use reflection to invoke the service
				constructor = svcType.GetConstructor(new Type[0]);

				if (constructor == null)
					throw new ArgumentException(
						"Cannot find a constructor that takes no parameters for type: "
						+ typeof(TInterface).FullName);
			}
			else
			{
				constructor = svcType.GetConstructor(
					parameters.Select(x => x.GetType()).ToArray());

				if (constructor == null)
					throw new ArgumentException(
						"Cannot find a constructor with the specified parameters for type: "
						+ typeof(TInterface).FullName);
			}

			return (TInterface)constructor.Invoke(parameters);
		}

		#endregion Public Methods

		#region Abstract Methods

		/// <summary>
		/// Builds the mapping of service types.
		/// </summary>
		protected abstract void BuildServiceTypesMap();

		#endregion Abstract Methods
	}
}
