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
            {
                if (target == typeof(Color))
                    return ColorExtensions.ToColor(value);
                else if (target == typeof(Vector2))
                {
                    string[] tokens = value.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 2)
                        throw new InvalidCastException(string.Format("Cannot convert '{}' to Vector2", value));
                    else
                        return new Vector2((float)Convert.ToDouble(tokens[0]), (float)Convert.ToDouble(tokens[1]));
                }
                else return Convert.ChangeType(value, target);
            }
        }
    }
}
