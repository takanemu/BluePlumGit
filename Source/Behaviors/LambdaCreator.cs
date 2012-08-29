using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Livet.Behaviors
{
    /// <summary>
    /// ラムダ式を生成するヘルパークラスです。
    /// </summary>
    public static class LambdaCreator
    {
        private static MethodInfo FindMethodInfoLoose(Type targetType, string methodName, Type returnType, params Type[] parameterTypes)
        {
            return targetType.GetMethods()
                .FirstOrDefault(method =>
                {
                    if (method.Name != methodName) return false;

                    var parameters = method.GetParameters();

                    if (parameters.Length != parameterTypes.Length) return false;

                    var paraResult = parameterTypes.Zip(parameters, Tuple.Create).All(x => x.Item1.IsAssignableFrom(x.Item2.ParameterType));

                    if (!paraResult) return false;

                    return returnType.IsAssignableFrom(method.ReturnType);
                });
        }

        /// <summary>
        /// 指定された型の指令された引数のないメソッドを実行するラムダ式を生成します。
        /// </summary>
        /// <typeparam name="TTargetObject">メソッドを呼び出す型</typeparam>
        /// <param name="targetRealType">TTargetObjectと異なる型が指定された場合、ここで指定された型にキャストしてからメソッドの呼び出しを行うラムダ式を生成します。</param>
        /// <param name="methodName">呼び出すメソッドの名前</param>
        /// <param name="ifNotFoundThrowException">対象のメソッドが見つからなかった場合、例外をスローするかどうかを指定します。</param>
        /// <returns>target => (([targetRealType])target).MethodName</returns>
        public static Action<TTargetObject> CreateActionLambda<TTargetObject>(Type targetRealType, string methodName, bool ifNotFoundThrowException)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");

            var targetType = typeof(TTargetObject);

            var methodInfo = FindMethodInfoLoose(targetRealType, methodName, typeof(void));

            if (methodInfo == null)
            {
                if (ifNotFoundThrowException)
                    throw new ArgumentException(string.Format(
                    "{0} 型に 引数を持たないメソッド {1} が見つかりません。",
                    targetType.Name,
                    methodName));

                return null;
            }

            var paraTarget = Expression.Parameter(targetType, "target");

            var realTargetType = methodInfo.DeclaringType;

            return Expression.Lambda<Action<TTargetObject>>
                        (
                            Expression.Call
                                (
                                    paraTarget.Type == realTargetType ? (Expression)paraTarget : Expression.Convert(paraTarget, realTargetType),
                                    methodInfo
                                ),
                                paraTarget
                        ).Compile();
        }

        /// <summary>
        /// 指定された型の指令された引数のが一つだけあるメソッドを実行するラムダ式を生成します。
        /// </summary>
        /// <typeparam name="TTargetObject">メソッドを呼び出す型</typeparam>
        /// <typeparam name="TArgument">メソッドの引数の型</typeparam>
        /// <param name="targetRealType">TTargetObjectと異なる型が指定された場合、ここで指定された型にキャストしてからメソッドの呼び出しを行うラムダ式を生成します。</param>
        /// <param name="methodName">呼び出すメソッドの名前</param>
        /// <param name="ifNotFoundThrowException">対象のメソッドが見つからなかった場合、例外をスローするかどうかを指定します。</param>
        /// <returns>target => ((targetRealType)target).MethodName(argument)</returns>
        public static Action<TTargetObject, TArgument> CreateActionLambda<TTargetObject, TArgument>(Type targetRealType, string methodName, bool ifNotFoundThrowException)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");

            var targetType = typeof(TTargetObject);
            var argumentType = typeof(TArgument);

            var methodInfo = FindMethodInfoLoose(targetRealType, methodName, typeof(void), argumentType);

            if (methodInfo == null)
            {
                if (ifNotFoundThrowException)
                    throw new ArgumentException(string.Format(
                    "{0} 型に {1} 型の引数を一つだけ持つメソッド {2} が見つかりません。",
                    targetType.Name,
                    argumentType.Name,
                    methodName));

                return null;
            }

            var paraTarget = Expression.Parameter(targetType, "target");
            var paraMessage = Expression.Parameter(argumentType, "argument");

            var realTargetType = methodInfo.DeclaringType;
            var realArgumentType = methodInfo.GetParameters()[0].ParameterType;

            return Expression.Lambda<Action<TTargetObject, TArgument>>
                        (
                            Expression.Call
                                (
                                    paraTarget.Type == realTargetType ? (Expression)paraTarget : Expression.Convert(paraTarget, realTargetType),
                                    methodInfo,
                                    paraMessage.Type == realArgumentType ? (Expression)paraMessage : Expression.Convert(paraMessage, realArgumentType)
                                ),
                                paraTarget,
                                paraMessage
                        ).Compile();
        }
    }

}
