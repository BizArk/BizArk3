
namespace BizArk.Core
{
    /// <summary>
    /// Provides a tri-state boolean to allow something else to determine the value.
    /// </summary>
    public enum DefaultBoolean
    {
        /// <summary>Parent object determines value.</summary>
        Default,
        /// <summary>True</summary>
        True,
        /// <summary>False</summary>
        False
    }
}
