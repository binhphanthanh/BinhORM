using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.ObjectCollectionBase
{
    public delegate void ItemAddedDelegate(BaseEntity entity);
    public delegate void ItemDeletedDelegate(int entityIndex);
    public delegate void ItemChangedDelegate(BaseEntity entity);

    public interface INotifyCollectionChanged
    {
        event ItemAddedDelegate ItemAdded;
        event ItemDeletedDelegate ItemDeleted;
        event ItemChangedDelegate ItemChanged;
    }
}
