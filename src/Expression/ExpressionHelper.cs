using EMDD.KtExpressions.Terms;

using System.Collections.Generic;
using System.Linq;

namespace EMDD.KtExpressions.Expression
{
    public static class ExpressionHelper
    {
        public static Term[] MergeTermData(Term[] data)
        {
            var tempData = new List<Term>();
            if (data == null) return tempData.ToArray();
            var dataCopy = data.ToList();
            while (dataCopy.Count > 0)
            {
                var term = dataCopy[0];
                dataCopy.RemoveAt(0);
                if (term == 0) continue;
                if (TermIsIsolated(dataCopy, term)) tempData.Add(term);
            }
            return tempData.Count < 1 ? new Term[] { 0 } : tempData.ToArray();
        }

        private static bool TermIsIsolated(List<Term> dataCopy, Term initTerm)
        {
            for (var i = 0; i < dataCopy.Count; i++)
            {
                var term = dataCopy[i];
                if (initTerm.AffectedBy(term))
                {
                    dataCopy.RemoveAt(i);
                    dataCopy.AddRange(initTerm + term);
                    return false;
                }
            }
            return true;
        }
    }
}