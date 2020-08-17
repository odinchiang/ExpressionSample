using System;
using System.Linq.Expressions;

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
        /// 自行拼裝表達式目錄樹
        /// </summary>
        public static void Intro2()
        {

        }
    }
}
