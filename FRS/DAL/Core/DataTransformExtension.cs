using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL.Core
{
    public static class DataTransformExtension
    {

        public static T TransformTo<T>(this object data) where T : class, new()
        {
            if (data == null)
                return null;

            var dataType = data.GetType();

            var targetType = typeof(T);
            var targetTypeProperties = targetType.GetProperties();

            var instance = Activator.CreateInstance(targetType);
            var transformedData = instance as T;

            foreach (var targetTypeProperty in targetTypeProperties)
            {
                if (!targetTypeProperty.CanWrite)
                    continue;

                var theProperty = dataType.GetProperty(targetTypeProperty.Name);

                if (theProperty == null)
                    continue;

                if (!theProperty.CanRead)
                    continue;

                if (theProperty.PropertyType != targetTypeProperty.PropertyType)
                    continue;

                if (targetTypeProperty.GetGetMethod().IsVirtual || theProperty.GetGetMethod().IsVirtual)
                    continue;

                targetTypeProperty.SetValue(transformedData, theProperty.GetValue(data));
            }

            return transformedData;
        }

        public static void CopyFrom<T1, T2>(this T1 dest, T2 source)
            where T1 : class
            where T2 : class
        {
            if (source == null)
                return;

            if (dest == null)
                return;

            var dataType = source.GetType();
            var dataTypeProperties = dataType.GetProperties();
            var targetType = dest.GetType();

            foreach (var dataTypeProperty in dataTypeProperties)
            {
                if (!dataTypeProperty.CanRead)
                    continue;

                var theProperty = targetType.GetProperty(dataTypeProperty.Name);

                if (theProperty == null)
                    continue;

                if (!theProperty.CanWrite)
                    continue;

                if (theProperty.PropertyType != dataTypeProperty.PropertyType)
                    continue;

                if (dataTypeProperty.GetGetMethod().IsVirtual || theProperty.GetGetMethod().IsVirtual)
                    continue;

                theProperty.SetValue(dest, dataTypeProperty.GetValue(source));
            }
        }

        public static T DeserializeXML<T>(this string input)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }
    }
}
