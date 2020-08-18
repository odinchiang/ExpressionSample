using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionSample
{
    public class ExpressionIntro
    {
        public static void Intro1()
        {
            // Lambda 實例化委派 => 方法，實例化委派的參數
            Func<int, int, int> func = (m, n) => m * n + 2;

            // Lambda 表達式聲明表達式目錄樹(快捷方式) => 資料結構
            // 表達式目錄樹就像聲明了多個變數及變數之間的操作及關係，需要的時候可以解開
            Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;

            // 表達式目錄樹不能有陳述式主體，只能是一列，亦即不能有大括號
            //Expression<Func<int, int, int>> exp = (m, n) =>
            //{
            //    return m * n + 2;
            //};

            int result1 = func.Invoke(12, 23);
            int result2 = exp.Compile().Invoke(12, 23); // exp.Compile() 之後就是一個委派

            Expression<Func<int>> expression = () => 123 + 234;
            // 利用 ILSpy 反編譯
            Expression<Func<int>> expression2 =
                Expression.Lambda<Func<int>>(Expression.Constant(357, typeof(int)), Array.Empty<ParameterExpression>());

            /*
                 new Expression<Func<int, int, int>> {
                    NodeType = ExpressionType.Lambda,
                    Type = typeof(Func<int, int, int>),
                    Parameters = new ReadOnlyCollection<ParameterExpression> {
                        new ParameterExpression {
                            Type = typeof(int),
                            IsByRef = false,
                            Name = "m"
                        },
                        new ParameterExpression {
                            Type = typeof(int),
                            IsByRef = false,
                            Name = "n"
                        }
                    },
                    Body = new BinaryExpression {
                        NodeType = ExpressionType.Add,
                        Type = typeof(int),
                        Left = new BinaryExpression {
                            NodeType = ExpressionType.Multiply,
                            Type = typeof(int),
                            Left = new ParameterExpression {
                                Type = typeof(int),
                                IsByRef = false,
                                Name = "m"
                            },
                            Right = new ParameterExpression {
                                Type = typeof(int),
                                IsByRef = false,
                                Name = "n"
                            }
                        },
                        Right = new ConstantExpression {
                            Type = typeof(int),
                            Value = 2
                        }
                    },
                    ReturnType = typeof(int)
                 }
             */
        }

        /// <summary>
        /// 手動拼裝表達式目錄樹(常數)
        /// </summary>
        public static void Intro2()
        {
            //Expression<Func<int>> expression = () => 123 + 234;

            ConstantExpression right = Expression.Constant(234);
            ConstantExpression left = Expression.Constant(123);
            BinaryExpression plus = Expression.Add(left, right); // 123 + 234
            Expression<Func<int>> expression = Expression.Lambda<Func<int>>(plus, new ParameterExpression[] { }); // () => 123 + 234
            int iResult = expression.Compile().Invoke();
        }

        /// <summary>
        /// 手動拼裝表達式目錄樹(變數)
        /// </summary>
        public static void Intro3()
        {
            {
                Expression<Func<int, int, int>> exp = (m, n) => m * n + m + n + 2;

                // 反編譯
                ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "m");
                ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int), "n");
                Expression<Func<int, int, int>> expression = Expression.Lambda<Func<int, int, int>>(Expression.Add(Expression.Add(Expression.Add(Expression.Multiply(parameterExpression, parameterExpression2), parameterExpression), parameterExpression2), Expression.Constant(2, typeof(int))), new ParameterExpression[2]
                {
                    parameterExpression,
                    parameterExpression2
                });
            }
            {
                //Expression<Func<int, int, int>> exp = (m, n) => m * n + m + n + 2;

                var m = Expression.Parameter(typeof(int), "m"); // 變數
                var n = Expression.Parameter(typeof(int), "n"); // 變數
                var constant = Expression.Constant(2, typeof(int)); // 常數
                var multiply = Expression.Multiply(m, n);
                var plus1 = Expression.Add(multiply, m);
                var plus2 = Expression.Add(plus1, n);
                var plus3 = Expression.Add(plus2, constant);
                var expression = Expression.Lambda<Func<int, int, int>>(plus3, new List<ParameterExpression>()
                {
                    m, n
                });
                int iResult = expression.Compile().Invoke(3, 1);
            }
        }

        /// <summary>
        /// 手動拼裝表達式目錄樹
        /// </summary>
        public static void Intro4()
        {
            //Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");

            ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
            var constantExp = Expression.Constant("5");
            var field = typeof(People).GetField("Id");
            var fieldExp = Expression.Field(parameterExpression, field); // x.Id
            var toString = typeof(int).GetMethod("ToString", new Type[] { });
            var toStringExp = Expression.Call(fieldExp, toString, Array.Empty<Expression>());
            var equals = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });
            var equalsExp = Expression.Call(toStringExp, equals, constantExp);

            Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(equalsExp, new ParameterExpression[]
            {
                    parameterExpression
            });

            bool result = expression.Compile().Invoke(new People() { Id = 5, Name = "Mark", Age = 20 });
        }

        /// <summary>
        /// 表達式目錄樹的用途是為了"動態"
        /// </summary>
        public static void Intro5()
        {
            // ADO.NET 時 WHERE 條件常用判斷條件並拼接字串的方式組合 SQL 語句
            // 在 LinQ 中則可使用表達式樹進行組合

            Expression<Func<People, bool>> lambda = x => x.Name.Contains("Mark") && x.Age > 5;

            //ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
            //Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(Expression.AndAlso(Expression.Call(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/), Expression.Constant("Mark", typeof(string))), Expression.GreaterThan(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), Expression.Constant(5, typeof(int)))), new ParameterExpression[1]
            //{
            //    parameterExpression
            //});
        }
    }
}
