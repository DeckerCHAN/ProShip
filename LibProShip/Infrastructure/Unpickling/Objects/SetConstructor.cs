/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System.Collections;
using System.Collections.Generic;

namespace LibProShip.Infrastructure.Unpickling.Objects
{

/// <summary>
/// This object constructor creates sets. (HashSet&lt;object&gt;)
/// </summary>
public class SetConstructor : IObjectConstructor {

	public dynamic construct(dynamic[] args) {
		// create a HashSet, args=arraylist of stuff to put in it
		ArrayList elements=(ArrayList)args[0];
		IEnumerable<dynamic> array=elements.ToArray();
		return new HashSet<dynamic>(array);
	}
}

}
