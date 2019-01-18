/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System.IO;

// ReSharper disable InconsistentNaming

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Interface for object Picklers used by the pickler, to pickle custom classes.
	/// </summary>
	public interface IObjectPickler
    {
        /**
         * Pickle an object.
         */
        void pickle(dynamic o, Stream outs, Pickler currentPickler);
    }
}