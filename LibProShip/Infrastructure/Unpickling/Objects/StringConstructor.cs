/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

// ReSharper disable UnusedMember.Global
namespace LibProShip.Infrastructure.Unpickling.Objects
{

/// <summary>
/// This object constructor uses reflection to create instances of the string type.
/// AnyClassConstructor cannot be used because string doesn't have the appropriate constructors.
///	see http://stackoverflow.com/questions/2092530/how-do-i-use-activator-createinstance-with-strings
/// </summary>
public class StringConstructor : IObjectConstructor
{
	public dynamic construct(dynamic[] args)
	{
		if(args.Length==0) {
			return "";
		}

		if(args.Length==1 && args[0] is string) {
			return (string)args[0];
		}

		throw new PickleException("invalid string constructor arguments");
	}
}

}
