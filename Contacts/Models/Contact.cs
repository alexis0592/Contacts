using System;
namespace Contacts.Models
{
    public class Contact
    {
		public int ContactId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Image { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
        public string FullName
        {
            get{
                return string.Format("{0} {1}", FirstName, LastName);
            }

        }
        public string ImageFullPath
        {
            get{
				if (string.IsNullOrEmpty(Image))
				{
					return "contact.png";
				}

                return string.Format("http://contactsxamarintata.azurewebsites.net{0}", Image.Substring(1));

			}
        }

		public override int GetHashCode()
		{
			return ContactId;
		}

	}
}
