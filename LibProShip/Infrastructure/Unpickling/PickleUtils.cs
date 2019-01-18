/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;
using System.IO;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Utility stuff for dealing with pickle data streams.
	/// </summary>
	public static class PickleUtils
    {
        /**
         * read a line of text, possibly including the terminating LF char
         */
        public static string readline(Stream input, bool includeLF = false)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var c = input.ReadByte();
                if (c == -1)
                {
                    if (sb.Length == 0)
                        throw new IOException("premature end of file");
                    break;
                }

                if (c != '\n' || includeLF)
                    sb.Append((char) c);
                if (c == '\n')
                    break;
            }

            return sb.ToString();
        }

        /**
         * read a single unsigned byte
         */
        public static byte readbyte(Stream input)
        {
            var b = input.ReadByte();
            if (b < 0) throw new IOException("premature end of input stream");
            return (byte) b;
        }

        /**
         * read a number of signed bytes
         */
        public static byte[] readbytes(Stream input, int n)
        {
            var buffer = new byte[n];
            readbytes_into(input, buffer, 0, n);
            return buffer;
        }

        /**
         * read a number of signed bytes
         */
        public static byte[] readbytes(Stream input, long n)
        {
            try
            {
                var small_n = Convert.ToInt32(n);
                return readbytes(input, small_n);
            }
            catch (OverflowException x)
            {
                throw new PickleException("pickle too large, can't read more than maxint", x);
            }
        }

        /**
         * read a number of signed bytes into the specified location in an existing byte array
         */
        public static void readbytes_into(Stream input, byte[] buffer, int offset, int length)
        {
            while (length > 0)
            {
                var read = input.Read(buffer, offset, length);
                if (read <= 0)
                    throw new IOException("read error; expected more bytes");
                offset += read;
                length -= read;
            }
        }

        /**
         * Convert a couple of bytes into the corresponding integer number.
         * Can deal with 2-bytes unsigned int and 4-bytes signed int.
         */
        public static int bytes_to_integer(byte[] bytes)
        {
            return bytes_to_integer(bytes, 0, bytes.Length);
        }

        public static int bytes_to_integer(byte[] bytes, int offset, int size)
        {
            // this operates on little-endian bytes

            switch (size)
            {
                case 2:
                    // 2-bytes unsigned int
                    if (BitConverter.IsLittleEndian) return BitConverter.ToUInt16(bytes, offset);
                    // need to byteswap because the converter needs big-endian...
                    return BitConverter.ToUInt16(new[] {bytes[1 + offset], bytes[0 + offset]}, 0);
                case 4:
                    // 4-bytes signed int
                    if (BitConverter.IsLittleEndian) return BitConverter.ToInt32(bytes, offset);
                    // need to byteswap because the converter needs big-endian...
                    return BitConverter.ToInt32(
                        new[] {bytes[3 + offset], bytes[2 + offset], bytes[1 + offset], bytes[0 + offset]}, 0);
                default:
                    throw new PickleException("invalid amount of bytes to convert to int: " + size);
            }
        }

        /**
         * Convert 8 little endian bytes into a long
         */
        public static long bytes_to_long(byte[] bytes, int offset)
        {
            if (bytes.Length - offset < 8)
                throw new PickleException("too few bytes to convert to long");
            if (BitConverter.IsLittleEndian) return BitConverter.ToInt64(bytes, offset);
            // need to byteswap because the converter needs big-endian...
            byte[] bigendian =
            {
                bytes[7 + offset], bytes[6 + offset], bytes[5 + offset], bytes[4 + offset], bytes[3 + offset],
                bytes[2 + offset], bytes[1 + offset], bytes[0 + offset]
            };
            return BitConverter.ToInt64(bigendian, 0);
        }

        /**
         * Convert 4 little endian bytes into an unsigned int
         */
        public static uint bytes_to_uint(byte[] bytes, int offset)
        {
            if (bytes.Length - offset < 4)
                throw new PickleException("too few bytes to convert to long");
            if (BitConverter.IsLittleEndian) return BitConverter.ToUInt32(bytes, offset);
            // need to byteswap because the converter needs big-endian...
            byte[] bigendian = {bytes[3 + offset], bytes[2 + offset], bytes[1 + offset], bytes[0 + offset]};
            return BitConverter.ToUInt32(bigendian, 0);
        }

        /**
         * Convert a signed integer to its 4-byte representation. (little endian)
         */
        public static byte[] integer_to_bytes(int i)
        {
            var bytes = BitConverter.GetBytes(i);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /**
         * Convert a double to its 8-byte representation (big endian).
         */
        public static byte[] double_to_bytes_bigendian(double d)
        {
            var bytes = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /**
         * Convert a big endian 8-byte to a double. 
         */
        public static double bytes_bigendian_to_double(byte[] bytes, int offset)
        {
            if (bytes.Length - offset < 8) throw new PickleException("decoding double: too few bytes");
            if (BitConverter.IsLittleEndian)
            {
                // reverse the bytes to make them littleendian for the bitconverter
                byte[] littleendian =
                {
                    bytes[7 + offset], bytes[6 + offset], bytes[5 + offset], bytes[4 + offset], bytes[3 + offset],
                    bytes[2 + offset], bytes[1 + offset], bytes[0 + offset]
                };
                return BitConverter.ToDouble(littleendian, 0);
            }

            return BitConverter.ToDouble(bytes, offset);
        }

        /**
         * Convert a big endian 4-byte to a float. 
         */
        public static float bytes_bigendian_to_float(byte[] bytes, int offset)
        {
            if (bytes.Length - offset < 4) throw new PickleException("decoding float: too few bytes");
            if (BitConverter.IsLittleEndian)
            {
                // reverse the bytes to make them littleendian for the bitconverter
                byte[] littleendian = {bytes[3 + offset], bytes[2 + offset], bytes[1 + offset], bytes[0 + offset]};
                return BitConverter.ToSingle(littleendian, 0);
            }

            return BitConverter.ToSingle(bytes, offset);
        }

        /**
         * read an arbitrary 'long' number. 
         * because c# doesn't have a bigint, we look if stuff fits into a regular long,
         * and raise an exception if it's bigger.
         */
        public static long decode_long(byte[] data)
        {
            if (data.Length == 0)
                return 0L;
            if (data.Length > 8)
                throw new PickleException("value to large for long, biginteger needed");
            if (data.Length < 8)
            {
                // bitconverter requires exactly 8 bytes so we need to extend it
                var larger = new byte[8];
                Array.Copy(data, larger, data.Length);

                // check if we need to sign-extend (if the original number was negative)
                if ((data[data.Length - 1] & 0x80) == 0x80)
                    for (var i = data.Length; i < 8; ++i)
                        larger[i] = 0xff;
                data = larger;
            }

            if (!BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        /**
         * Construct a string from the given bytes where these are directly
         * converted to the corresponding chars, without using a given character
         * encoding
         */
        public static string rawStringFromBytes(byte[] data)
        {
            var str = new StringBuilder(data.Length);
            foreach (var b in data) str.Append((char) b);
            return str.ToString();
        }

        /**
         * Convert a string to a byte array, no encoding is used. String must only contain characters <256.
         */
        public static byte[] str2bytes(string str)
        {
            var b = new byte[str.Length];
            for (var i = 0; i < str.Length; ++i)
            {
                var c = str[i];
                if (c > 255) throw new ArgumentException("string contained a char > 255, cannot convert to bytes");
                b[i] = (byte) c;
            }

            return b;
        }

        /**
         * Decode a string with possible escaped char sequences in it (\x??).
         */
        public static string decode_escaped(string str)
        {
            if (str.IndexOf('\\') == -1)
                return str;
            var sb = new StringBuilder(str.Length);
            for (var i = 0; i < str.Length; ++i)
            {
                var c = str[i];
                if (c == '\\')
                {
                    // possible escape sequence
                    var c2 = str[++i];
                    switch (c2)
                    {
                        case '\\':
                            // double-escaped '\\'--> '\'
                            sb.Append(c);
                            break;
                        case 'x':
                            // hex escaped "\x??"
                            var h1 = str[++i];
                            var h2 = str[++i];
                            c2 = (char) Convert.ToInt32("" + h1 + h2, 16);
                            sb.Append(c2);
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case '\'':
                            sb.Append('\''); // sometimes occurs in protocol level 0 strings
                            break;
                        default:
                            if (str.Length > 80)
                                str = str.Substring(0, 80);
                            throw new PickleException("invalid escape sequence char \'" + c2 + "\' in string \"" + str +
                                                      " [...]\" (possibly truncated)");
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }

        /**
         * Decode a string with possible escaped unicode in it (\u20ac)
         */
        public static string decode_unicode_escaped(string str)
        {
            if (str.IndexOf('\\') == -1)
                return str;
            var sb = new StringBuilder(str.Length);
            for (var i = 0; i < str.Length; ++i)
            {
                var c = str[i];
                if (c == '\\')
                {
                    // possible escape sequence
                    var c2 = str[++i];
                    switch (c2)
                    {
                        case '\\':
                            // double-escaped '\\'--> '\'
                            sb.Append(c);
                            break;
                        case 'u':
                            // hex escaped unicode "\u20ac"
                            var h1 = str[++i];
                            var h2 = str[++i];
                            var h3 = str[++i];
                            var h4 = str[++i];
                            c2 = (char) Convert.ToInt32("" + h1 + h2 + h3 + h4, 16);
                            sb.Append(c2);
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        default:
                            if (str.Length > 80)
                                str = str.Substring(0, 80);
                            throw new PickleException("invalid escape sequence char \'" + c2 + "\' in string \"" + str +
                                                      " [...]\" (possibly truncated)");
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }
    }
}