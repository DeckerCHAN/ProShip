/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System.Collections;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace LibProShip.Infrastructure.Unpickling.Objects
{
	/// <summary>
	///     This object constructor creates ClassDicts (for unsupported classes)
	/// </summary>
	public class ClassDictConstructor : IObjectConstructor
    {
        public readonly string module;
        public readonly string name;

        public ClassDictConstructor(string module, string name)
        {
            this.module = module;
            this.name = name;
        }

        public object construct(object[] args)
        {
            if (args.Length > 0)
                throw new PickleException("expected zero arguments for construction of ClassDict (for " + module + "." +
                                          name + ")");
            return new ClassDict(module, name);
        }
    }

	/// <summary>
	///     A dictionary containing just the fields of the class.
	/// </summary>
	public class ClassDict : Dictionary<string, object>
    {
        public ClassDict(string modulename, string classname)
        {
            if (string.IsNullOrEmpty(modulename))
                ClassName = classname;
            else
                ClassName = modulename + "." + classname;

            Add("__class__", ClassName);
        }

        /// <summary>
        ///     retrieve the (python) class name of the object that was pickled.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        ///     for the unpickler to restore state
        /// </summary>
        public void __setstate__(Hashtable values)
        {
            Clear();
            Add("__class__", ClassName);
            foreach (string x in values.Keys)
                Add(x, values[x]);
        }
    }
}