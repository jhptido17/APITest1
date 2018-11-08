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

        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        /*private List<Users> _users = new List<Users>
        {
            new Users { Id = 1, Username = "test", Password = "test", Role = "admin" }
        };*/

        public async Task<Users> Authenticate(string username, string password)
        {
            //var users = await _context.Users.FindAsync(id);

            //var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));
            Console.WriteLine("-----username " + username + " Password " + password);
            var user = await Task.Run(() => _context.Users.Where(x => x.Username == username && x.Password == password));
            //var user = _context.Users.Where(d => d.Username == username && d.Password == password);
            Console.WriteLine("------------------------------------------------------------------------------");
            if (user.Count() == 0)
            {
                Console.WriteLine("esta vacio");
                return null;
            }
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            //user.First().Password = null;
            //user.Password = null;
            return user.First();
            //return user;
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            //return await Task.Run(() => _context.Users.Select(x => { x.Password = null; return x; }));
            /*var userList = await Task.Run(() => _context.Users);
            return userList.Select(x => { x.Password = null; return x; });*/
            return await Task.Run(() => _context.Users);

            //return await Task.Run(() => _context.Users.Select(x => x.Password = null;);
        }

        /*public async Task<IEnumerable<Users>> GetAll()
        {
            // return users without passwords
            return await Task.Run(() => _users.Select(x => {
                x.Password = null;
                return x;
            }));
        }*/
    }
}
