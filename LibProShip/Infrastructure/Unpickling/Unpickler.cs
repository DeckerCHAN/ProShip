/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using LibProShip.Infrastructure.Unpickling.Objects;

namespace LibProShip.Infrastructure.Unpickling
{
	/// <summary>
	///     Unpickles an object graph from a pickle data inputstream. Supports all pickle protocol versions.
	///     Maps the python objects on the corresponding java equivalents or similar types.
	///     This class is NOT threadsafe! (Don't use the same unpickler from different threads)
	///     See the README.txt for a table with the type mappings.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InvertIf")]
    public class Unpickler : IDisposable
    {
        protected const int HIGHEST_PROTOCOL = 4;
        protected static readonly IDictionary<string, IObjectConstructor> objectConstructors;
        protected static readonly dynamic NO_RETURN_VALUE = new object();

        protected readonly IDictionary<int, dynamic> memo;
        protected Stream input;
        protected UnpickleStack stack;

        static Unpickler()
        {
            objectConstructors = new Dictionary<string, IObjectConstructor>
            {
                ["__builtin__.complex"] = new AnyClassConstructor(typeof(ComplexNumber)),
                ["builtins.complex"] = new AnyClassConstructor(typeof(ComplexNumber)),
                ["array.array"] = new ArrayConstructor(),
                ["array._array_reconstructor"] = new ArrayConstructor(),
                ["__builtin__.bytearray"] = new ByteArrayConstructor(),
                ["builtins.bytearray"] = new ByteArrayConstructor(),
                ["__builtin__.bytes"] = new ByteArrayConstructor(),
                ["__builtin__.set"] = new SetConstructor(),
                ["builtins.set"] = new SetConstructor(),
                ["datetime.datetime"] = new DateTimeConstructor(DateTimeConstructor.PythonType.DateTime),
                ["datetime.time"] = new DateTimeConstructor(DateTimeConstructor.PythonType.Time),
                ["datetime.date"] = new DateTimeConstructor(DateTimeConstructor.PythonType.Date),
                ["datetime.timedelta"] = new DateTimeConstructor(DateTimeConstructor.PythonType.TimeDelta),
                ["decimal.Decimal"] = new DecimalConstructor(),
                ["_codecs.encode"] = new ByteArrayConstructor()
            };
            // we're lucky, the bytearray constructor is also able to mimic codecs.encode()
        }

        /**
         * Create an unpickler.
         */
        public Unpickler()
        {
            memo = new Dictionary<int, dynamic>();
        }

        public void Dispose()
        {
            close();
        }

        /**
         * Register additional object constructors for custom classes.
         */
        public static void registerConstructor(string module, string classname, IObjectConstructor constructor)
        {
            objectConstructors[module + "." + classname] = constructor;
        }

        /**
         * Read a pickled object representation from the given input stream.
         * 
         * @return the reconstituted object hierarchy specified in the file.
         */
        public dynamic load(Stream stream)
        {
            input = stream;
            stack = new UnpickleStack();
            while (true)
            {
                var key = PickleUtils.readbyte(input);
                var value = dispatch(key);
                if (value != NO_RETURN_VALUE)
                    return value;
            }
        }

        /**
         * Read a pickled dynamic representation from the given pickle data bytes.
         * 
         * @return the reconstituted dynamic hierarchy specified in the file.
         */
        public dynamic loads(byte[] pickledata)
        {
            return load(new MemoryStream(pickledata));
        }

        /**
         * Close the unpickler and frees the resources such as the unpickle stack and memo table.
         */
        public void close()
        {
            stack?.clear();
            memo?.Clear();
            input?.Close();
        }

        /**
         * Process a single pickle stream opcode.
         */
        protected dynamic dispatch(short key)
        {
            switch (key)
            {
                case Opcodes.MARK:
                    load_mark();
                    break;
                case Opcodes.STOP:
                    var value = stack.pop();
                    stack.clear();
                    return value; // final result value
                case Opcodes.POP:
                    load_pop();
                    break;
                case Opcodes.POP_MARK:
                    load_pop_mark();
                    break;
                case Opcodes.DUP:
                    load_dup();
                    break;
                case Opcodes.FLOAT:
                    load_float();
                    break;
                case Opcodes.INT:
                    load_int();
                    break;
                case Opcodes.BININT:
                    load_binint();
                    break;
                case Opcodes.BININT1:
                    load_binint1();
                    break;
                case Opcodes.LONG:
                    load_long();
                    break;
                case Opcodes.BININT2:
                    load_binint2();
                    break;
                case Opcodes.NONE:
                    load_none();
                    break;
                case Opcodes.PERSID:
                    load_persid();
                    break;
                case Opcodes.BINPERSID:
                    load_binpersid();
                    break;
                case Opcodes.REDUCE:
                    load_reduce();
                    break;
                case Opcodes.STRING:
                    load_string();
                    break;
                case Opcodes.BINSTRING:
                    load_binstring();
                    break;
                case Opcodes.SHORT_BINSTRING:
                    load_short_binstring();
                    break;
                case Opcodes.UNICODE:
                    load_unicode();
                    break;
                case Opcodes.BINUNICODE:
                    load_binunicode();
                    break;
                case Opcodes.APPEND:
                    load_append();
                    break;
                case Opcodes.BUILD:
                    load_build();
                    break;
                case Opcodes.GLOBAL:
                    load_global();
                    break;
                case Opcodes.DICT:
                    load_dict();
                    break;
                case Opcodes.EMPTY_DICT:
                    load_empty_dictionary();
                    break;
                case Opcodes.APPENDS:
                    load_appends();
                    break;
                case Opcodes.GET:
                    load_get();
                    break;
                case Opcodes.BINGET:
                    load_binget();
                    break;
                case Opcodes.INST:
                    load_inst();
                    break;
                case Opcodes.LONG_BINGET:
                    load_long_binget();
                    break;
                case Opcodes.LIST:
                    load_list();
                    break;
                case Opcodes.EMPTY_LIST:
                    load_empty_list();
                    break;
                case Opcodes.OBJ:
                    load_obj();
                    break;
                case Opcodes.PUT:
                    load_put();
                    break;
                case Opcodes.BINPUT:
                    load_binput();
                    break;
                case Opcodes.LONG_BINPUT:
                    load_long_binput();
                    break;
                case Opcodes.SETITEM:
                    load_setitem();
                    break;
                case Opcodes.TUPLE:
                    load_tuple();
                    break;
                case Opcodes.EMPTY_TUPLE:
                    load_empty_tuple();
                    break;
                case Opcodes.SETITEMS:
                    load_setitems();
                    break;
                case Opcodes.BINFLOAT:
                    load_binfloat();
                    break;

                // protocol 2
                case Opcodes.PROTO:
                    load_proto();
                    break;
                case Opcodes.NEWOBJ:
                    load_newobj();
                    break;
                case Opcodes.EXT1:
                case Opcodes.EXT2:
                case Opcodes.EXT4:
                    throw new PickleException(
                        "Unimplemented opcode EXT1/EXT2/EXT4 encountered. Don't use extension codes when pickling via copyreg.add_extension() to avoid this error.");
                case Opcodes.TUPLE1:
                    load_tuple1();
                    break;
                case Opcodes.TUPLE2:
                    load_tuple2();
                    break;
                case Opcodes.TUPLE3:
                    load_tuple3();
                    break;
                case Opcodes.NEWTRUE:
                    load_true();
                    break;
                case Opcodes.NEWFALSE:
                    load_false();
                    break;
                case Opcodes.LONG1:
                    load_long1();
                    break;
                case Opcodes.LONG4:
                    load_long4();
                    break;

                // Protocol 3 (Python 3.x)
                case Opcodes.BINBYTES:
                    load_binbytes();
                    break;
                case Opcodes.SHORT_BINBYTES:
                    load_short_binbytes();
                    break;

                // Protocol 4 (Python 3.4+)
                case Opcodes.BINUNICODE8:
                    load_binunicode8();
                    break;
                case Opcodes.SHORT_BINUNICODE:
                    load_short_binunicode();
                    break;
                case Opcodes.BINBYTES8:
                    load_binbytes8();
                    break;
                case Opcodes.EMPTY_SET:
                    load_empty_set();
                    break;
                case Opcodes.ADDITEMS:
                    load_additems();
                    break;
                case Opcodes.FROZENSET:
                    load_frozenset();
                    break;
                case Opcodes.MEMOIZE:
                    load_memoize();
                    break;
                case Opcodes.FRAME:
                    load_frame();
                    break;
                case Opcodes.NEWOBJ_EX:
                    load_newobj_ex();
                    break;
                case Opcodes.STACK_GLOBAL:
                    load_stack_global();
                    break;

                default:
                    throw new InvalidOpcodeException("invalid pickle opcode: " + key);
            }

            return NO_RETURN_VALUE;
        }

        private void load_build()
        {
            var args = stack.pop();
            var target = stack.peek();
            dynamic[] arguments = {args};
            Type[] argumentTypes = {args.GetType()};

            // call the __setstate__ method with the given arguments
            try
            {
                MethodInfo setStateMethod = target.GetType().GetMethod("__setstate__", argumentTypes);
                if (setStateMethod == null)
                    throw new PickleException(
                        $"no __setstate__() found in type {target.GetType()} with argument type {args.GetType()}");
                setStateMethod.Invoke(target, arguments);
            }
            catch (Exception e)
            {
                throw new PickleException("failed to __setstate__()", e);
            }
        }

        private void load_proto()
        {
            var proto = PickleUtils.readbyte(input);
            if (proto > HIGHEST_PROTOCOL)
                throw new PickleException("unsupported pickle protocol: " + proto);
        }

        private void load_none()
        {
            stack.add(null);
        }

        private void load_false()
        {
            stack.add(false);
        }

        private void load_true()
        {
            stack.add(true);
        }

        private void load_int()
        {
            var data = PickleUtils.readline(input, true);
            dynamic val;
            if (data == Opcodes.FALSE.Substring(1))
            {
                val = false;
            }
            else if (data == Opcodes.TRUE.Substring(1))
            {
                val = true;
            }
            else
            {
                var number = data.Substring(0, data.Length - 1);
                try
                {
                    val = int.Parse(number);
                }
                catch (OverflowException)
                {
                    // hmm, integer didnt' work.. is it perhaps an int from a 64-bit python? so try long:
                    val = long.Parse(number);
                }
            }

            stack.add(val);
        }

        private void load_binint()
        {
            var integer = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            stack.add(integer);
        }

        private void load_binint1()
        {
            stack.add((int) PickleUtils.readbyte(input));
        }

        private void load_binint2()
        {
            var integer = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 2));
            stack.add(integer);
        }

        private void load_long()
        {
            var val = PickleUtils.readline(input);
            if (val.EndsWith("L")) val = val.Substring(0, val.Length - 1);
            long longvalue;
            if (long.TryParse(val, out longvalue))
                stack.add(longvalue);
            else
                throw new PickleException("long too large in load_long (need BigInt)");
        }

        private void load_long1()
        {
            var n = PickleUtils.readbyte(input);
            var data = PickleUtils.readbytes(input, n);
            stack.add(PickleUtils.decode_long(data));
        }

        private void load_long4()
        {
            var n = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            var data = PickleUtils.readbytes(input, n);
            stack.add(PickleUtils.decode_long(data));
        }

        private void load_float()
        {
            var val = PickleUtils.readline(input, true);
            var d = double.Parse(val,
                NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                NumberFormatInfo.InvariantInfo);
            stack.add(d);
        }

        private void load_binfloat()
        {
            var val = PickleUtils.bytes_bigendian_to_double(PickleUtils.readbytes(input, 8), 0);
            stack.add(val);
        }

        private void load_string()
        {
            var rep = PickleUtils.readline(input);
            var quotesOk = false;
            foreach (var q in new[] {"\"", "'"}) // double or single quote
                if (rep.StartsWith(q))
                {
                    if (!rep.EndsWith(q)) throw new PickleException("insecure string pickle");
                    rep = rep.Substring(1, rep.Length - 2); // strip quotes
                    quotesOk = true;
                    break;
                }

            if (!quotesOk)
                throw new PickleException("insecure string pickle");

            stack.add(PickleUtils.decode_escaped(rep));
        }

        private void load_binstring()
        {
            var len = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            var data = PickleUtils.readbytes(input, len);
            stack.add(PickleUtils.rawStringFromBytes(data));
        }

        private void load_binbytes()
        {
            var len = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            stack.add(PickleUtils.readbytes(input, len));
        }

        private void load_binbytes8()
        {
            var len = PickleUtils.bytes_to_long(PickleUtils.readbytes(input, 8), 0);
            stack.add(PickleUtils.readbytes(input, len));
        }

        private void load_unicode()
        {
            var str = PickleUtils.decode_unicode_escaped(PickleUtils.readline(input));
            stack.add(str);
        }

        private void load_binunicode()
        {
            var len = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            var data = PickleUtils.readbytes(input, len);
            stack.add(Encoding.UTF8.GetString(data));
        }

        private void load_binunicode8()
        {
            var len = PickleUtils.bytes_to_long(PickleUtils.readbytes(input, 8), 0);
            var data = PickleUtils.readbytes(input, len);
            stack.add(Encoding.UTF8.GetString(data));
        }

        private void load_short_binunicode()
        {
            int len = PickleUtils.readbyte(input);
            var data = PickleUtils.readbytes(input, len);
            stack.add(Encoding.UTF8.GetString(data));
        }

        private void load_short_binstring()
        {
            var len = PickleUtils.readbyte(input);
            var data = PickleUtils.readbytes(input, len);
            stack.add(PickleUtils.rawStringFromBytes(data));
        }

        private void load_short_binbytes()
        {
            var len = PickleUtils.readbyte(input);
            stack.add(PickleUtils.readbytes(input, len));
        }

        private void load_tuple()
        {
            var top = stack.pop_all_since_marker();
            stack.add(top.ToArray());
        }

        private void load_empty_tuple()
        {
            stack.add(new dynamic[0]);
        }

        private void load_tuple1()
        {
            stack.add(new[] {stack.pop()});
        }

        private void load_tuple2()
        {
            var o2 = stack.pop();
            var o1 = stack.pop();
            stack.add(new[] {o1, o2});
        }

        private void load_tuple3()
        {
            var o3 = stack.pop();
            var o2 = stack.pop();
            var o1 = stack.pop();
            stack.add(new[] {o1, o2, o3});
        }

        private void load_empty_list()
        {
            stack.add(new ArrayList(5));
        }

        private void load_empty_dictionary()
        {
            stack.add(new Hashtable(5));
        }

        private void load_empty_set()
        {
            stack.add(new HashSet<dynamic>());
        }

        private void load_list()
        {
            var top = stack.pop_all_since_marker();
            stack.add(top); // simply add the top items as a list to the stack again
        }

        private void load_dict()
        {
            var top = stack.pop_all_since_marker();
            var map = new Hashtable(top.Count);
            for (var i = 0; i < top.Count; i += 2)
            {
                dynamic key = top[i];
                dynamic value = top[i + 1];
                map[key] = value;
            }

            stack.add(map);
        }

        private void load_frozenset()
        {
            var top = stack.pop_all_since_marker();
            var set = new HashSet<dynamic>();
            foreach (var element in top)
                set.Add(element);
            stack.add(set);
        }

        private void load_additems()
        {
            var top = stack.pop_all_since_marker();
            var set = (HashSet<dynamic>) stack.pop();
            foreach (dynamic item in top)
                set.Add(item);
            stack.add(set);
        }

        private void load_global()
        {
            var module = PickleUtils.readline(input);
            var name = PickleUtils.readline(input);
            load_global_sub(module, name);
        }

        private void load_stack_global()
        {
            var name = (string) stack.pop();
            var module = (string) stack.pop();
            load_global_sub(module, name);
        }

        private void load_global_sub(string module, string name)
        {
            IObjectConstructor constructor;
            var key = module + "." + name;
            if (objectConstructors.ContainsKey(key))
                constructor = objectConstructors[module + "." + name];
            else
                switch (module)
                {
                    // check if it is an exception
                    case "exceptions":
                        // python 2.x
                        constructor = new ExceptionConstructor(typeof(PythonException), module, name);
                        break;
                    case "builtins":
                    case "__builtin__":
                        if (name.EndsWith("Error") || name.EndsWith("Warning") || name.EndsWith("Exception")
                            || name == "GeneratorExit" || name == "KeyboardInterrupt"
                            || name == "StopIteration" || name == "SystemExit")
                            constructor = new ExceptionConstructor(typeof(PythonException), module, name);
                        else
                            constructor = new ClassDictConstructor(module, name);

                        break;
                    default:
                        constructor = new ClassDictConstructor(module, name);
                        break;
                }
            stack.add(constructor);
        }

        private void load_pop()
        {
            stack.pop();
        }

        private void load_pop_mark()
        {
            dynamic o;
            do
            {
                o = stack.pop();
            } while (o != stack.MARKER);

            stack.trim();
        }

        private void load_dup()
        {
            stack.add(stack.peek());
        }

        private void load_get()
        {
            var i = int.Parse(PickleUtils.readline(input));
            if (!memo.ContainsKey(i)) throw new PickleException("invalid memo key");
            stack.add(memo[i]);
        }

        private void load_binget()
        {
            var i = PickleUtils.readbyte(input);
            if (!memo.ContainsKey(i)) throw new PickleException("invalid memo key");
            stack.add(memo[i]);
        }

        private void load_long_binget()
        {
            var i = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            if (!memo.ContainsKey(i)) throw new PickleException("invalid memo key");
            stack.add(memo[i]);
        }

        private void load_put()
        {
            var i = int.Parse(PickleUtils.readline(input));
            memo[i] = stack.peek();
        }

        private void load_binput()
        {
            var i = PickleUtils.readbyte(input);
            memo[i] = stack.peek();
        }

        private void load_memoize()
        {
            memo[memo.Count] = stack.peek();
        }

        private void load_long_binput()
        {
            var i = PickleUtils.bytes_to_integer(PickleUtils.readbytes(input, 4));
            memo[i] = stack.peek();
        }

        private void load_append()
        {
            var value = stack.pop();
            var list = (ArrayList) stack.peek();
            list.Add(value);
        }

        private void load_appends()
        {
            var top = stack.pop_all_since_marker();
            var list = (ArrayList) stack.peek();
            list.AddRange(top);
            list.TrimToSize();
        }

        private void load_setitem()
        {
            var value = stack.pop();
            var key = stack.pop();
            var dict = (Hashtable) stack.peek();
            dict[key] = value;
        }

        private void load_setitems()
        {
            var newitems = new List<KeyValuePair<dynamic, dynamic>>();
            var value = stack.pop();
            while (value != stack.MARKER)
            {
                var key = stack.pop();
                newitems.Add(new KeyValuePair<dynamic, dynamic>(key, value));
                value = stack.pop();
            }

            var dict = (Hashtable) stack.peek();
            foreach (var item in newitems) dict[item.Key] = item.Value;
        }

        private void load_mark()
        {
            stack.add_mark();
        }

        private void load_reduce()
        {
            var args = (dynamic[]) stack.pop();
            var constructor = (IObjectConstructor) stack.pop();
            stack.add(constructor.construct(args));
        }

        private void load_newobj()
        {
            load_reduce(); // we just do the same as class(*args) instead of class.__new__(class,*args)
        }

        private void load_newobj_ex()
        {
            var kwargs = (Hashtable) stack.pop();
            var args = (dynamic[]) stack.pop();
            var constructor = (IObjectConstructor) stack.pop();
            if (kwargs.Count == 0)
                stack.add(constructor.construct(args));
            else
                throw new PickleException("newobj_ex with keyword arguments not supported");
        }

        private void load_frame()
        {
            // for now we simply skip the frame opcode and its length
            PickleUtils.readbytes(input, 8);
        }

        private void load_persid()
        {
            // the persistent id is taken from the argument
            var pid = PickleUtils.readline(input);
            stack.add(persistentLoad(pid));
        }

        private void load_binpersid()
        {
            // the persistent id is taken from the stack
            string pid = stack.pop().ToString();
            stack.add(persistentLoad(pid));
        }

        private void load_obj()
        {
            var args = stack.pop_all_since_marker();
            var constructor = (IObjectConstructor) args[0];
            args = args.GetRange(1, args.Count - 1);
            var obj = constructor.construct(args.ToArray());
            stack.add(obj);
        }

        private void load_inst()
        {
            var module = PickleUtils.readline(input);
            var classname = PickleUtils.readline(input);
            var args = stack.pop_all_since_marker();
            IObjectConstructor constructor;
            if (objectConstructors.ContainsKey(module + "." + classname))
            {
                constructor = objectConstructors[module + "." + classname];
            }
            else
            {
                constructor = new ClassDictConstructor(module, classname);
                args.Clear(); // classdict doesn't have constructor args... so we may lose info here, hmm.
            }

            var obj = constructor.construct(args.ToArray());
            stack.add(obj);
        }

        protected virtual dynamic persistentLoad(string pid)
        {
            throw new PickleException(
                "A load persistent id instruction was encountered, but no persistentLoad function was specified. (implement it in custom Unpickler subclass)");
        }
    }
}