﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country {  get; set; }
        public string? Address { get; set; }
        public bool ReceieveNewsletter { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) 
            {
                return false;
            }
            if (obj.GetType() != typeof(PersonResponse)) 
            {
                return false;
            }

            PersonResponse other = (PersonResponse) obj;
            return PersonId == other.PersonId && PersonName == other.PersonName && Email == other.Email && DateOfBirth == other.DateOfBirth && Gender == other.Gender && CountryId == other.CountryId && Address == other.Address && ReceieveNewsletter == other.ReceieveNewsletter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class PersonExtensions 
    {
        public static PersonResponse ToPersonResponse(this Person person) 
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                CountryId = person.CountryId,
                ReceieveNewsletter = person.ReceieveNewsletter,
                Age = (person.DateOfBirth!=null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays/365.25) : null
            };
        }
    }
}
