using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using APITest.Models;
using APITest.Helpers;

namespace APITest.Services
{
    public interface IUserService
    {
        Task<Users> Authenticate(string username, string password);
        Task<IEnumerable<Users>> GetAll();
    }

    public class UserService : IUserService
    {

        private readonly TheCRMserviceContext _context;

        public UserService(TheCRMserviceContext context)
        {
            _context = context;
        }

        public async Task<Users> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _context.Users.Where(x => x.Username == username && x.Password == password));
           
            if (user.Count() == 0)
            {
                return null;
            }
            
            if (user == null)
                return null;

            return user.First();
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            return await Task.Run(() => _context.Users);
        }
    }
}
