using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp
{
    /// <summary>
    /// Empty decoration that is used in the BaseConsoleApp to pull command line parameter defaults from a configuration file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigAttribute : Attribute
    {
    }

    /// <summary>
    /// .Net Framework configuration manager abstraction
    /// </summary>
    public class NetFrameworkConfigProvider: IConfigurationProvider
    {
        /// <summary>
        /// Retrieves a setting using the .Net Framework's ConfigurationManager class
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }

    /// <summary>
    /// An abstraction to allow building configurations using .Net Framework or .Net Standard (or in unit tests)
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Returns the setting provided by the given key.
        /// </summary>
        /// <param name="key">setting key to retrieve</param>
        /// <returns></returns>
        string GetSetting(string key);
    }
}
