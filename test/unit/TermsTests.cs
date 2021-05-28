using System;
using System.Linq;
using EMDD.KtExpressions;
using EMDD.KtExpressions.Limits;
using EMDD.KtExpressions.Terms;
using EMDD.KtPolynomials;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTest
{
    [TestClass]
    public class TermsAddingTests
    {
        [TestMethod]
        public void ContinuousAndNot()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            Term term1 = poly3;
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create( -12, 20));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 3);
            Term expectedResult1 = new DiscontinuousTerm(poly3, KtPolynomial.Create(1), ForwardLimit.Create(-12));
            Assert.AreEqual(actualResult[0], expectedResult1);
            Term expectedResult2 = new DiscontinuousTerm((poly3 * poly2) + poly1, poly2, Limit.Create(-12, 20));
            Assert.AreEqual(actualResult[1], expectedResult2);
            Term expectedResult3 = new DiscontinuousTerm(poly3, KtPolynomial.Create(1), BackwardLimit.Create( 20));
            Assert.AreEqual(actualResult[2], expectedResult3);
            Assert.IsTrue(actualResult.SequenceEqual(term2 + term1));
        }

        [TestMethod]
        public void BothDiscontinuousBarelyInclusive()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 10));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(-12, 20));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 2);
            Term expectedResult2 = new DiscontinuousTerm((poly3 * poly2) + (poly1 * poly4), poly4 * poly2, Limit.Create(-12, 10));
            Assert.AreEqual(actualResult[0], expectedResult2);
            Term expectedResult3 = new DiscontinuousTerm(poly1, poly2, Limit.Create(10, 20));
            Assert.AreEqual(actualResult[1], expectedResult3);
            Assert.IsTrue(actualResult.SequenceEqual(term2 + term1));
        }

        [TestMethod]
        public void BothDiscontinuousTouching()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 10));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(10, 20));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 2);
            Assert.AreEqual(actualResult[0], term1);
            Assert.AreEqual(actualResult[1], term2);
        }

        [TestMethod]
        public void BothDiscontinuousNotTouching()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 9));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(10, 20));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 2);
            Assert.AreEqual(actualResult[0], term1);
            Assert.AreEqual(actualResult[1], term2);
        }
        [TestMethod]
        public void BothDiscontinuousOverlap()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 15));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(10, 20));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 3);
            Term term3 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 10));
            Term term4 = new DiscontinuousTerm((poly3 * poly2) + (poly1 * poly4), poly4 * poly2, Limit.Create(10, 15));
            Term term5 = new DiscontinuousTerm(poly1, poly2, Limit.Create(15, 20));
            Assert.AreEqual(actualResult[0], term3);
            Assert.AreEqual(actualResult[1], term4);
            Assert.AreEqual(actualResult[2], term5);
        }

        [TestMethod]
        public void BothDiscontinuousSpliceable()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4,ForwardLimit.Create( 9));
            Term term2 = new DiscontinuousTerm(poly3, poly4, BackwardLimit.Create( 9));
            var actualResult = term1 + term2;
            Assert.AreEqual(actualResult.Length, 1);
            Assert.AreEqual(actualResult[0], new ContinuousTerm(poly3, poly4));
        }

        [TestMethod]
        public void BothContinuous()
        {
            var poly1A = KtPolynomial.Create(4, 3, -24, 1);
            var poly1B = KtPolynomial.Create(1.134, 3, 33, -12);
            var poly2A = KtPolynomial.Create(-3, 45, 2, 4, 3);
            var poly2B = KtPolynomial.Create(8, 0, 4, -1);
            var cont1 = new ContinuousTerm(poly1A, poly1B);
            var cont2 = new ContinuousTerm(poly2A, poly2B);
            var num = (poly1A * poly2B) + (poly1B * poly2A);
            var denom = poly2B * poly1B;
            var actualResult = cont1 + cont2;

            Assert.AreEqual(actualResult.Length, 1);
            Assert.AreEqual(actualResult[0], new ContinuousTerm(num, denom));
        }
    }
    [TestClass]
    public class TermsSubtractingTest
    {
        [TestMethod]
        public void DifferentTypes()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            Term term1 = poly3;
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);

            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(-12, 20));
            var actualResult = term1 - term2;
            Assert.AreEqual(actualResult.Length, 3);
            Term expectedResult1 = new DiscontinuousTerm(poly3, KtPolynomial.Create(1), ForwardLimit.Create( -12));
            Assert.AreEqual(actualResult[0], expectedResult1);
            Term expectedResult2 = new DiscontinuousTerm((poly3 * poly2) - poly1, poly2, Limit.Create(-12, 20));
            Assert.AreEqual(actualResult[1], expectedResult2);
            Term expectedResult3 = new DiscontinuousTerm(poly3, KtPolynomial.Create(1),BackwardLimit.Create( 20));
            Assert.AreEqual(actualResult[2], expectedResult3);
            actualResult = term2 - term1; //reversed
            Assert.AreEqual(actualResult.Length, 3);
            expectedResult1 = new DiscontinuousTerm(-poly3, KtPolynomial.Create(1), ForwardLimit.Create( -12));
            Assert.AreEqual(actualResult[0], expectedResult1);
            expectedResult2 = new DiscontinuousTerm(poly1 - (poly3 * poly2), poly2, Limit.Create(-12, 20));
            Assert.AreEqual(actualResult[1], expectedResult2);
            expectedResult3 = new DiscontinuousTerm(-poly3, KtPolynomial.Create(1),BackwardLimit.Create( 20));
            Assert.AreEqual(actualResult[2], expectedResult3);
        }
        [TestMethod]
        public void BothContinuous()
        {
            var poly1A = KtPolynomial.Create(4, 3, -24, 1);
            var poly1B = KtPolynomial.Create(1.134, 3, 33, -12);
            var poly2A = KtPolynomial.Create(-3, 45, 2, 4, 3);
            var poly2B = KtPolynomial.Create(8, 0, 4, -1);
            var cont1 = new ContinuousTerm(poly1A, poly1B);
            var cont2 = new ContinuousTerm(poly2A, poly2B);
            var actualResult = cont1 - cont2;
            Assert.AreEqual(actualResult.Length, 1);
            Assert.AreEqual(actualResult[0], new ContinuousTerm((poly1A * poly2B) - (poly1B * poly2A), poly2B * poly1B));
        }
        [TestMethod]
        public void BothDiscontinuousBarelyInclusive()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 10));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(-12, 20));
            var actualResult = term1 - term2;
            Assert.AreEqual(actualResult.Length, 2);
            Term expectedResult2 = new DiscontinuousTerm((poly3 * poly2) - (poly1 * poly4), poly4 * poly2, Limit.Create(-12, 10));
            Assert.AreEqual(actualResult[0], expectedResult2);
            Term expectedResult3 = new DiscontinuousTerm(-poly1, poly2, Limit.Create(10, 20));
            Assert.AreEqual(actualResult[1], expectedResult3);
        }
        [TestMethod]
        public void BothDiscontinuousTouching()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            var poly4 = KtPolynomial.Create(3, 1);
            Term term1 = new DiscontinuousTerm(poly3, poly4, Limit.Create(-12, 10));
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(10, 20));
            var actualResult = term1 - term2;
            Assert.AreEqual(actualResult.Length, 2);
            Assert.AreEqual(actualResult[0], term1);
            Assert.AreEqual(actualResult[1], -term2);
        }
    }

    [TestClass]
    public class TermsMultiplyingTest
    {
        [TestMethod]
        public void MultTest()
        {
            var poly3 = KtPolynomial.Create(2, 1);
            Term term1 = poly3;
            var poly1 = KtPolynomial.Create(3, 5, 2, 5);
            var poly2 = KtPolynomial.Create(3, 12, -2);
            Term term2 = new DiscontinuousTerm(poly1, poly2, Limit.Create(-12, 20));
            var actualResult = term1 * term2;
            Term expectedResult2 = new DiscontinuousTerm(poly3 * poly1, poly2, Limit.Create(-12, 20));
            Assert.AreEqual(actualResult, expectedResult2);
        }
    }
}
