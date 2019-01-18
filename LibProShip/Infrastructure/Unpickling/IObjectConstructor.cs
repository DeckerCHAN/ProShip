/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

// ReSharper disable InconsistentNaming

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Interface for object Constructors that are used by the unpickler
	///     to create instances of non-primitive or custom classes.
	/// </summary>
	public interface IObjectConstructor
    {
        /**
         * Create an object. Use the given args as parameters for the constructor.
         */
        dynamic construct(dynamic[] args);
    }
}