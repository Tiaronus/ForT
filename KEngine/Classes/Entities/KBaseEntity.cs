using System.Collections.ObjectModel;
using System.ComponentModel;

namespace KEngine.Classes.Entities
{
    public abstract class KBaseEntityGen<T> : KBaseEntity where T : KBaseEntity, new()
    {
        public abstract ObservableCollection<T> LoadAll();
        public abstract bool Save();
        public virtual T New()
        {
            return new T();
        }
        public abstract bool Delete();
    }

    public abstract class KBaseEntity : INotifyPropertyChanged
    {
        public long? ID { get; set; }
       
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string ListBoxDisplay { get { return ID.ToString(); } }

        public virtual void ForceLBDChanged()
        {
            OnPropertyChanged("ListBoxDisplay");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
