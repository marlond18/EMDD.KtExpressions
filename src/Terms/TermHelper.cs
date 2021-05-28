using System;
using System.Linq;
using EMDD.KtPolynomials;
using EMDD.KtExpressions.Limits;

namespace EMDD.KtExpressions.Terms
{
    internal static class TermHelper
    {
        public static (KtPolynomial Numerator, KtPolynomial Denominator) AddValues(Term a, Term b) => ((a.Numerator * b.Denominator) + (b.Numerator * a.Denominator), a.Denominator * b.Denominator);

        public static (KtPolynomial Numerator, KtPolynomial Denominator) MultiplyValues(Term a, Term b) => (a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Term Create(KtPolynomial numerator, KtPolynomial denominator, LimitBase limits)
        {
            return limits is Limitless || numerator is Zero ? (Term)new ContinuousTerm(numerator, denominator) : new DiscontinuousTerm(numerator, denominator, limits);
        }

        public static Term Create(KtPolynomial polynomial, LimitBase limits)
        {
            return limits is Limitless || polynomial is Zero ? new ContinuousTerm(polynomial) : new DiscontinuousTerm(polynomial, limits);
        }

        //public static (double Lower, double Upper)[] UniteLimits(Term a, Term b)
        //{
        //    var sortedLimits = new[] { a.LowerLimit, a.UpperLimit, b.LowerLimit, b.UpperLimit }.Distinct().ToArray();
        //    Array.Sort(sortedLimits);
        //    return sortedLimits.SelectPair((first, second) => (first, second)).ToArray();
        //}

        public static bool TermsAreSpliceable(Term a, Term b) => EqualVal(a, b) && a.Limits.Spliceable(b.Limits);

        public static bool EqualVal(Term a, Term b) => a.Numerator == b.Numerator && a.Denominator == b.Denominator;

        //public static bool HasContinuousLimits(Term a, Term b) => a.UpperLimit.NearEqual(b.LowerLimit) || b.UpperLimit.NearEqual(a.LowerLimit);
    }
}
