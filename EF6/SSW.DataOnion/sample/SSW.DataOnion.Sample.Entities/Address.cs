using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SSW.DataOnion.Sample.Entities.Exceptions;

namespace SSW.DataOnion.Sample.Entities
{
    public class Address : AggregateRoot
    {
        protected Address()
        {
        }

        private Address(string addressLine1, string addressLine2, string suburb, string postcode, string state)
        {
            Guard.AgainstNullOrEmptyString(addressLine1, nameof(addressLine1));
            Guard.AgainstNullOrEmptyString(suburb, nameof(suburb));
            Guard.AgainstNullOrEmptyString(postcode, nameof(postcode));
            Guard.AgainstNullOrEmptyString(state, nameof(state));
            Guard.Against(() => this.InvalidPostcode(postcode), "Invalid postcode");

            this.Id = Guid.NewGuid();
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Suburb = suburb;
            this.Postcode = postcode;
            this.State = state;
        }

        public static Address Create(string addressLine1, string addressLine2, string suburb, string postcode, string state)
        {
            return new Address(addressLine1, addressLine2, suburb, postcode, state);
        }

        public void Update(string addressLine1, string addressLine2, string suburb, string postcode, string state)
        {
            Guard.AgainstNullOrEmptyString(addressLine1, nameof(addressLine1));
            Guard.AgainstNullOrEmptyString(suburb, nameof(suburb));
            Guard.AgainstNullOrEmptyString(postcode, nameof(postcode));
            Guard.AgainstNullOrEmptyString(state, nameof(state));
            Guard.Against(() => this.InvalidPostcode(postcode), "Invalid postcode");

            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Suburb = suburb;
            this.Postcode = postcode;
            this.State = state;
        }

        private IEnumerable<string> InvalidPostcode(string postocode)
        {
            if (!Regex.IsMatch(postocode, @"\d{4}"))
            {
                yield return "Postcode must have 4 digits";
            }
        } 

        public string AddressLine1 { get; protected set; }

        public string AddressLine2 { get; protected set; }

        public string Suburb { get; protected set; }

        public string State { get; protected set; }

        public string Postcode { get; protected set; }

        public string FullAddress => $@"{this.AddressLine1} 
                                        {(string.IsNullOrEmpty(this.AddressLine2) ? string.Empty : ", " + this.AddressLine2)}, 
                                        {this.Suburb} {this.Postcode}, 
                                        {this.State}";

        public override string ToString()
        {
            return
                $"{nameof(this.Id)}: {this.Id}; {nameof(this.FullAddress)}: {this.FullAddress};";
        }
    }
}
