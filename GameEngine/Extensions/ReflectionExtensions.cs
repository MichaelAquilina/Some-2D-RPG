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

            Type targetType = propertyInfo.PropertyType;

            if (targetType.IsEnum)
                propertyInfo.SetValue(target, Enum.Parse(targetType, value), null);
            else
            {
                object convertedValue = (targetType == typeof(Color)) ? ColorExtensions.ToColor(value) : Convert.ChangeType(value, targetType);

                propertyInfo.SetValue(target, convertedValue , null);
            }
        }

        public static void SetProperty(object target, string propertyName, object value)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(target, value, null);
        }
    }
}
