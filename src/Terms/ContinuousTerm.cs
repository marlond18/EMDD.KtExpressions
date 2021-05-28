using EMDD.KtExpressions.Limits;
using EMDD.KtPolynomials;
using System;

namespace EMDD.KtExpressions.Terms
{
    public class ContinuousTerm : Term
    {
        public ContinuousTerm(KtPolynomial numerator, KtPolynomial denominator) : base(numerator, denominator, new Limitless()) { }

        public ContinuousTerm(KtPolynomial polynomial) : base(polynomial, new Limitless()) { }

        public override Term[] AddWith(Term other) => other switch
        {
            ContinuousTerm term => CombineValues(
                () => TermHelper.AddValues(this, term),
                c => new Term[] { new ContinuousTerm(c.Numerator, c.Denominator) }),
            _ => other + this
        };

        private static T2 CombineValues<T1, T2>(Func<T1> func1, Func<T1, T2> func2) => func2(func1());

        public override Term Invert() => new ContinuousTerm(Denominator, Numerator);

        public override Term Negate() => new ContinuousTerm(-Numerator, Denominator);

        public override Term MultiplyWith(Term other) => other switch
        {
            ContinuousTerm ct => CombineValues(
                () => TermHelper.MultiplyValues(this, other),
                c => new ContinuousTerm(c.Numerator, c.Denominator)),
            _ => other * this
        };

        public override Term Clone() => new ContinuousTerm(Numerator, Denominator);

        public override string ToString() => Denominator == new One() ? Numerator.ToString() : $"({Numerator})/({Denominator})";
        public override string ToStringPieceWise() => ToString();
    }
}