using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class ListHelper
    {
        public static List<T> ConvertDataTableToList<T>(DataTable table,
            ConvertTypeEum compareType = ConvertTypeEum.ByIndexName, int startIndex = 0)
        {
            if (table == null)
            {
                return null;
            }

            var list = new List<T>();
            var classProperties = typeof(T).GetProperties();
            foreach (DataRow row in table.Rows)
            {
                #region About Linq

                //Linq確實好強大而且優雅,可是有些時候linq可讀性太差,不利於調試及維護.不要爲了用linq而用linq.
                //當然,也有可能本人眼殘~TT.TT
                //foreach (var classProperty in from classProperty in classProperties where table.Columns.Contains(classProperty.Name) where classProperty.CanWrite let value = row[classProperty.Name] select classProperty)
                //{
                //    classProperty.SetValue(tmpClass, row[classProperty.Name] is DBNull ? null : row[classProperty.Name]);
                //} 

                #endregion

                var tmpClass = (T)typeof(T).Assembly.CreateInstance(typeof(T).FullName);

                if (compareType == ConvertTypeEum.ByIndexNumber)
                {
                    for (int i = 0; i < classProperties.Length; i++)
                    {
                        if (classProperties[i].PropertyType == typeof(Guid))
                        {
                            classProperties[i].SetValue(tmpClass,
                                                       new Guid(row.ItemArray[i + startIndex] is DBNull ? null : row.ItemArray[i + startIndex].ToString()));
                        }
                        else
                        {
                            classProperties[i].SetValue(tmpClass,
                                                       row.ItemArray[i + startIndex] is DBNull ? null : row.ItemArray[i + startIndex]);
                        }

                    }
                }
                else
                {
                    foreach (var classProperty in classProperties)
                    {
                        if (table.Columns.Contains(classProperty.Name))
                        {
                            if (!classProperty.CanWrite)
                            {
                                continue;
                            }
                            if (classProperty.PropertyType == typeof (Guid))
                            {
                                classProperty.SetValue(tmpClass,
                                  new Guid(row[classProperty.Name] is DBNull ? null : row[classProperty.Name].ToString()));
                            }
                            else { 
                            classProperty.SetValue(tmpClass,
                                row[classProperty.Name] is DBNull ? null : row[classProperty.Name]);
                            }
                        }

                    }
                }


                list.Add(tmpClass);
            }


            return list;

        }

        public static bool ClassIsEmptyOrNull(object obj)
        {
            var tmpBool = true;
            var protities = obj.GetType().GetProperties();
            foreach (var protity in protities)
            {
                if (protity.GetValue(obj) != null && (string)protity.GetValue(obj) != "")
                {
                    tmpBool = false;
                }
            }

            return tmpBool;
        }

    }
}