/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	/// Exception thrown when something went wrong with pickling or unpickling.
	/// </summary>
	public class PickleException : Exception
	{
		public PickleException(string message) : base(message)
		{
		}

		public PickleException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
