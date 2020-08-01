using System.Linq.Expressions;

namespace DataAccess.Classes
{
    public class ParameterSubstitutionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _destination;
        private readonly ParameterExpression _source;

        public ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
        {
            _source = source;
            _destination = destination;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _source) ? _destination : base.VisitParameter(node);
        }
    }
}
