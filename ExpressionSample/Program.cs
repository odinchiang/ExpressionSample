using System;
using System.Linq.Expressions;

namespace ExpressionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ExpressionIntro.Intro1();

            Expression firstArg = Expression.Constant(2);
            Expression secondArg = Expression.Constant(3);
            Expression add = Expression.Add(firstArg, secondArg);
            Console.WriteLine(add); // (2 + 3)

            Func<int> compiled = Expression.Lambda<Func<int>>(add).Compile();
            Console.WriteLine(compiled()); // 5

            Expression<Func<int>> expr = () => 1 + 5;
            //Expression<Func<int>> value = Expression.Lambda<Func<int>>(Expression.Constant(6, typeof(int)), Array.Empty<ParameterExpression>());
            Console.WriteLine(expr); // () => 6
        }
    }
}
