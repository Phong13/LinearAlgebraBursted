using System;

using Unity.Burst;
using System.Runtime.InteropServices;


namespace LinearAlgebra
{
    [StructLayout(LayoutKind.Sequential)]
    public struct fProxy : IComparable<fProxy>, IEquatable<fProxy>
    {
        private float value;

        public static float MinValue => float.MinValue;
        public static float MaxValue => float.MaxValue;

        public static float NaN => float.NaN;

        public static float PositiveInfinity => float.PositiveInfinity;

        public static float NegativeInfinity => float.NegativeInfinity;

        public fProxy(float value)
        {
            this.value = value;
        }

        // Conversion operators
        public static implicit operator fProxy(float value) => new fProxy(value);
        public static implicit operator float(fProxy d) => d.value;

        // Arithmetic operators
        public static fProxy operator +(fProxy a, fProxy b) => new fProxy(a.value + b.value);
        public static fProxy operator -(fProxy a, fProxy b) => new fProxy(a.value - b.value);
        public static fProxy operator *(fProxy a, fProxy b) => new fProxy(a.value * b.value);
        public static fProxy operator /(fProxy a, fProxy b) => new fProxy(a.value / b.value);

        // Unary operators
        public static fProxy operator -(fProxy a) => new fProxy(-a.value);
        public static fProxy operator ++(fProxy a) => new fProxy(a.value + 1);
        public static fProxy operator --(fProxy a) => new fProxy(a.value - 1);

        // Comparison operators
        public static bool operator ==(fProxy a, fProxy b) => a.value == b.value;
        public static bool operator !=(fProxy a, fProxy b) => a.value != b.value;
        public static bool operator <(fProxy a, fProxy b) => a.value < b.value;
        public static bool operator >(fProxy a, fProxy b) => a.value > b.value;
        public static bool operator <=(fProxy a, fProxy b) => a.value <= b.value;
        public static bool operator >=(fProxy a, fProxy b) => a.value >= b.value;

        // Overridden methods for proper struct behavior
        /*[BurstDiscard]
        public override bool Equals(object obj) => obj is fProxy d && this.value == d.value;*/
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString(); 

        public static bool IsNaN(fProxy f)
        {
            return float.IsNaN(f);
        }

        public static bool IsInfinity(fProxy f)
        {
            return float.IsInfinity(f);
        }

        public static bool IsPositiveInfinity(fProxy f)
        {
            return float.IsPositiveInfinity(f);
        }

        public static bool IsNegativeInfinity(fProxy f)
        {
            return float.IsNegativeInfinity(f);
        }

        public static bool IsFinite(fProxy f)
        {
            return float.IsFinite(f);
        }

        public int CompareTo(fProxy other) {
            return value.CompareTo(other.value);
        }

        public bool Equals(fProxy other)
        {
            return this.value == other.value;
        }

        // Allow explicit cast from double → fProxy
        public static implicit operator fProxy(double d)
        {
            return new fProxy { value = (float) d };
        }

        // Allow implicit cast from fProxy → double
        public static implicit operator double(fProxy p)
        {
            return p.value;
        }
    }

    public struct iProxy
    {
        private int value;

        public iProxy(int value)
        {
            this.value = value;
        }

        // Conversion operators
        public static implicit operator iProxy(int value) => new iProxy(value);
        public static implicit operator int(iProxy d) => d.value;

        // Arithmetic operators
        public static iProxy operator +(iProxy a, iProxy b) => new iProxy(a.value + b.value);
        public static iProxy operator -(iProxy a, iProxy b) => new iProxy(a.value - b.value);
        public static iProxy operator *(iProxy a, iProxy b) => new iProxy(a.value * b.value);
        public static iProxy operator /(iProxy a, iProxy b) => new iProxy(a.value / b.value);
        public static iProxy operator %(iProxy a, iProxy b) => new iProxy(a.value % b.value);

        // Unary operators
        public static iProxy operator -(iProxy a) => new iProxy(-a.value);
        public static iProxy operator ++(iProxy a) => new iProxy(a.value + 1);
        public static iProxy operator --(iProxy a) => new iProxy(a.value - 1);

        // Comparison operators
        public static bool operator ==(iProxy a, iProxy b) => a.value == b.value;
        public static bool operator !=(iProxy a, iProxy b) => a.value != b.value;
        public static bool operator <(iProxy a, iProxy b) => a.value < b.value;
        public static bool operator >(iProxy a, iProxy b) => a.value > b.value;
        public static bool operator <=(iProxy a, iProxy b) => a.value <= b.value;
        public static bool operator >=(iProxy a, iProxy b) => a.value >= b.value;

        // Bitwise operators
        public static iProxy operator &(iProxy a, iProxy b) => new iProxy(a.value & b.value);
        public static iProxy operator |(iProxy a, iProxy b) => new iProxy(a.value | b.value);
        public static iProxy operator ^(iProxy a, iProxy b) => new iProxy(a.value ^ b.value);
        public static iProxy operator ~(iProxy a) => new iProxy(~a.value);

        // Shift operators
        public static iProxy operator <<(iProxy a, int shift) => new iProxy(a.value << shift);
        public static iProxy operator >>(iProxy a, int shift) => new iProxy(a.value >> shift);


        // Overridden methods for proper struct behavior
        [BurstDiscard]
        public override bool Equals(object obj) => obj is iProxy d && this.value == d.value;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public struct anyProxy : IComparable<anyProxy> {

        private float value;

        public static float MinValue => float.MinValue;
        public static float MaxValue => float.MaxValue;

        public anyProxy(float value) {
            this.value = value;
        }

        // Conversion operators
        public static implicit operator anyProxy(float value) => new anyProxy(value);
        public static implicit operator float(anyProxy d) => d.value;

        // Arithmetic operators
        public static anyProxy operator +(anyProxy a, anyProxy b) => new anyProxy(a.value + b.value);
        public static anyProxy operator -(anyProxy a, anyProxy b) => new anyProxy(a.value - b.value);
        public static anyProxy operator *(anyProxy a, anyProxy b) => new anyProxy(a.value * b.value);
        public static anyProxy operator /(anyProxy a, anyProxy b) => new anyProxy(a.value / b.value);

        // Unary operators
        public static anyProxy operator -(anyProxy a) => new anyProxy(-a.value);
        public static anyProxy operator ++(anyProxy a) => new anyProxy(a.value + 1);
        public static anyProxy operator --(anyProxy a) => new anyProxy(a.value - 1);

        // Comparison operators
        public static bool operator ==(anyProxy a, anyProxy b) => a.value == b.value;
        public static bool operator !=(anyProxy a, anyProxy b) => a.value != b.value;
        public static bool operator <(anyProxy a, anyProxy b) => a.value < b.value;
        public static bool operator >(anyProxy a, anyProxy b) => a.value > b.value;
        public static bool operator <=(anyProxy a, anyProxy b) => a.value <= b.value;
        public static bool operator >=(anyProxy a, anyProxy b) => a.value >= b.value;

        // Overridden methods for proper struct behavior
        /*[BurstDiscard]
        public override bool Equals(object obj) => obj is anyProxy d && this.value == d.value;*/
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();

        public int CompareTo(anyProxy other) {
            return value.CompareTo(other.value);
        }
    }

}