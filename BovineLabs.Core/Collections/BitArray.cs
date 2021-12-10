﻿// <copyright file="BitArray.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#pragma warning disable SA1649 // Filename must match

namespace BovineLabs.Core.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// IBitArray interface.
    /// Originally based off com.unity.render-pipelines.core@12.0.0\Runtime\Utilities\BitArray but made generic and burst friendly.
    /// </summary>
    /// <typeparam name="T"> The type.</typeparam>
    public interface IBitArray<T>
        where T : IBitArray<T>
    {
        /// <summary>Gets the capacity of this BitArray. This is the number of bits that are usable.</summary>
        uint Capacity { get; }

        /// <summary>Return `true` if all the bits of this BitArray are set to 0. Returns `false` otherwise.</summary>
        bool AllFalse { get; }

        /// <summary>Return `true` if all the bits of this BitArray are set to 1. Returns `false` otherwise.</summary>
        bool AllTrue { get; }

        /// <summary> An indexer that allows access to the bit at a given index. This provides both read and write access. </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        bool this[uint index] { get; set; }

        /// <summary>
        /// Writes the bits in the array in a human-readable form. This is as a string of 0s and 1s packed by 8 bits. This is useful for debugging.
        /// </summary>
        string HumanizedData { get; }

        /// <summary>
        /// Perform an AND bitwise operation between this BitArray and the one you pass into the function and return the result.
        /// Both BitArrays must have the same capacity. This will not change current BitArray values.
        /// </summary>
        /// <param name="other">BitArray with which to the And operation.</param>
        /// <returns>The resulting bit array.</returns>
        T BitAnd(T other);

        /// <summary>
        /// Perform an OR bitwise operation between this BitArray and the one you pass into the function and return the result.
        /// Both BitArrays must have the same capacity. This will not change current BitArray values.
        /// </summary>
        /// <param name="other">BitArray with which to the Or operation.</param>
        /// <returns>The resulting bit array.</returns>
        T BitOr(T other);

        /// <summary> Return the BitArray with every bit inverted. </summary>
        /// <returns>The resulting bit array.</returns>
        T BitNot();
    }

    // /!\ Important for serialization:
    // Serialization helper will rely on the name of the struct type.
    // In order to work, it must be BitArrayN where N is the capacity without suffix.

    /// <summary> Bit array of size 8. </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray8 : IBitArray<BitArray8>
    {
        [SerializeField]
        private byte data;

        /// <summary> Initializes a new instance of the <see cref="BitArray8"/> struct. </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray8(byte initValue) => this.data = initValue;

        /// <summary> Initializes a new instance of the <see cref="BitArray8"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray8(IEnumerable<uint> bitIndexTrue)
        {
            this.data = (byte)0u;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= this.Capacity)
                {
                    continue;
                }

                this.data |= (byte)(1u << (int)bitIndex);
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 8u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data == 0u;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data == byte.MaxValue;

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData => string.Format("{0, " + this.Capacity + "}", Convert.ToString(this.data, 2)).Replace(' ', '0');

        /// <summary> Returns the state of the bit at a specific index. </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get8(index, this.data);
            set => BitArrayUtilities.Set8(index, ref this.data, value);
        }

        /// <summary> Bit-wise Not operator </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator ~(BitArray8 a) => new BitArray8((byte)~a.data);

        /// <summary> Bit-wise Or operator </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator |(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data | b.data));

        /// <summary> Bit-wise And operator </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray8 operator &(BitArray8 a, BitArray8 b) => new BitArray8((byte)(a.data & b.data));

        /// <summary> Equality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray8 a, BitArray8 b) => a.data == b.data;

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray8 a, BitArray8 b) => a.data != b.data;

        /// <summary> Bit-wise And </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray8 BitAnd(BitArray8 other) => this & other;

        /// <summary> Bit-wise Or </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray8 BitOr(BitArray8 other) => this | other;

        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray8 BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray8 && ((BitArray8)obj).data == this.data;

        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + this.data.GetHashCode();
    }

    /// <summary> Bit array of size 16. </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray16 : IBitArray<BitArray16>
    {
        [SerializeField]
        private ushort data;

        /// <summary> Initializes a new instance of the <see cref="BitArray16"/> struct. </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray16(ushort initValue) => this.data = initValue;

        /// <summary> Initializes a new instance of the <see cref="BitArray16"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray16(IEnumerable<uint> bitIndexTrue)
        {
            this.data = (ushort)0u;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= this.Capacity)
                {
                    continue;
                }

                this.data |= (ushort)(1u << (int)bitIndex);
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 16u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data == 0u;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data == ushort.MaxValue;

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData => System.Text.RegularExpressions.Regex
            .Replace(string.Format("{0, " + this.Capacity + "}", Convert.ToString(this.data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get16(index, this.data);
            set => BitArrayUtilities.Set16(index, ref this.data, value);
        }


        /// <summary> Bit-wise Not operator. </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator ~(BitArray16 a) => new BitArray16((ushort)~a.data);

        /// <summary> Bit-wise Or operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator |(BitArray16 a, BitArray16 b) => new BitArray16((ushort)(a.data | b.data));

        /// <summary> Bit-wise And operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray16 operator &(BitArray16 a, BitArray16 b) => new BitArray16((ushort)(a.data & b.data));

        /// <summary> Equality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray16 a, BitArray16 b) => a.data == b.data;

        /// <summary> Inequality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray16 a, BitArray16 b) => a.data != b.data;

        /// <summary> Bit-wise And. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray16 BitAnd(BitArray16 other) => this & other;

        /// <summary> Bit-wise Or. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray16 BitOr(BitArray16 other) => this | other;

        /// <summary> Bit-wise Not. </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray16 BitNot() => ~this;

        /// <summary> Equality operator. </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray16 array16 && array16.data == this.data;

        /// <summary> Get the hashcode of the bit array. </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + this.data.GetHashCode();
    }

    /// <summary> Bit array of size 32. </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray32 : IBitArray<BitArray32>
    {
        [SerializeField]
        private uint data;

        /// <summary> Initializes a new instance of the <see cref="BitArray32"/> struct. </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray32(uint initValue) => this.data = initValue;

        /// <summary> Initializes a new instance of the <see cref="BitArray32"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray32(IEnumerable<uint> bitIndexTrue)
        {
            this.data = 0u;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= this.Capacity)
                {
                    continue;
                }

                this.data |= 1u << (int)bitIndex;
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 32u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data == 0u;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data == uint.MaxValue;

        private string humanizedVersion => Convert.ToString(this.data, 2);

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData => System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + this.Capacity + "}", Convert.ToString(this.data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get32(index, this.data);
            set => BitArrayUtilities.Set32(index, ref this.data, value);
        }

        /// <summary> Bit-wise Not operator. </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator ~(BitArray32 a) => new BitArray32(~a.data);

        /// <summary> Bit-wise Or operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator |(BitArray32 a, BitArray32 b) => new BitArray32(a.data | b.data);

        /// <summary> Bit-wise And operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray32 operator &(BitArray32 a, BitArray32 b) => new BitArray32(a.data & b.data);

        /// <summary> Equality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray32 a, BitArray32 b) => a.data == b.data;

        /// <summary> Inequality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray32 a, BitArray32 b) => a.data != b.data;

        /// <summary> Bit-wise And. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray32 BitAnd(BitArray32 other) => this & other;

        /// <summary> Bit-wise Or. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray32 BitOr(BitArray32 other) => this | other;

        /// <summary> Bit-wise Not. </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray32 BitNot() => ~this;

        /// <summary> Equality operator. </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray32 && ((BitArray32)obj).data == this.data;

        /// <summary> Get the hashcode of the bit array. </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + this.data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 64.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray64 : IBitArray<BitArray64>
    {
        [SerializeField]
        private ulong data;

        /// <summary> Initializes a new instance of the <see cref="BitArray64"/> struct. </summary>
        /// <param name="initValue">Initialization value.</param>
        public BitArray64(ulong initValue) => this.data = initValue;

        /// <summary> Initializes a new instance of the <see cref="BitArray64"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray64(IEnumerable<uint> bitIndexTrue)
        {
            this.data = 0L;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex >= this.Capacity)
                {
                    continue;
                }

                this.data |= 1uL << (int)bitIndex;
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 64u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data == 0uL;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data == ulong.MaxValue;

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData => System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + this.Capacity + "}", Convert.ToString((long)this.data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary>
        /// Returns the state of the bit at a specific index.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get64(index, this.data);
            set => BitArrayUtilities.Set64(index, ref this.data, value);
        }

        /// <summary> Bit-wise Not operator/ </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator ~(BitArray64 a) => new BitArray64(~a.data);

        /// <summary> Bit-wise Or operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator |(BitArray64 a, BitArray64 b) => new BitArray64(a.data | b.data);

        /// <summary> Bit-wise And operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray64 operator &(BitArray64 a, BitArray64 b) => new BitArray64(a.data & b.data);

        /// <summary> Equality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray64 a, BitArray64 b) => a.data == b.data;

        /// <summary> Inequality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray64 a, BitArray64 b) => a.data != b.data;

        /// <summary> Bit-wise And. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray64 BitAnd(BitArray64 other) => this & other;

        /// <summary> Bit-wise Or. </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray64 BitOr(BitArray64 other) => this | other;

        /// <summary> Bit-wise Not. </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray64 BitNot() => ~this;

        /// <summary> Equality operator. </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => obj is BitArray64 && ((BitArray64)obj).data == this.data;

        /// <summary> Get the hashcode of the bit array. </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode() => 1768953197 + this.data.GetHashCode();
    }

    /// <summary>
    /// Bit array of size 128.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray128 : IBitArray<BitArray128>
    {
        [SerializeField]
        private ulong data1;

        [SerializeField]
        private ulong data2;

        /// <summary> Initializes a new instance of the <see cref="BitArray128"/> struct. </summary>
        /// <param name="initValue1">Initialization value 1.</param>
        /// <param name="initValue2">Initialization value 2.</param>
        public BitArray128(ulong initValue1, ulong initValue2)
        {
            this.data1 = initValue1;
            this.data2 = initValue2;
        }

        /// <summary> Initializes a new instance of the <see cref="BitArray128"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>

        public BitArray128(IEnumerable<uint> bitIndexTrue)
        {
            this.data1 = this.data2 = 0uL;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex < 64u)
                {
                    this.data1 |= 1uL << (int)bitIndex;
                }
                else
                {
                    if (bitIndex < this.Capacity)
                    this.data2 |= 1uL << (int)(bitIndex - 64u);
                }
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 128u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data1 == 0uL && this.data2 == 0uL;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data1 == ulong.MaxValue && this.data2 == ulong.MaxValue;

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData =>
            System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data2, 2)).Replace(' ', '0'), ".{8}",
                "$0.")
            + System.Text.RegularExpressions.Regex
                .Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary> Returns the state of the bit at a specific index. </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get128(index, this.data1, this.data2);
            set => BitArrayUtilities.Set128(index, ref this.data1, ref this.data2, value);
        }

        /// <summary> Bit-wise Not operator </summary>
        /// <param name="a">First bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray128 operator ~(BitArray128 a) => new BitArray128(~a.data1, ~a.data2);

        /// <summary>
        /// Bit-wise Or operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray128 operator |(BitArray128 a, BitArray128 b) => new BitArray128(a.data1 | b.data1, a.data2 | b.data2);

        /// <summary>
        /// Bit-wise And operator
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray128 operator &(BitArray128 a, BitArray128 b) => new BitArray128(a.data1 & b.data1, a.data2 & b.data2);

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray128 a, BitArray128 b) => a.data1 == b.data1 && a.data2 == b.data2;

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray128 a, BitArray128 b) => a.data1 != b.data1 || a.data2 != b.data2;

        /// <summary>
        /// Bit-wise And
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray128 BitAnd(BitArray128 other) => this & other;

        /// <summary>
        /// Bit-wise Or
        /// </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray128 BitOr(BitArray128 other) => this | other;

        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray128 BitNot() => ~this;

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) => (obj is BitArray128) && this.data1.Equals(((BitArray128)obj).data1) && this.data2.Equals(((BitArray128)obj).data2);

        /// <summary>
        /// Get the hashcode of the bit array.
        /// </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode()
        {
            var hashCode = 1755735569;
            hashCode = hashCode * -1521134295 + this.data1.GetHashCode();
            hashCode = hashCode * -1521134295 + this.data2.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// Bit array of size 256.
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{this.GetType().Name} {HumanizedData}")]
    public struct BitArray256 : IBitArray<BitArray256>
    {
        [SerializeField]
        private ulong data1;

        [SerializeField]
        private ulong data2;

        [SerializeField]
        private ulong data3;

        [SerializeField]
        private ulong data4;

        /// <summary> Initializes a new instance of the <see cref="BitArray256"/> struct. </summary>
        /// <param name="initValue1">Initialization value 1.</param>
        /// <param name="initValue2">Initialization value 2.</param>
        /// <param name="initValue3">Initialization value 3.</param>
        /// <param name="initValue4">Initialization value 4.</param>
        public BitArray256(ulong initValue1, ulong initValue2, ulong initValue3, ulong initValue4)
        {
            this.data1 = initValue1;
            this.data2 = initValue2;
            this.data3 = initValue3;
            this.data4 = initValue4;
        }

        /// <summary> Initializes a new instance of the <see cref="BitArray256"/> struct. </summary>
        /// <param name="bitIndexTrue">List of indices where bits should be set to true.</param>
        public BitArray256(IEnumerable<uint> bitIndexTrue)
        {
            this.data1 = this.data2 = this.data3 = this.data4 = 0uL;
            if (bitIndexTrue == null)
            {
                return;
            }

            for (int index = bitIndexTrue.Count() - 1; index >= 0; --index)
            {
                uint bitIndex = bitIndexTrue.ElementAt(index);
                if (bitIndex < 64u)
                {
                    this.data1 |= 1uL << (int)bitIndex;
                }
                else if (bitIndex < 128u)
                {
                    this.data2 |= 1uL << (int)(bitIndex - 64u);
                }
                else if (bitIndex < 192u)
                {
                    this.data3 |= 1uL << (int)(bitIndex - 128u);
                }
                else if (bitIndex < this.Capacity)
                {
                    this.data4 |= 1uL << (int)(bitIndex - 192u);
                }
            }
        }

        /// <summary>Number of elements in the bit array.</summary>
        public uint Capacity => 256u;

        /// <summary>True if all bits are 0.</summary>
        public bool AllFalse => this.data1 == 0uL && this.data2 == 0uL && this.data3 == 0uL && this.data4 == 0uL;

        /// <summary>True if all bits are 1.</summary>
        public bool AllTrue => this.data1 == ulong.MaxValue && this.data2 == ulong.MaxValue && this.data3 == ulong.MaxValue && this.data4 == ulong.MaxValue;

        /// <summary>Returns the bit array in a human readable form.</summary>
        public string HumanizedData =>
            System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data4, 2)).Replace(' ', '0'), ".{8}", "$0.")
            + System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data3, 2)).Replace(' ', '0'), ".{8}", "$0.")
            + System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data2, 2)).Replace(' ', '0'), ".{8}", "$0.")
            + System.Text.RegularExpressions.Regex.Replace(string.Format("{0, " + 64u + "}", Convert.ToString((long)this.data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');

        /// <summary> Returns the state of the bit at a specific index. </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>State of the bit at the provided index.</returns>
        public bool this[uint index]
        {
            get => BitArrayUtilities.Get256(index, this.data1, this.data2, this.data3, this.data4);
            set => BitArrayUtilities.Set256(index, ref this.data1, ref this.data2, ref this.data3, ref this.data4, value);
        }

        /// <summary> Bit-wise Not operator </summary>
        /// <param name="a">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray256 operator ~(BitArray256 a) => new BitArray256(~a.data1, ~a.data2, ~a.data3, ~a.data4);

        /// <summary> Bit-wise Or operator </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray256 operator |(BitArray256 a, BitArray256 b) => new BitArray256(a.data1 | b.data1, a.data2 | b.data2, a.data3 | b.data3, a.data4 | b.data4);

        /// <summary> Bit-wise And operator </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>The resulting bit array.</returns>
        public static BitArray256 operator &(BitArray256 a, BitArray256 b) => new BitArray256(a.data1 & b.data1, a.data2 & b.data2, a.data3 & b.data3, a.data4 & b.data4);

        /// <summary> Equality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if both bit arrays are equals.</returns>
        public static bool operator ==(BitArray256 a, BitArray256 b) => a.data1 == b.data1 && a.data2 == b.data2 && a.data3 == b.data3 && a.data4 == b.data4;

        /// <summary> Inequality operator. </summary>
        /// <param name="a">First bit array.</param>
        /// <param name="b">Second bit array.</param>
        /// <returns>True if the bit arrays are not equals.</returns>
        public static bool operator !=(BitArray256 a, BitArray256 b) => a.data1 != b.data1 || a.data2 != b.data2 || a.data3 != b.data3 || a.data4 != b.data4;

        /// <summary> Bit-wise And </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray256 BitAnd(BitArray256 other) => this & other;

        /// <summary> Bit-wise Or </summary>
        /// <param name="other">Bit array with which to do the operation.</param>
        /// <returns>The resulting bit array.</returns>
        public BitArray256 BitOr(BitArray256 other) => this | other;

        /// <summary>
        /// Bit-wise Not
        /// </summary>
        /// <returns>The resulting bit array.</returns>
        public BitArray256 BitNot() => ~this;

        /// <summary> Equality operator. </summary>
        /// <param name="obj">Bit array to compare to.</param>
        /// <returns>True if the provided bit array is equal to this..</returns>
        public override bool Equals(object obj) =>
            (obj is BitArray256)
            && this.data1.Equals(((BitArray256)obj).data1)
            && this.data2.Equals(((BitArray256)obj).data2)
            && this.data3.Equals(((BitArray256)obj).data3)
            && this.data4.Equals(((BitArray256)obj).data4);

        /// <summary> Get the hashcode of the bit array. </summary>
        /// <returns>Hashcode of the bit array.</returns>
        public override int GetHashCode()
        {
            var hashCode = 1870826326;
            hashCode = hashCode * -1521134295 + this.data1.GetHashCode();
            hashCode = hashCode * -1521134295 + this.data2.GetHashCode();
            hashCode = hashCode * -1521134295 + this.data3.GetHashCode();
            hashCode = hashCode * -1521134295 + this.data4.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// Bit array utility class.
    /// </summary>
    public static class BitArrayUtilities
    {
        // written here to not duplicate the serialized accessor and runtime accessor

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get8(uint index, byte data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get16(uint index, ushort data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get32(uint index, uint data) => (data & (1u << (int)index)) != 0u;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get64(uint index, ulong data) => (data & (1uL << (int)index)) != 0uL;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get128(uint index, ulong data1, ulong data2)
            => index < 64u
            ? (data1 & (1uL << (int)index)) != 0uL
            : (data2 & (1uL << (int)(index - 64u))) != 0uL;

        /// <summary>
        /// Get a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="data3">Bit array data 3.</param>
        /// <param name="data4">Bit array data 4.</param>
        /// <returns>The value of the bit at the specific index.</returns>
        public static bool Get256(uint index, ulong data1, ulong data2, ulong data3, ulong data4)
            => index < 128u
            ? index < 64u
            ? (data1 & (1uL << (int)index)) != 0uL
            : (data2 & (1uL << (int)(index - 64u))) != 0uL
            : index < 192u
            ? (data3 & (1uL << (int)(index - 128u))) != 0uL
            : (data4 & (1uL << (int)(index - 192u))) != 0uL;

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set8(uint index, ref byte data, bool value) => data = (byte)(value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set16(uint index, ref ushort data, bool value) => data = (ushort)(value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index)));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set32(uint index, ref uint data, bool value) => data = value ? (data | (1u << (int)index)) : (data & ~(1u << (int)index));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data">Bit array data.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set64(uint index, ref ulong data, bool value) => data = value ? (data | (1uL << (int)index)) : (data & ~(1uL << (int)index));

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set128(uint index, ref ulong data1, ref ulong data2, bool value)
        {
            if (index < 64u)
            {
                data1 = value ? (data1 | (1uL << (int)index)) : (data1 & ~(1uL << (int)index));
            }
            else
            {
                data2 = value ? (data2 | (1uL << (int)(index - 64u))) : (data2 & ~(1uL << (int)(index - 64u)));
            }
        }

        /// <summary>
        /// Set a bit at a specific index.
        /// </summary>
        /// <param name="index">Bit index.</param>
        /// <param name="data1">Bit array data 1.</param>
        /// <param name="data2">Bit array data 2.</param>
        /// <param name="data3">Bit array data 3.</param>
        /// <param name="data4">Bit array data 4.</param>
        /// <param name="value">Value to set the bit to.</param>
        public static void Set256(uint index, ref ulong data1, ref ulong data2, ref ulong data3, ref ulong data4, bool value)
        {
            if (index < 64u)
            {
                data1 = value ? (data1 | (1uL << (int)index)) : (data1 & ~(1uL << (int)index));
            }
            else if (index < 128u)
            {
                data2 = value ? (data2 | (1uL << (int)(index - 64u))) : (data2 & ~(1uL << (int)(index - 64u)));
            }
            else if (index < 192u)
            {
                data3 = value ? (data3 | (1uL << (int)(index - 64u))) : (data3 & ~(1uL << (int)(index - 128u)));
            }
            else
            {
                data4 = value ? (data4 | (1uL << (int)(index - 64u))) : (data4 & ~(1uL << (int)(index - 192u)));
            }
        }
    }
}