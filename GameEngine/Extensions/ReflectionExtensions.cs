using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace GameEngine.Extensions
{
    public class ReflectionExtensions
    {
        public static object GetProperty(object target, string propertyName)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            return propertyInfo.GetValue(target, null);
        }

        public static void SmartSetProperty(object target, string propertyName, string value)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(target, SmartConvert(value, propertyInfo.PropertyType), null);
        }

        public static void SetProperty(object target, string propertyName, object value)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(target, value, null);
        }

        public static object SmartConvert(string value, Type target)
        {
            if (target.IsEnum)
                return Enum.Parse(target, value);
            else
                return (target == typeof(Color)) ? ColorExtensions.ToColor(value) : Convert.ChangeType(value, target);
        }
    }
}
