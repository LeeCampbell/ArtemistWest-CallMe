namespace ArtemisWest.CallMe.Contract.Contacts
{
    public class ContactAssociation : IContactAssociation
    {
        private readonly string _association;
        private readonly string _name;

        public ContactAssociation(string association, string name)
        {
            _association = association;
            _name = name;
        }

        public string Association
        {
            get { return _association; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}