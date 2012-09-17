using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GordiasClassLibrary.Headquarters
{
    using System;
    using System.Reflection;
    using Livet.Commands;
    using GordiasClassLibrary.Interfaces;
    using GordiasClassLibrary.Headquarters;

    /// <summary>
    /// Livetコマンド属性定義クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : System.Attribute
    {
        /// <summary>
        /// 構築処理
        /// </summary>
        /// <typeparam name="CommandType">コマンド実装クラスタイプ</typeparam>
        /// <param name="target">構築ターゲットクラス</param>
        public static void Construction<CommandType>(ITacticsCommand<CommandType> target)
        {
            var commands = target.Commands;
            Type targetType = target.GetType();
            MethodInfo[] methods = targetType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(CommandAttribute), false))
                {
                    var name = method.Name;

                    Type commandsType = commands.GetType();
                    PropertyInfo propertys = commandsType.GetProperty(name);
                    MethodInfo commandMethod = method;

                    propertys.SetValue(
                        commands,
                        new TacticsCommand(() =>
                        {
                            commandMethod.Invoke(target, null);
                        }),
                        null);
                }
            }
        }
    }
}
