
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using SystemFramework.Common.ObjectCollectionBase;

using System.Runtime.Serialization.Formatters.Binary;

namespace SystemFramework.Common
{
    [Serializable()]
    public class ObjectCollection<T> : BindingList<T>, INotifyCollectionChanged where T : BaseEntity
    {
        #region Member fields

        private BindingList<T> _deletedEntities;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new empty list of entyties.
        /// </summary>
        public ObjectCollection()
            : base()
        {
            this.AllowNew = true;
            _deletedEntities = new BindingList<T>();
            //this.ListChanged += new ListChangedEventHandler(ObjectCollection_ListChanged);
        }

        /// <summary>
        /// Create a new empty list of entyties.
        /// </summary>
        public ObjectCollection(IList<T> objectsList)
            : base(objectsList)
        {
            this.AllowNew = true;
            _deletedEntities = new BindingList<T>();
        }

        #endregion

        #region Binding List

        #region public methods

        /// <summary>
        /// Indicates that the data of this list is in editing mode.
        /// This method will keep the original state of this entity althought 
        /// its data is modified. You must call EndEdit() method when the editting
        /// is completed
        /// </summary>
        /// <remarks></remarks>
        public void BeginEdit()
        {
            this.RaiseListChangedEvents = false;
        }

        /// <summary>
        /// Completed editting data.
        /// </summary>
        /// <remarks></remarks>
        public void EndEdit()
        {
            this.RaiseListChangedEvents = true;
        }

        /// <summary>
        /// Gets all entities that have Added, Modified and Deleted state.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<T> GetChanges()
        {
            // We need to return all items that have been changed, including the delted rows
            List<T> changedItems = new List<T>();
            foreach (T item in this.Items)
            {
                // return any item that has been changed
                if (item.EntityState != EntityState.Unchanged)
                {
                    changedItems.Add(item);
                }
            }
            // Get the deleted items
            foreach (T item in _deletedEntities)
            {
                changedItems.Add(item);
            }
            // return all the changed items (deleted, modified, added)
            return changedItems;
        }

        /// <summary>
        /// Gets all entities that have the specific state.
        /// </summary>
        /// <param name="state">The state of entities that you want to get.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<T> GetChanges(EntityState state)
        {
            List<T> changedItems = new List<T>();
            if (state == EntityState.Deleted)
            {
                foreach (T item in DeletedEntities)
                    changedItems.Add(item);
            }
            else
            {
                foreach (T item in this.Items)
                {
                    if (item.EntityState == state)
                        changedItems.Add(item);
                }
            }
            return changedItems;
        }

        /// <summary>
        /// Accepts all changes. After calls AcceptChanges(), you cannot RejectChanges().
        /// The entity will return to Unchanged state.
        /// </summary>
        public void AcceptChanges()
        {
            foreach (T item in this.Items)
            {
                item.AcceptChanges();
            }
            _deletedEntities.Clear();
        }

        /// <summary>
        /// Accepts all the deletions of this collection. 
        /// After calls AcceptDeletions(), you cannot recover these deleted items.
        /// </summary>
        public void AcceptDeletions()
        {
            _deletedEntities.Clear();
        }

        /// <summary>
        /// Rolls Back all changes and the entity will return to Unchanged state.
        /// </summary>
        public void RejectChanges()
        {
            BeginEdit();
            for (int i = this.Count - 1; i >= 0; i--)
            {
                T item = this[i];
                if (item.IsAdded)
                {
                    this.Remove(item);
                }
                else if (item.IsModified)
                {
                    item.RejectChanges();
                }
            }
            foreach (T item in _deletedEntities)
            {
                item.RejectChanges();
                this.Add(item);
            }
            _deletedEntities.Clear();
            EndEdit();
            // Let bound controls know they should refresh their views
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Clear all entities.
        /// </summary>
        public new void Clear()
        {
            base.ClearItems();
            DeletedEntities.Clear();
        }

        /// <summary>
        /// Adds a list object to the collection. 
        /// </summary>
        /// <param name="objectsList"></param>
        public void AddRange(IList<T> objectsList)
        {
            foreach (T item in objectsList)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Merges an entity to this list
        /// </summary>
        /// <param name="entity"></param>
        public void Merge(T sourceEntity)
        {
            if (sourceEntity.KeyValue.IsKeyNull)
            {
                this.Add(sourceEntity);
            }
            else
            {
                int size = this.Count;
                for (int i = 0; i < size; i++)
                {
                    T entity = this[i];
                    if (entity.KeyValue == sourceEntity.KeyValue)
                    {
                        if (sourceEntity.IsDeleted)
                        {
                            this.Remove(entity);
                        }
                        else
                        {
                           entity.AcceptChanges();
                           this.SetItem(i, (T)sourceEntity.Copy());
                        }
                        return;
                    }
                }
                this.Add(sourceEntity);
            }
        }

        /// <summary>
        /// Merges a list of entities to this list
        /// </summary>
        /// <param name="entities"></param>
        public void Merge(IList entities)
        {
            foreach (T entity in entities)
            {
                Merge(entity);
            }
        }

        /// <summary>
        /// Clone a list of entity
        /// </summary>
        /// <returns></returns>
        public ObjectCollection<T> Clone()
        {
            List<T> list = new List<T>();
            foreach (T entity in this)
            {
                list.Add(entity.Clone() as T);
            }
            return new ObjectCollection<T>(list);
        }

        public bool ContainKeyOf(T item)
        {
            KeyValue key = item.KeyValue;
            foreach (T entity in this)
            {
                if (entity.KeyValue == key)
                {
                    return true;
                }
            }
            return false;
        }

        public T this[KeyValue key]
        {
            get
            {
                foreach (T entity in this)
                {
                    if (entity.KeyValue == key)
                    {
                        return entity;
                    }
                }
                return null;
            }
            set
            {
                this[key] = value;
            }
        }

        public ObjectCollection<T> Select(string propertyName, object value)
        {
            ObjectCollection<T> list = new ObjectCollection<T>();
            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            foreach (T item in this)
            {
                if (prop.GetValue(item, null).Equals(value))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public T FindByKey(string keyProperty, object value)
        {
            ObjectCollection<T> list = Select(keyProperty, value);
            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Gets the deleted entities
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected BindingList<T> DeletedEntities
        {
            get { return _deletedEntities; }
        }

        /// <summary>
        /// Removes an entity and mark it as deleted.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            BaseEntity deletedEntity = this[index];
            if (!deletedEntity.IsAdded)
            {
                _deletedEntities.Add(deletedEntity as T);
            }
            deletedEntity.EntityState = EntityState.Deleted;
            base.RemoveItem(index);
        }

        #endregion

        #endregion

        #region Sorting

        private bool _isSorted;
        private ListSortDirection _dir = ListSortDirection.Ascending;
        [NonSerialized()]
        private PropertyDescriptor _sort = null;

        public void Sort()
        {
            ApplySortCore(_sort, _dir);
        }

        public void Sort(string property)
        {
            // Get the PD
            _sort = FindPropertyDescriptor(property);

            //Sort
            ApplySortCore(_sort, _dir);
        }

        public void Sort(string property, ListSortDirection direction)
        {
            //Get the sort property 
            _sort = FindPropertyDescriptor(property);
            _dir = direction;

            //Sort
            ApplySortCore(_sort, _dir);
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;

            if ((null != items) && (null != property))
            {
                PropertyComparer<T> pc = new PropertyComparer<T>(property, direction);
                BeginEdit();
                items.Sort(pc);
                EndEdit();

                //Set sorted
                _isSorted = true;
            }
            else
            {
                // Set sorted
                _isSorted = false;
            }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
        }

        private PropertyDescriptor FindPropertyDescriptor(string property)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = null;

            if (null != pdc)
            {
                prop = pdc.Find(property, true);
            }

            return prop;
        }

        #endregion

        #region Searching

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {

            // Specify search columns
            if (property == null)
                return -1;

            // Traverse list for value
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                object value =property.GetValue(this[i]);
                if (value != null)
                {
                    if (value.Equals(key))
                        return i;
                }
                else
                {
                    if (key == null)
                        return i;
                }
            }
            return -1;
        }

        #endregion

        #region Persistence

        /// <summary>
        /// Saves teh list to file.
        /// </summary>
        /// <param name="filename">"The file name"</param>
        public void Save(string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                // Serialize data list items
                formatter.Serialize(stream, (List<T>)this.Items);
            }
        }

        /// <summary>
        /// Load the list from a file to memory.
        /// </summary>
        /// <param name="filename">The name of the file</param>
        public void Load(string filename)
        {
            BeginEdit();
            this.ClearItems();

            if (File.Exists(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    // Deserialize data list items
                    ((List<T>)this.Items).AddRange((IEnumerable<T>)formatter.Deserialize(stream));
                }
            }

            // Let bound controls know they should refresh their views
            EndEdit();
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        #endregion

        #region ICollectionItemChanged Members

        public event ItemAddedDelegate ItemAdded;
        public event ItemDeletedDelegate ItemDeleted;
        public event ItemChangedDelegate ItemChanged;

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (ItemAdded != null)
                        ItemAdded(this[e.NewIndex]);
                    break;
                case ListChangedType.ItemChanged:
                    if (ItemChanged != null)
                        ItemChanged(this[e.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    if (ItemDeleted != null)
                        ItemDeleted(e.NewIndex);
                    break;
            }
        }

        #endregion
    }
}