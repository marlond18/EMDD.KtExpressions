using System;
using EMDD.KtExpressions;
using EMDD.KtExpressions.Expression;
using EMDD.KtExpressions.Limits;
using EMDD.KtExpressions.Terms;
using EMDD.KtNumerics;
using EMDD.KtPolynomials;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTest
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void Construction()
        {
            var poly1 = KtPolynomial.Create(4, 0, -2, 0, 4, 1);
            var poly2 = KtPolynomial.Create(3, -4.3, 1);
            var poly3 = KtPolynomial.Create(4, 3, 7);
            var denom = KtPolynomial.Create(1);
            var term1 = new DiscontinuousTerm(poly1, denom, Limit.Create(-20, -10));
            var term2 = new DiscontinuousTerm(poly2, denom, Limit.Create(-15, -5));
            Term term3 = poly3;
            var expression = new Expression(term1, term3, term2);
            var expected = new Expression(
                new DiscontinuousTerm(poly3, denom, ForwardLimit.Create(-20)),
                new DiscontinuousTerm(poly3 + poly1, denom, Limit.Create(-20, -15)),
                new DiscontinuousTerm(poly3 + poly1 + poly2, denom, Limit.Create(-15, -10)),
                new DiscontinuousTerm(poly3 + poly2, denom, Limit.Create(-10, -5)),
                new DiscontinuousTerm(poly3, denom, BackwardLimit.Create(-5)));
            Assert.AreEqual(expression, expected);
        }

        [TestMethod]
        public void Adding()
        {
            var poly1 = KtPolynomial.Create(4, 0, -2, 0, 4, 1);
            var poly2 = KtPolynomial.Create(3, -4.3, 1);
            var poly3 = KtPolynomial.Create(4, 3, 7);
            var denom = KtPolynomial.Create(1);
            var term1 = new DiscontinuousTerm(poly1, denom, Limit.Create(-20, -10));
            var term2 = new DiscontinuousTerm(poly2, denom, Limit.Create(-15, -5));
            Term term3 = poly3;
            var expression = new Expression(term1, term3, term2);
            var expected = new Expression(
                new DiscontinuousTerm(2 * poly3, denom, ForwardLimit.Create (-20)),
                new DiscontinuousTerm(2 * (poly3 + poly1), denom, Limit.Create(-20, -15)),
                new DiscontinuousTerm(2 * (poly3 + poly1 + poly2), denom, Limit.Create(-15, -10)),
                new DiscontinuousTerm(2 * (poly3 + poly2), denom, Limit.Create(-10, -5)),
                new DiscontinuousTerm(2 * poly3, denom, BackwardLimit.Create (-5)));
            Assert.AreEqual(expression + expression, expected);
        }

        [TestMethod]
        public void Subtracting()
        {
            var poly1 = KtPolynomial.Create(4, 0, -2, 0, 4, 1);
            var poly2 = KtPolynomial.Create(3, -4.3, 1);
            var poly3 = KtPolynomial.Create(4, 3, 7);
            var denom = KtPolynomial.Create(1);
            var term1 = new DiscontinuousTerm(poly1, denom, Limit.Create(-20, -10));
            var term2 = new DiscontinuousTerm(poly2, denom, Limit.Create(-15, -5));
            Term term3 = poly3;
            var expression = new Expression(term1, term3, term2);
            var actual = expression - expression;
            _ = new Zero();
            var expected = new Expression((Number)0);
            Assert.AreEqual(actual,expected );
        }
    }
}