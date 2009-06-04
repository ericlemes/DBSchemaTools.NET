using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace DBInfo.Core{
	
	public class TypeUtility {

		public static bool IsNullableType(Type t) {
			return System.Nullable.GetUnderlyingType(t) != null;
		}

		public static bool InheritsFromOrIsNullableThatInheritsFrom(Type t, Type TypeToCompare) {
      if (InheritsFrom(t, TypeToCompare))
				return true;

			if (System.Nullable.GetUnderlyingType(t) == null)
				return false;

      return InheritsFrom(System.Nullable.GetUnderlyingType(t), TypeToCompare);
		}

		public static bool Implements(Type t, Type InterfaceTypeToCheck) {
			foreach (Type i in t.GetInterfaces()) {
        if (i == InterfaceTypeToCheck)
					return true;
			}
			return false;
		}

		
		public static List<PropertyInfo> getPropertiesbyAttribute(object source, List<Type> attribute) {
			List<PropertyInfo> SelObj = new List<PropertyInfo>();
			bool itemOK = true;
			foreach (PropertyInfo item in source.GetType().GetProperties()) {
				itemOK = true;
				foreach (Type att in attribute) {
					if (item.GetCustomAttributes(att, true).Length == 0) {
						itemOK = false;
					}
				}
				if (itemOK) {
					SelObj.Add(item);
				}
			}
			return SelObj;
		}

    public static bool InheritsFrom(Type t, Type TypeToCompare) {
      if (t == TypeToCompare)
        return true;
      else {
        if (t.BaseType == null || t.BaseType == typeof(object))
          return false;
        else
          return InheritsFrom(t.BaseType, TypeToCompare);
      }
    }

	}
}
