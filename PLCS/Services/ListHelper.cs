using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PLCS.Models
{
    public class ListHelper
    {
        public static List<T> ConvertDataTableToList<T>(DataTable table)
        {
            var list = new List<T>();



            var classProperties = typeof(T).GetProperties();


            foreach (DataRow row in table.Rows)
            {
                //有些時候linq可讀性太差,不利於維護.不要爲了用linq而用linq
                //foreach (var classProperty in from classProperty in classProperties where table.Columns.Contains(classProperty.Name) where classProperty.CanWrite let value = row[classProperty.Name] select classProperty)
                //{
                //    classProperty.SetValue(tmpClass, row[classProperty.Name] is DBNull ? null : row[classProperty.Name]);
                //} 
                
                var tmpClass = (T)typeof(T).Assembly.CreateInstance(typeof(T).FullName);

                foreach (var classProperty in classProperties)
                {
                    if (table.Columns.Contains(classProperty.Name))
                    {
                        if (!classProperty.CanWrite)
                        {
                            continue;
                        }
                        classProperty.SetValue(tmpClass,
                            row[classProperty.Name] is DBNull ? null : row[classProperty.Name]);
                    }
                    
                }
                list.Add(tmpClass);
            }


            return list;

        }



    }
}