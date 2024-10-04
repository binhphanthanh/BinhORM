using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.EntityBase
{
    [Serializable]
    public class ValueUpdatable : StatefullEntity
    {
        protected void UpdateValue<T>(ref T oldValue, T newValue, string propName)
        {
            bool different = false;
            if (oldValue == null)
            {
                if (newValue != null)
                    different = true;
            }
            else
            {
                if (!oldValue.Equals(newValue))
                    different = true;
            }            
            if (different)
            {
                oldValue = newValue;
                this.DataStateChanged(EntityState.Modified, propName);
            }
        }

        protected void UpdateValue(ref int oldValue, int newValue, string propName)
        {
            UpdateValue<int>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref int? oldValue, int? newValue, string propName)
        {
            UpdateValue<int?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref long oldValue, long newValue, string propName)
        {
            UpdateValue<long>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref long? oldValue, long? newValue, string propName)
        {
            UpdateValue<long?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref short oldValue, short newValue, string propName)
        {
            UpdateValue<short>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref short? oldValue, short? newValue, string propName)
        {
            UpdateValue<short?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref byte oldValue, byte newValue, string propName)
        {
            UpdateValue<byte>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref byte? oldValue, byte? newValue, string propName)
        {
            UpdateValue<byte?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref bool oldValue, bool newValue, string propName)
        {
            UpdateValue<bool>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref bool? oldValue, bool? newValue, string propName)
        {
            UpdateValue<bool?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref float oldValue, float newValue, string propName)
        {
            UpdateValue<float>(ref oldValue, newValue, propName);
        }
        protected void UpdateValue(ref float? oldValue, float? newValue, string propName)
        {
            UpdateValue<float?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref double oldValue, double newValue, string propName)
        {
            UpdateValue<double>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref double? oldValue, double? newValue, string propName)
        {
            UpdateValue<double?>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref string oldValue, string newValue, string propName)
        {
            UpdateValue<string>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref DateTime oldValue, DateTime newValue, string propName)
        {
            UpdateValue<DateTime>(ref oldValue, newValue, propName);
        }

        protected void UpdateValue(ref byte[] oldValue, byte[] newValue, string propName)
        {
            UpdateValue<byte[]>(ref oldValue, newValue, propName);
        }
    }
}
