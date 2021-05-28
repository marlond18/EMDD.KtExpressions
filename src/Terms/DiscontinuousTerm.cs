using System.Collections.Generic;
using System.Linq;
using EMDD.KtPolynomials;
using EMDD.KtNumerics;
using EMDD.KtExpressions.Limits;

namespace EMDD.KtExpressions.Terms
{
    public class DiscontinuousTerm : Term
    {
        public DiscontinuousTerm(KtPolynomial numerator, KtPolynomial denominator, LimitBase limits) : base(numerator, denominator, limits) { }

        public DiscontinuousTerm(KtPolynomial polynomial, LimitBase limits) : base(polynomial, limits) { }

        public override Term[] AddWith(Term other)
        {
            if (Spliceable(other))
                return new[] { Splice(other) };
            if (!AffectedBy(other)) return new[] { Clone(), other.Clone() };
            var added = TermHelper.AddValues(this, other);
            if (Limits == other.Limits)
                return new[] { TermHelper.Create(added.Numerator, added.Denominator, Limits) };
            var tempList = new List<Term>();
            foreach (var limits in Limits.BreakDown(other.Limits))
            {
                var withinA = limits.IsWithin(Limits);
                var withinB = limits.IsWithin(other.Limits);
                if (withinA && withinB)
                {
                    tempList.Add(TermHelper.Create(added.Numerator, added.Denominator, limits));
                }
                else if (withinA)
                {
                    tempList.Add(TermHelper.Create(Numerator, Denominator, limits));
                }
                else if (withinB)
                {
                    tempList.Add(TermHelper.Create(other.Numerator, other.Denominator, limits));
                }
            }
            return tempList.ToArray();
        }

        public override Term Invert() => new DiscontinuousTerm(Denominator, Numerator, Limits);

        public override Term Negate() => new DiscontinuousTerm(-Numerator, Denominator, Limits);

        public override Term MultiplyWith(Term other)
        {
            var multipliedVals = TermHelper.MultiplyValues(this, other);
            var asdasdasdasd = Limits.BreakDown(other.Limits).First(lim => lim.IsWithin(Limits) && lim.IsWithin(other.Limits));
            if (Limits is null) return (Number)0;
            return TermHelper.Create(multipliedVals.Numerator, multipliedVals.Denominator, Limits);
        }

        public override Term Clone() => new DiscontinuousTerm(Numerator, Denominator, Limits);

        public override string ToString() => $"[{BodyToString()}]{Limits}";

        public override string ToStringPieceWise() => $"{BodyToString()},   {Limits.ToStringPieceWise()}";

        private string BodyToString() => Denominator == new One() ? $"{Numerator}" : $"({Numerator})/({Denominator})";
    }
}
