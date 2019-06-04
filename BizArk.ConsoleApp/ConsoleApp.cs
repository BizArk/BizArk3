using System;
using System.ComponentModel;
using System.Configuration;

namespace BizArk.ConsoleApp
{

    /// <summary>
    /// Interface used by BaCon to manage a conole app.
    /// </summary>
    public interface IConsoleApp
    {
        /// <summary>
        /// The method to call to start running the console application.
        /// </summary>
        /// <returns>Environment.ExitCode</returns>
        int Start();

        /// <summary>
        /// Gets a value that determines if the process should pause before exiting.
        /// </summary>
        bool Wait { get; }

        /// <summary>
        /// Gets a value that determines if help text should be displayed instead of running the console app.
        /// </summary>
        bool Help { get; }

        /// <summary>
        /// Called if an exception is raised. Return true to indicate the error is handled. If handled, the BaCon object won't display help or set the ExitCode.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool Error(Exception ex);
    }

    /// <summary>
    /// A base class that can be used so developers don't have to implement all the methods.
    /// </summary>
    public abstract class BaseConsoleApp : IConsoleApp, IStringIndexedObject
    {
        private PropertyDescriptorCollection LogProps;

        /// <summary>
        /// The method to call to start running the console application.
        /// </summary>
        /// <returns>Environment.ExitCode</returns>
        public abstract int Start();

        /// <summary>
        /// Gets or sets a value that determines if help text should be displayed instead of running the console app.
        /// </summary>
        [CmdLineArg("?", ShowInUsage = false)]
        [Description("If true, displays the help text.")]
        public bool Help { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the process should pause before exiting.
        /// </summary>
        [CmdLineArg(ShowInUsage = false)]
        [Description("If true, waits for a key to be pressed before exiting the application.")]
        public bool Wait { get; set; }

        /// <summary>
        /// Called if an exception is raised. Return true to indicate the error is handled. If handled, the BaCon object won't display help or set the ExitCode.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual bool Error(Exception ex)
        {
            return false;
        }

        /// <summary>
        /// Default constructor will pull configurations from the traditional .NET Framework configuration provider.
        /// </summary>
        public BaseConsoleApp() : this(new NetFrameworkConfigProvider()) { }

        /// <summary>
        /// If an alternate provider is required (Net Core's ConfigurationBuilder, for example, or for unit testing) 
        /// this constructor can be used. 
        /// </summary>
        /// <param name="config">Configuration Provider to be used (</param>
        public BaseConsoleApp(IConfigurationProvider config)
        {
            LogProps = TypeDescriptor.GetProperties(this.GetType());
            foreach (var p in this.GetType().GetProperties())
            {
                foreach (var a in p.GetCustomAttributes(true))
                {
                    if (a is ConfigAttribute)
                    {
                        this[p.Name] = config.GetSetting(p.Name);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// BaseConsoleApp implements IStringIndexedObject, meaning it provides this[string i]
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Property value as a string or the Empty String if the value is not set</returns>
        public string this[string propertyName]
        {
            get { return LogProps[propertyName]?.GetValue(this)?.ToString() ?? ""; }
            set { LogProps[propertyName]?.SetValue(this, value); }
        }

        /// <summary>
        /// Component of the IStringIndexedObject interface that allows safely accessing the indexes
        /// </summary>
        /// <param name="name">Index to search for</param>
        /// <returns></returns>
        public bool ContainsKey(string name)
        {
            return LogProps.Find(name, false) != null;
        }
    }

    /// <summary>
    /// Indicates that the object implements this[] indexing using strings
    /// </summary>
    public interface IStringIndexedObject
    {
        /// <summary>
        /// Implement the C# indexing syntax
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string this[string name] { get; set; }

        /// <summary>
        /// Allows safe access to indexed values
        /// </summary>
        /// <param name="name">The index to search for</param>
        /// <returns></returns>
        bool ContainsKey(string name);
    }
}
