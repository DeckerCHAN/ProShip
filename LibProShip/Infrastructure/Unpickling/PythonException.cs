/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Exception thrown that represents a certain Python exception.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PythonException : Exception
    {
        public PythonException(string message) : base(message)
        {
        }

        // special constructor for UnicodeDecodeError
        // ReSharper disable once UnusedMember.Global
        public PythonException(string encoding, byte[] data, int i1, int i2, string message)
            : base("UnicodeDecodeError: " + encoding + ": " + message)
        {
        }

        public string _pyroTraceback { get; set; }
        public string PythonExceptionType { get; set; }

        /// <summary>
        ///     for the unpickler to restore state
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void __setstate__(Hashtable values)
        {
            if (!values.ContainsKey("_pyroTraceback"))
                return;
            var tb = values["_pyroTraceback"];
            // if the traceback is a list of strings, create one string from it
            var tbcoll = tb as ICollection;
            if (tbcoll != null)
            {
                var sb = new StringBuilder();
                foreach (var line in tbcoll) sb.Append(line);
                _pyroTraceback = sb.ToString();
            }
            else
            {
                _pyroTraceback = (string) tb;
            }

            //Console.WriteLine("pythonexception state set to:{0}",_pyroTraceback);
        }
    }
}