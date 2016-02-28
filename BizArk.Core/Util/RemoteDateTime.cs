using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BizArk.Core.Util
{

    /// <summary>
    /// Provides a convenient DateTime wrapper for handling remote date/time values (eg, the server date/time).
    /// </summary>
    public class RemoteDateTime
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of RemoteDateTime.
        /// </summary>
        /// <param name="start"></param>
        public RemoteDateTime(DateTime start)
        {
            Start = start;
            mTimer.Start();
        }

        #endregion

        #region Fields and Properties

        private Stopwatch mTimer = new Stopwatch();

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// Gets the amount of time that has elapsed since this was started.
        /// </summary>
        public TimeSpan Elapsed
        {
            get { return mTimer.Elapsed; }
        }

        /// <summary>
        /// Gets the value that represents the current DateTime on the remote machine.
        /// </summary>
        public DateTime Now
        {
            get { return Start.Add(mTimer.Elapsed); }
        }

        #endregion

    }
}
