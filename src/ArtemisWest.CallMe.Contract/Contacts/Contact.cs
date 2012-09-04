using System;
using System.Collections.Generic;

namespace ArtemisWest.CallMe.Contract.Contacts
{
    public class Contact : IContact
    {
        private readonly List<IContactAssociation> _organizations = new List<IContactAssociation>();
        private readonly List<IContactAssociation> _relationships = new List<IContactAssociation>();
        private readonly List<IContactAssociation> _emailAddresses = new List<IContactAssociation>();
        private readonly List<IContactAssociation> _phoneNumbers = new List<IContactAssociation>(); 

        public string Title { get; set; }

        public string FullName { get; set; }

        public Uri Image { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public List<IContactAssociation> Organizations
        {
            get { return _organizations; }
        }

        public List<IContactAssociation> Relationships
        {
            get { return _relationships; }
        }

        public List<IContactAssociation> EmailAddresses
        {
            get { return _emailAddresses; }
        }

        public List<IContactAssociation> PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        #region Explict implementation of IContact interface for IEnumerable<T> properties

        IEnumerable<IContactAssociation> IContact.Organizations
        {
            get { return _organizations; }
        }

        IEnumerable<IContactAssociation> IContact.Relationships
        {
            get { return Relationships; }
        }

        IEnumerable<IContactAssociation> IContact.EmailAddresses
        {
            get { return EmailAddresses; }
        }

        IEnumerable<IContactAssociation> IContact.PhoneNumbers
        {
            get { return PhoneNumbers; }
        }

        #endregion
    }
}