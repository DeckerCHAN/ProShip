/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;
using System.Globalization;

namespace LibProShip.Infrastructure.Unpickling.Objects
{

/// <summary>
/// This object constructor uses reflection to create instances of the decimal type.
/// (AnyClassConstructor cannot be used because decimal doesn't have the appropriate constructors).
/// </summary>
public class DecimalConstructor : IObjectConstructor
{
	public dynamic construct(dynamic[] args)
	{
		if(args.Length==1 && args[0] is string) {
			return Convert.ToDecimal((string)args[0], CultureInfo.InvariantCulture);
		}

		throw new PickleException("invalid arguments for decimal constructor");
	}
}

}
