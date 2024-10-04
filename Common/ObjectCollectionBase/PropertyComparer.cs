using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace SystemFramework.Common.ObjectCollectionBase
{
   internal class PropertyComparer<TKey> :IComparer<TKey>
   {
      private PropertyDescriptor _property;
      private ListSortDirection _direction;

      public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
      {
         _property = property;
         _direction = direction;
      }

      public int Compare(TKey xVal, TKey yVal)
      {
         //Get property values
         object xValue = GetPropertyValue(xVal, _property.Name);
         object yValue = GetPropertyValue(yVal, _property.Name);

         //Determine sort order
         if (_direction == ListSortDirection.Ascending)
         {
            return CompareAscending(xValue, yValue);
         }
         else
         {
            return CompareDescending(xValue, yValue);
         }
      }

      public bool Equals(TKey xVal, TKey yVal)
      {
         return xVal.Equals(yVal);
      }

      public int GetHashCode(TKey obj)
      {
         return obj.GetHashCode();
      }

      // Compare two property values of any type
      private int CompareAscending(object xValue, object yValue)
      {
         int result = 0;
         if (xValue != null)
         {
            //If values implement IComparer
            if (xValue is IComparable)
            {
               result = ((IComparable)xValue).CompareTo(yValue);
            }
            // If values don't implement IComparer but are equivalent
            else if (xValue.Equals(yValue))
            {
               result = 0;
            }
            //Values don't implement IComparer and are not equivalent, so compare as string values
            else result = xValue.ToString().CompareTo(yValue.ToString());

            // Return result
         }
         return result;
      }

      private int CompareDescending(object xValue, object yValue)
      {
         //Return result adjusted for ascending or descending sort order ie
         //multiplied by 1 for ascending or -1 for descending
         return -CompareAscending(xValue, yValue);
      }

      private object GetPropertyValue(TKey value, string property)
      {
         //Get property
         PropertyInfo propertyInfo = value.GetType().GetProperty(property);

         //Return value 
         return propertyInfo.GetValue(value, null);
      }
   }
}
