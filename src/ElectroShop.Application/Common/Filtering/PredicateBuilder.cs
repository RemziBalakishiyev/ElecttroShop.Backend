using System.Linq.Expressions;

namespace ElectroShop.Application.Common.Filtering;

/// <summary>
/// Predicate Builder - Dinamik LINQ expression builder
/// Bütün entity-lər üçün universal
/// </summary>
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() => x => true;
    public static Expression<Func<T, bool>> False<T>() => x => false;

    /// <summary>
    /// AND əməliyyatı
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left!, right!),
            parameter);
    }

    /// <summary>
    /// OR əməliyyatı
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(left!, right!),
            parameter);
    }

    /// <summary>
    /// Conditional AND - yalnız condition true olarsa əlavə edir
    /// </summary>
    public static Expression<Func<T, bool>> AndIf<T>(
        this Expression<Func<T, bool>> expr,
        bool condition,
        Expression<Func<T, bool>> predicateToAdd)
    {
        return condition ? expr.And(predicateToAdd) : expr;
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}


