using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SystemFramework.Common.EntityBase
{
    [Serializable]
    public class StatefullEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Member fields

        protected EntityState _state;
        protected bool _isEditing;

        #endregion

        /// <summary>
        /// Change the state of the entity
        /// </summary>
        /// <param name="dataState"></param>
        /// <param name="propertyName"></param>
        protected void DataStateChanged(EntityState dataState, string propertyName)
        {
            if (!_isEditing)
            {
                if (!(string.IsNullOrEmpty(propertyName)))
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                }
                if (this._state == EntityState.Unchanged && dataState == EntityState.Modified)
                {
                    this._state = dataState;
                }
                else
                {
                    if (dataState == EntityState.Deleted)
                    {
                        this._state = EntityState.Deleted;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current state of this entity
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        internal EntityState EntityState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        /// <summary>
        /// Gets the value indicates that the state of this entity is added or not.
        /// </summary>
        [Browsable(false)]
        public bool IsAdded
        {
            get
            {
                return (_state == EntityState.Added);
            }
        }

        /// <summary>
        /// Gets the value indicates that the state of this entity is modified or not.
        /// </summary>
        [Browsable(false)]
        public bool IsModified
        {
            get
            {
                return (_state == EntityState.Modified);
            }
        }

        /// <summary>
        /// Gets the value indicates that the state of this entity is modified or not.
        /// </summary>
        [Browsable(false)]
        public bool IsDeleted
        {
            get
            {
                return (_state == EntityState.Deleted);
            }
        }

        /// <summary>
        /// Gets the value indicates that the state of this entity is unchanged or not.
        /// </summary>
        [Browsable(false)]
        public bool IsUnchanged
        {
            get
            {
                return (_state == EntityState.Unchanged);
            }
        }

        /// <summary>
        /// Check whether the data of this entity is changed or not.
        /// It just check for this instant only, not for its sub collections or sub entities.
        /// So you must overide this method in case your entity has sub collections. 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDataChanged()
        {
            return (_state == EntityState.Added || _state == EntityState.Modified);
        }

        /// <summary>
        /// Keep the original status of the entity
        /// </summary>
        public void BeginEdit()
        {
            _isEditing = true;
        }

        /// <summary>
        /// Ends editing
        /// </summary>
        public void EndEdit()
        {
            _isEditing = false;
        }

        /// <summary>
        /// Removes all changes
        /// </summary>
        public void RejectChanges()
        {
            _state = EntityState.Unchanged;
        }

        /// <summary>
        /// Accepts all changes
        /// </summary>
        public void AcceptChanges()
        {
            _state = EntityState.Unchanged;
        }

        /// <summary>
        /// Mark delete for the entity
        /// </summary>
        public void Delete()
        {
            _state = EntityState.Deleted;
        }
    }
}
