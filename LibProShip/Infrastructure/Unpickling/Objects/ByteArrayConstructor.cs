/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;
using System.Collections;
using System.Text;

namespace LibProShip.Infrastructure.Unpickling.Objects
{
	/// <summary>
	///     Creates byte arrays (byte[]).
	/// </summary>
	public class ByteArrayConstructor : IObjectConstructor
    {
        public dynamic construct(dynamic[] args)
        {
            // args for bytearray constructor: [ String string, String encoding ]
            // args for bytearray constructor (from python3 bytes): [ ArrayList ] or just [byte[]] (when it uses BINBYTES opcode)
            if (args.Length != 1 && args.Length != 2)
                throw new PickleException("invalid pickle data for bytearray; expected 1 or 2 args, got " +
                                          args.Length);

            if (args.Length == 1)
            {
                if (args[0] is byte[]) return args[0];
                var values = (ArrayList) args[0];
                var data = new byte[values.Count];
                for (var i = 0; i < data.Length; ++i) data[i] = Convert.ToByte(values[i]);
                return data;
            }
            else
            {
                // This thing is fooling around with byte<>string mappings using an encoding.
                // I think that is fishy... but for now it seems what Python itself is also doing...
                var data = (string) args[0];
                var encoding = (string) args[1];
                if (encoding.StartsWith("latin-"))
                    encoding = "ISO-8859-" + encoding.Substring(6);
                return Encoding.GetEncoding(encoding).GetBytes(data);
            }
        }
    }
}