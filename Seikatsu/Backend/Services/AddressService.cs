using Seikatsu.Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seikatsu.Backend.Entity;
using Seikatsu.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Seikatsu.Backend.Services
{
    public class AddressService(Data.UserContext context) : IAddressService
    {
        //method to get all addresses of a customer
        public async Task<IEnumerable<AddressResponseDTO>> GetAllAddressAysnc(Guid customerID)
        {

            var address = await context.Addresses.Where(a => a.CustomerId == customerID)
                   .Include(a => a.Customer)
                   .Select(a => new AddressResponseDTO
                   {
                       Id = a.Id,
                       AddressLine1 = a.AddressLine1,
                       AddressLine2 = a.AddressLine2,
                       FullName = a.Customer!.FullName,
                       PhoneNumber = a.Customer!.PhoneNumber,
                       City = a.City,
                       PostalCode = a.PostalCode,
                       Country = a.Country,
                       IsDefault = a.IsDefault
                   }).ToListAsync();

            return address;
        }
        public async Task<IEnumerable<AddressResponseDTO>> GetDefaultAddressAysnc(Guid customerID)
        {
            var defaultAdress = await context.Addresses.Where(a => a.CustomerId == customerID && a.IsDefault)
                     .Include(a => a.Customer)
                     .Select(a => new AddressResponseDTO
                     {
                         Id = a.Id,
                         AddressLine1 = a.AddressLine1,
                         AddressLine2 = a.AddressLine2,
                         FullName = a.Customer!.FullName,
                         PhoneNumber = a.Customer!.PhoneNumber,
                         City = a.City,
                         PostalCode = a.PostalCode,
                         Country = a.Country,
                         IsDefault = a.IsDefault
                     }).ToListAsync();
            return defaultAdress;


        }

        public async Task<AddressResponseDTO> AddAddressAsync(Guid customerID, AddAddressDTO request)
        {

            var customer = await context.Customers
       .Where(c => c.Id == customerID)
       .Select(c => new
       {
           c.FullName,
           c.PhoneNumber,
           HasExisting = c.Addresses.Any()  // checks existing in same query
       })
       .FirstOrDefaultAsync();
            var address = new Address
            {
                Id = Guid.NewGuid(),
                CustomerId = customerID,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = !customer.HasExisting  //  first address auto becomes default 
            };

            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            return new AddressResponseDTO
            {
                Id = address.Id,
                FullName = customer.FullName,     //  from customer
                PhoneNumber = customer.PhoneNumber, // from customer 
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };

        }

        public async Task<bool> DeleteAddressAsync(Guid customerID, Guid addressID)
        {
            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == addressID && a.CustomerId == customerID);
            if (address == null)
            {
                return false;
            }
            context.Addresses.Remove(address);
            await context.SaveChangesAsync();
            return true; //right now it deletes even if the address is set as default.

        }

        public async Task<UpdateAddressDTO> UpdateAddressAsync(Guid customerID, UpdateAddressDTO request, Guid addressID)
        {
            var address = await context.Addresses
        .Include(a => a.Customer)
        .FirstOrDefaultAsync(a => a.Id == addressID
            && a.CustomerId == customerID);
            if (address == null) { return null; }
            address.Id = addressID;
            address.Customer!.FullName = request.FullName;
            address.AddressLine1 = request.AddressLine1;
            address.AddressLine2 = request.AddressLine2;
            address.City = request.City;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;
            address.IsDefault = request.IsDefault;

            await context.SaveChangesAsync();

            return new UpdateAddressDTO
            {
                FullName = address.Customer!.FullName,     //  from customer
                PhoneNumber = address.Customer!.PhoneNumber, // from customer 
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };

        }


    }


}