using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TourFactory.Core.Extensions.StreamExt
{

    /// <summary>
    /// Provides utility methods for handling streams.
    /// </summary>
    public static class StreamExt
    {

        /// <summary>
        /// Copies the stream to another stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
		/// <param name="bufferSize">Size of the buffer to use for copying the stream.</param>
        public static void Write(this Stream output, Stream input, int bufferSize = 4096)
        {
            byte[] b = new byte[bufferSize];
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);
        }

    }

}
