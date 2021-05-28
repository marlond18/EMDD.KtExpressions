using System;
using System.Linq;
using EMDD.KtPolynomials;
using EMDD.KtNumerics;
using static EMDD.KtExpressions.Expression.ExpressionHelper;
using System.Collections.Generic;
using KtExtensions;
using EMDD.KtExpressions.Limits;
using EMDD.KtExpressions.Terms;

namespace EMDD.KtExpressions.Expression
{
    public class Expression
    {
        public Expression(params Term[] data)
        {
            if (data == null || data.Length < 1) data = new Term[] { 0 };
            Terms = MergeTermData(data).Select(elem => elem.Clone()).OrderBy(elem => elem).ToArray();
        }

        public Expression(params ((KtPolynomial numerator, KtPolynomial denominator) expression, LimitBase limits)[] data) : this(data.Select(datum => TermHelper.Create(datum.expression.numerator, datum.expression.denominator, datum.limits)).ToArray()) { }

        public Expression(params (KtPolynomial polynomial, LimitBase limits)[] data) : this(data.Select(datum => TermHelper.Create(datum.polynomial, datum.limits)).ToArray()) { }

        public Number Evaluate(Number domain) => Terms.Find(term => domain.IsWithin(term.Limits.Lower, term.Limits.Upper))?.InnerEvaluate(domain) ?? 0;

        public IEnumerable<(double location, Number result)> EvaluateOnRange(int division, LimitBase limits)
        {
            if (limits.IsContinuous) yield break;
            var interval = limits.Distance / division;
            for (int i = 0; i < division + 1; i++)
            {
                var location = (i * interval) + limits.Lower;
                yield return (location, Evaluate(location));
            }
        }

        public IEnumerable<(double location, Number result)> EvaluateOnEqualInterval(int division)
        {
            var ul = Terms.Select(t => t.Limits).Aggregate((agg, next) => agg.Upper > next.Upper ? agg : next);
            var ll = Terms.Select(t => t.Limits).Aggregate((agg, next) => agg.Upper < next.Upper ? agg : next);
            return EvaluateOnRange(division, ll.Span(ul));
        }

        public IEnumerable<IEnumerable<(double location, Number result)>> EvaluateEachTerm(int division) => Limits().Select(l => EvaluateOnRange(division, l));

        public LimitBase[] Limits() => Terms.Select(t => t.Limits).ToArray();

        internal Term[] Terms { get; }

        public static implicit operator Expression(long val) => (Term)val;

        public static implicit operator Expression(double val) => (Term)val;

        public static implicit operator Expression(Number val) => (Term)val;

        public static implicit operator Expression(KtPolynomial val) => (Term)val;

        public static implicit operator Expression(Term val) => new(val);

        public static bool operator ==(Expression a, Expression b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Terms.Length == b.Terms.Length && a.Terms.SequenceEqual(b.Terms);
        }

        public static bool operator !=(Expression a, Expression b) => !(a == b);

        public bool Equals(Expression other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (this is null || other is null) return false;
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (this is null || obj is null) return false;
            return obj is Expression exp && Equals(exp);
        }

        public override int GetHashCode() => Terms?.Aggregate(1, (final, elem) => final * elem.GetHashCode()) ?? 0;

        public static Expression operator -(Expression a) => a is null ? null : new Expression(a.Terms.Select(term => -term).ToArray());

        public static Expression operator +(Expression a, Expression b)
        {
            if (a is null && b is null) return null;
            if (b is null) return a;
            if (a is null) return b;
            return new Expression(a.Terms.Concat(b.Terms).ToArray());
        }

        public static Expression operator -(Expression a, Expression b) => a + -b;

        public static Expression operator *(Expression a, Expression b) => a is null || b is null ? null : GetNewExpression(a, b, (term1, term2) => term1 * term2);

        private static Expression GetNewExpression(Expression a, Expression b, Func<Term, Term, Term> func) => new(MergeTermData((from termA in a.Terms from termB in b.Terms select func(termA, termB)).ToArray()));

        public static Expression operator /(Expression a, Expression b) => a is null || b is null ? null : GetNewExpression(a, b, (term1, term2) => term1 / term2);

        public Expression Clone() => new(Terms);

        public override string ToString() => Terms.Aggregate("", (str, term) => $"{str}{(str?.Length == 0 ? "" : "+")}{term}");

        public string ToStringPieceWise() => "{█(" + Terms.Aggregate("", (str, term) => $"{str}{(str?.Length == 0 ? "" : "@")}{term.ToStringPieceWise()}") + ")┤";
    }
}