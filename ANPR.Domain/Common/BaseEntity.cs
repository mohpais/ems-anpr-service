using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Lonsum.Services.ANPR.Domain.Common
{
    public abstract class BaseEntity
    {
        int? _requestedHashCode;
        int _Id;
        string _CreateBy;
        DateTime _CreateDate;
        string _LastUpdateBy;
        DateTime _LastUpdateDate;
        bool _IsDeleted;

        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        public virtual string CreateBy
        {
            get
            {
                return _CreateBy;
            }
            protected set
            {
                _CreateBy = value;
            }
        }

        public virtual DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            protected set
            {
                _CreateDate = value;
            }
        }

        public virtual string LastUpdateBy
        {
            get
            {
                return _LastUpdateBy;
            }
            protected set
            {
                _LastUpdateBy = value;
            }
        }

        public virtual DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            protected set
            {
                _LastUpdateDate = value;
            }
        }

        public virtual bool IsDelete { get => _IsDeleted; protected set => _IsDeleted = value; }


        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            BaseEntity item = (BaseEntity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator == (BaseEntity left, BaseEntity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator != (BaseEntity left, BaseEntity right)
        {
            return !(left == right);
        }
        //public int Id { get; set; }
        //public bool IsDelete { get; set; } = false;
        //public string CreateBy { get; set; }
        //public DateTime CreateDate { get; set; } = DateTime.Now;
        //public string LastUpdateBy { get; set; }
        //public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
