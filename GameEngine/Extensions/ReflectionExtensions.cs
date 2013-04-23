using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
                propertyInfo.SetValue(target, Convert.ChangeType(value, targetType), null);
        }

        public static void SetProperty(object target, string propertyName, object value)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(target, value, null);
        }
    }
}
