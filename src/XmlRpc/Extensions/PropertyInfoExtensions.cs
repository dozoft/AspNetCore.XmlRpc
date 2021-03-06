﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AspNetCore.XmlRpc.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetSerializationName(
            this PropertyInfo instance)
        {
            return
                instance.GetCustomAttributes(
                    typeof(DataMemberAttribute),
                    true).
                        Cast<DataMemberAttribute>().
                        Where(
                            dma =>
                            !string.IsNullOrWhiteSpace(
                                dma.Name)).
                        Select(dma => dma.Name).
                        FirstOrDefault()
                            ?? instance.Name;
        }

        public static void SetValue(
            this PropertyInfo instance,
            object obj,
            object value)
        {
            try
            {
                //instance.SetValue(obj, value, null);
                instance.SetValue(obj, Convert.ChangeType(value, instance.PropertyType), null);
            }
            catch
            {
                if (instance.PropertyType.IsArray)
                {
                    var source = (object[])value;
                    var destinationType = 
                        instance.PropertyType.GetElementType();

                    var array =
                        Array.CreateInstance(
                            destinationType,
                            source.Length);

                    Array.Copy(source, array, source.Length);

                    instance.SetValue(obj, array, null);    
                }
                else
                {
                    instance.SetValue(obj, value.ToString(), null);
                }
            }
        }
    }
}
