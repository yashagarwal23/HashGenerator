using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HashGenerator
{
    internal class BuiltInTypeSerializer<T>
    {
        /// <summary>
        /// The binary reader method appropriate for type T
        /// </summary>
        private Func<BinaryReader, T> binaryReaderFunction;

        /// <summary>
        /// The binary writer method appropriate for type T
        /// </summary>
        private Action<BinaryWriter, T> binaryWriterFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltInTypeSerializer{T}"/> class.
        /// </summary>
        /// <param name="binaryWriterFunction"> Binary Writer Function to use</param>
        /// <param name="binaryReaderFunction"> Binary Reader Function to use</param>
        internal BuiltInTypeSerializer(
            Action<BinaryWriter, T> binaryWriterFunction,
            Func<BinaryReader, T> binaryReaderFunction)
        {
            this.binaryWriterFunction = binaryWriterFunction;
            this.binaryReaderFunction = binaryReaderFunction;
        }

        /// <summary>
        /// Returns whether this class supports the provided type
        /// </summary>
        /// <returns>Whether the type parameter is supported</returns>
        public static bool IsSupportedType()
        {
            var builtInType = typeof(T);
            return builtInType.Equals(typeof(Guid)) || builtInType.Equals(typeof(bool))
                   || builtInType.Equals(typeof(byte)) || builtInType.Equals(typeof(sbyte))
                   || builtInType.Equals(typeof(char)) || builtInType.Equals(typeof(decimal))
                   || builtInType.Equals(typeof(double)) || builtInType.Equals(typeof(float))
                   || builtInType.Equals(typeof(int)) || builtInType.Equals(typeof(uint))
                   || builtInType.Equals(typeof(long)) || builtInType.Equals(typeof(ulong))
                   || builtInType.Equals(typeof(short)) || builtInType.Equals(typeof(ushort))
                   || builtInType.Equals(typeof(string)) || builtInType.Equals(typeof(byte[]));
        }

        /// <summary>
        /// From byte array.
        /// </summary>
        /// <param name="binaryReader"> Binary Reader instance </param>
        /// <returns>Representation of value as a 'T'</returns>
        public T Read(BinaryReader binaryReader)
        {
            return (T)this.binaryReaderFunction(binaryReader);
        }

        /// <summary>
        /// De-serializes to object.
        /// </summary>
        /// <param name="baseValue"> Base value </param>
        /// <param name="reader"> Instance of Binary Reader </param>
        /// <returns>Deserialized version of T.</returns>
        public T Read(T baseValue, BinaryReader reader)
        {
            return this.Read(reader);
        }

        /// <summary>
        /// To byte array.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        /// <param name="binaryWriter"> Binary Writer instance </param>
        public void Write(T value, BinaryWriter binaryWriter)
        {
            this.binaryWriterFunction(binaryWriter, value);
        }

        /// <summary>
        /// Serializes in to binary writer.
        /// </summary>
        /// <param name="currentValue"> Current value </param>
        /// <param name="newValue"> New value</param>
        /// <param name="binaryWriter"> Binary Writer Instance </param>
        public void Write(T currentValue, T newValue, BinaryWriter binaryWriter)
        {
            this.Write(newValue, binaryWriter);
        }
    }
}
