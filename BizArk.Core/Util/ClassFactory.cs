using System;

namespace BizArk.Core
{
	/// <summary>
	/// The class factory for objects allows for objects to be changed at runtime.
	/// </summary>
	public static class ClassFactory
    {

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(params object[] args)
        {
            return (T)CreateObject(typeof(T), args);
        }

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateObject(Type type, params object[] args)
        {
            if (Factory != null)
                return Factory(type, args);
            else
                return Activator.CreateInstance(type, args);
        }

		/// <summary>
		/// Get or set the factory to use. Only a single factory can be used at a time.
		/// </summary>
		public static Func<Type, object[], object> Factory { get; set; }

	}
}
