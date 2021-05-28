using System;
using EMDD.KtPolynomials;
using EMDD.KtNumerics;
using KtExtensions;
using EMDD.KtExpressions.Limits;

namespace EMDD.KtExpressions.Terms
{
    public abstract class Term : IComparable<Term>
    {
        internal LimitBase Limits { get; set; }

        protected Term(KtPolynomial numerator, KtPolynomial denominator, LimitBase limits)
        {
            (Numerator, Denominator) = numerator / denominator;
            Limits = limits;
        }

        protected Term(KtPolynomial polynomial, LimitBase limits) : this(polynomial.Clone(), new One(), limits) { }

        public static implicit operator Term(double other) => (Number)other;

        public static implicit operator Term(long other) => (Number)other;

        public Number Evaluate(Number variableValue) => variableValue.IsWithin(Limits.Lower, Limits.Upper) ? InnerEvaluate(variableValue) : 0;

        internal Number InnerEvaluate(Number variableValue) => Numerator.HornerScheme(variableValue) / Denominator.HornerScheme(variableValue);

        public static implicit operator Term(Number other) => KtPolynomial.Create(other);

        public static implicit operator Term(KtPolynomial other) => new ContinuousTerm(other);

        protected bool Equals(Term other) => this.TestNullBeforeEquals(other, () => Numerator == other.Numerator && Denominator == other.Denominator && Limits == other.Limits);

        public override bool Equals(object obj) => Equals(obj as Term);

        public override int GetHashCode() => unchecked(HashCode.Combine(397, Numerator, Denominator, Limits));

        internal KtPolynomial Numerator { get; set; }

        internal KtPolynomial Denominator { get; set; }

        public abstract Term[] AddWith(Term other);

        public abstract Term Invert();

        public abstract Term Negate();

        public abstract Term MultiplyWith(Term other);

        public static Term[] operator +(Term a, Term b)
        {
            if (a is null && b is null) return null;
            if (b is null || b == 0) return new[] { a };
            if (a is null || a == 0) return new[] { b };
            return a.AddWith(b);
        }

        public static Term operator -(Term a) => a?.Negate();

        public static Term[] operator -(Term a, Term b) => a + -b;

        public static Term operator *(Term a, Term b) => a is null || b is null ? null : a.MultiplyWith(b);

        public static Term operator /(Term a, Term b) => a * b.Invert();

        public static bool operator ==(Term a, Term b) => a.DefaultEquals(b);

        public static bool operator !=(Term a, Term b) => !(a == b);

        public abstract Term Clone();

        public abstract override string ToString();
        public abstract string ToStringPieceWise();

        public int CompareTo(Term other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return Limits.CompareTo(other.Limits);
        }

        public Term Splice(Term a)
        {
            if (Spliceable(a))
                return TermHelper.Create(Numerator, Denominator, Limits.Splice(a.Limits));
            throw new InvalidOperationException($"Cannot Splice {this} and {a}");
        }

        public bool Spliceable(Term a) =>
            Numerator == a.Numerator &&
            Denominator == a.Denominator &&
            Limits.Spliceable(a.Limits);

        public bool AffectedBy(Term b)
        {
            if (Spliceable(b)) return true;
            if (Limits.IsWithin(b.Limits)) return true;
            if (b.Limits.IsWithin(Limits)) return true;
            return Limits.PartiallyOverlaps(b.Limits);
        }
    }
}