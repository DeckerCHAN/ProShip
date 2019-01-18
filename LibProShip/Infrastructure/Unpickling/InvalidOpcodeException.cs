/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Exception thrown when the unpickler encountered an unknown or unimplemented opcode.
	/// </summary>
	public class InvalidOpcodeException : PickleException
    {
        public InvalidOpcodeException(string message) : base(message)
        {
        }
    }
}