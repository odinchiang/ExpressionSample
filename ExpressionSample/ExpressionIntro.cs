using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

using ExpressionSample.MappingExtend;
using ExpressionSample.Models;

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

        // 表達式目錄樹的用途是為了"動態"
        // ADO.NET 時 WHERE 條件常用判斷條件並拼接字串的方式組合 SQL 語句
        // 在 LinQ 中若想用類似拼接的方式
        // 1. 表達式目錄樹的擴展，擴展 AND 和 OR，基於 Visitor
        // 2. 根據字串 + 條件自動拼裝起來

        /// <summary>
        /// 根據字串 + 條件自動拼裝起來 (此處尚未封裝)
        /// </summary>
        public static void Intro5()
        {
            // 可以封裝一個表達式目錄樹的自動生成，根據使用者介面的輸入

            Expression<Func<People, bool>> lambda = x => x.Name.Contains("Mark") && x.Age > 5;

            ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");

            // if(name 不為空)
            var name = typeof(People).GetProperty("Name");
            var mark = Expression.Constant("Mark", typeof(string));
            var nameExp = Expression.Property(parameterExpression, name);
            var contains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            var containsExp = Expression.Call(nameExp, contains, new Expression[] { mark });

            // if(age 不為空)
            var age = typeof(People).GetProperty("Age");
            var age5 = Expression.Constant(5, typeof(int));
            var ageExp = Expression.Property(parameterExpression, age);
            var greaterThanExp = Expression.GreaterThan(ageExp, age5);

            var body = Expression.AndAlso(containsExp, greaterThanExp);

            Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(body, new ParameterExpression[1]
            {
                parameterExpression
            });

            bool result = expression.Compile().Invoke(new People()
            {
                Id = 333,
                Name = "Mark",
                Age = 20
            });
        }

        /*
         類型轉換的方法

         1. 傳統的類型轉換 (硬編碼，性能最佳)

            People people = new People()
            {
                Id = 11, Name = "Mark", Age = 20
            };

            PeopleDto peopleDto = new PeopleDto()
            {
                Id = people.Id, Name = people.Name, Age = people.Age
            };
         
         2. 用反射 + 泛型實現封裝類型轉換 (性能較差)
         3. 序列化反序列化 (性能最差，但序列化反序列化主要不是用在此)
         4. 表達式目錄樹 (性能比傳統略差一點，但可加入快取機制，動態生成硬編碼，比 Automapper 性能好)
        
         思路：用委派封裝傳統的類型轉換方式
            Func<People, PeopleDto> func = x => new PeopleDto()
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.Age
            };

            PeopleDto peopleDto = func.Invoke(new People()
            {
                Id = 323, Name = "Mary", Age = 18
            });

         思路：動態拼裝委派並快取這個委派，之後再次轉換就沒有性能損耗
            Expression<Func<People, PeopleDto>> lambda = p => new PeopleDto()
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age
            };
         */

        /// <summary>
        /// 表達式目錄樹實現類型轉換 (動態拼裝委派)
        /// </summary>
        public static void MappingTest()
        {
            People people = new People()
            {
                Id = 11,
                Name = "Mark",
                Age = 31
            };
            long common = 0;
            long generic = 0;
            long cache = 0;
            long reflection = 0;
            long serialize = 0;
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1_000_000; i++)
                {
                    PeopleDto peopleDto = new PeopleDto()
                    {
                        Id = people.Id,
                        Name = people.Name,
                        Age = people.Age
                    };
                }
                watch.Stop();
                common = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1_000_000; i++)
                {
                    PeopleDto peopleCopy = ReflectionMapper.Trans<People, PeopleDto>(people);
                }
                watch.Stop();
                reflection = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1_000_000; i++)
                {
                    PeopleDto peopleCopy = SerializeMapper.Trans<People, PeopleDto>(people);
                }
                watch.Stop();
                serialize = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1_000_000; i++)
                {
                    PeopleDto peopleCopy = ExpressionMapper.Trans<People, PeopleDto>(people);
                }
                watch.Stop();
                cache = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1_000_000; i++)
                {
                    PeopleDto peopleCopy = ExpressionGenericMapper<People, PeopleDto>.Trans(people);
                }
                watch.Stop();
                generic = watch.ElapsedMilliseconds;
            }

            Console.WriteLine($"common = { common} ms");
            Console.WriteLine($"reflection = { reflection} ms");
            Console.WriteLine($"serialize = { serialize} ms");
            Console.WriteLine($"cache = { cache} ms");
            Console.WriteLine($"generic = { generic} ms");
        }
    }
}
