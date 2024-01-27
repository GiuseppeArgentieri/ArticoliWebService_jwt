using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using ArticoliWebService.Security;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class UserService : IUserService
    {
        private readonly AlphaShopDbContext alphaShopDbContext;

        public UserService(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }
        public async Task<bool> Authenticate(string username, string password)
        {
            bool retVal = false;

            PasswordHasher Hasher = new PasswordHasher();

            Utenti utente = await this.GetUser(username);

            if (utente != null)
            {
                string EncryptPwd = utente.Password;
                retVal = Hasher.Check(EncryptPwd, password).Verified;
            }
            
            return retVal; 
        }

        public async Task<Utenti> GetUser(string UserId)
        {
            return await this.alphaShopDbContext.Utenti
                .Where(c => c.UserId == UserId)
                .Include(r => r.Profili)
                .FirstOrDefaultAsync();
        }
    }
}