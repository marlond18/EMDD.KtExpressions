using EMDD.KtExpressions.Terms;

using KtExtensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace EMDD.KtExpressions.Limits
{
    public class Limitless : LimitBase
    {
        public Limitless() : base(double.NegativeInfinity, double.PositiveInfinity)
        {
        }

        public override bool IsContinuous => true;
    }

    public class Limit : LimitBase
    {
        protected Limit(double lower, double upper) : base(lower, upper)
        {
        }

        public static LimitBase Create(double lower, double upper)
        {
            if (lower > upper) return Create(upper, lower);
            if (lower == double.NegativeInfinity && upper == double.PositiveInfinity) return new Limitless();
            if (lower == double.NegativeInfinity) ForwardLimit.Create(upper);
            if (upper == double.PositiveInfinity) BackwardLimit.Create(lower);
            return new Limit(lower, upper);
        }

        public override bool IsContinuous => false;
    }

    public class ForwardLimit : LimitBase
    {
        protected ForwardLimit(double upper) : base(double.NegativeInfinity, upper)
        {
        }

        public static LimitBase Create(double upper)
        {
            if (upper == double.PositiveInfinity) return new Limitless();
            return new ForwardLimit(upper);
        }
        public override bool IsContinuous => true;
    }

    public class BackwardLimit : LimitBase
    {
        protected BackwardLimit(double lower) : base(lower, double.PositiveInfinity)
        {
        }

        public override bool IsContinuous => true;

        public static LimitBase Create(double lower)
        {
            if (lower == double.PositiveInfinity) return new Limitless();
            return new BackwardLimit(lower);
        }
    }

    public abstract class LimitBase : IEquatable<LimitBase>, IComparable<LimitBase>
    {
        protected LimitBase(double lower, double upper)
        {
            if (double.IsNaN(upper)) throw new ArgumentException($"{upper} is not a valid upper limit");
            if (double.IsNaN(lower)) throw new ArgumentException($"{lower} is not a valid upper limit");
            Upper = upper;
            Lower = lower;
        }

        public abstract bool IsContinuous { get; }

        public double Upper { get; }
        public double Lower { get; }

        public int CompareTo(LimitBase other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            var lowerLimitComparison = Lower.CompareTo(other.Lower);
            if (lowerLimitComparison != 0) return lowerLimitComparison;
            return Upper.CompareTo(other.Upper);
        }

        public bool PartiallyOverlaps(LimitBase b) =>
            Lower.IsBetween(b.Lower, b.Upper) || Upper.IsBetween(b.Lower, b.Upper);

        public LimitBase[] BreakDown(LimitBase b)
        {
            var sortedLimits = new[] { Lower, Upper, b.Lower, b.Upper }.Distinct().ToArray();
            Array.Sort(sortedLimits);
            return sortedLimits.SelectPair(Limit.Create).ToArray();
        }

        public bool IsWithin(LimitBase b) =>
            Lower.IsWithin(b.Lower, b.Upper) && Upper.IsWithin(b.Lower, b.Upper);

        public bool IsBetween(LimitBase b) =>
            Lower.IsBetween(b.Lower, b.Upper) && Upper.IsBetween(b.Lower, b.Upper);

        public bool Spliceable(LimitBase b) =>
            Upper.NearEqual(b.Lower) || b.Upper.NearEqual(Lower);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (this is null || obj is null) return false;
            return Equals(obj as Term);
        }

        public bool Equals(LimitBase other)
        {
            return other != null &&
                   Upper.NearEqual(other.Upper) &&
                   Lower.NearEqual(other.Lower);
        }

        public override int GetHashCode() => unchecked(HashCode.Combine(Upper, Lower));

        public static bool operator ==(LimitBase left, LimitBase right)
        {
            return EqualityComparer<LimitBase>.Default.Equals(left, right);
        }

        public static bool operator !=(LimitBase left, LimitBase right)
        {
            return !(left == right);
        }

        public static string FormatNumber(double number)
        {
            if (number.NearEqual(double.NegativeInfinity)) return "-∞";
            if (number.NearEqual(double.PositiveInfinity)) return "∞";
            if (number.NearZero()) return $"{0}";
            return $"{number:##.###}";
        }

        public override string ToString() => $"[{FormatNumber(Lower)} to {FormatNumber(Upper)}]";

        public string ToStringPieceWise() => $"{FormatNumber(Lower)} ≤ x ≤ {FormatNumber(Upper)}";

        public LimitBase Splice(LimitBase a)
        {
            if (Spliceable(a)) return Limit.Create(Lower < a.Lower ? Lower : a.Lower, a.Upper > Upper ? a.Upper : Upper);
            throw new InvalidOperationException($"Cannot Splice {this} and {a}");
        }

        /// <summary>
        /// Gets the lowest limit and the highest limit and create a single limit out of the two
        /// </summary>
        /// <param name="a"></param>
        public LimitBase Span(LimitBase a)
        {
            return Limit.Create(Math.Min(Lower, Lower), Math.Max(Upper, a.Upper));
        }

        public double Distance => Upper - Lower;

        public static implicit operator LimitBase((double lower, double upper) limits) => Limit.Create(limits.upper, limits.lower);
    }
}