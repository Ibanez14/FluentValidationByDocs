using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidationWeb.Models
{
    public class UserRegisterRequest
    {
        internal object Id;

        public string Tagname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address MainAddress{ get; set; }

        public decimal Money { get; set; }
        public string FavoriteBand { get; set; }

        public List<string> FavoriteBooks { get; set; }
        public List<Address> AdditionalAddresses { get; set; }
        public List<Licence> Licenses { get; set; }
        public int CustomerDiscount { get; internal set; }
        public bool IsPreferredCustomer { get; internal set; }
        public bool IsPreferred { get; internal set; }
    }

    // For Address you have to have separate validator
    public class Address
    {
        public string Street { get; set; }
    }

    public class Licence
    {
        public DateTime IssueDate { get; set; }
    }
}
