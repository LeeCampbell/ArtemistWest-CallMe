using System;
using System.Collections.Generic;

namespace ArtemisWest.CallMe.Contract.Contacts
{
    public interface IContact
    {
        /// <summary>
        /// How the user commonly references the contact e.g. Dan Rowe
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The formal or full name of the contact e.g. Mr. Daniel Alex Rowe
        /// </summary>
        string FullName { get; }
        
        //Full Name
        //GivenName
        //FamilyName

        /// <summary>
        /// Link to an image of the contact
        /// </summary>
        Uri Image { get; }

        /// <summary>
        /// The Date of birth for the contact. If the Year is unknown then it should be set to a value of 1.
        /// </summary>
        DateTime? DateOfBirth { get; }

        IEnumerable<IContactAssociation> Organizations { get; }

        IEnumerable<IContactAssociation> Relationships { get; }
        
        //EmailAddresses
        /*
         * Association e.g. Home/Work
         * Value e.g. lee@home.com
         * IsPrimary? Just put this one first?
         */
        IEnumerable<IContactAssociation> EmailAddresses { get; }

        //PhoneNumbers
        /*
         * Association : e.g. Home/work/mobile
         */
        IEnumerable<IContactAssociation> PhoneNumbers { get; }

        //Messenger handles? -->Twitter/MSN/Skype etc?
         
        //Categories -->Circles/Groups/Forums etc...
    }
}