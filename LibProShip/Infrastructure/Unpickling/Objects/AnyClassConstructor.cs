/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;

namespace LibProShip.Infrastructure.Unpickling.Objects
{

/// <summary>
/// This object constructor uses reflection to create instances of any given class. 
/// </summary>
public class AnyClassConstructor : IObjectConstructor {

	private readonly Type _type;

	public AnyClassConstructor(Type type) {
		_type = type;
	}

	public dynamic construct(dynamic[] args) {
		try {
			return Activator.CreateInstance(_type, args);
		} catch (Exception x) {
			throw new PickleException("problem constructing object",x);
		}
	}
}

}
