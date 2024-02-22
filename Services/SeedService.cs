using Cinemate.Data;
using Cinemate.Models.Database;
using Cinemate.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinemate.Services
{
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
            await UpdateDatabaseAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCollections();
        }

        private async Task UpdateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedRolesAsync()
        { /* The primary objective of this method is to seed 1 and only 1 role. 
           * If there are any role available already im the system, it won't do anything.
           */

            if (_dbContext.Roles.Any()) return;

            var adminRole = _appSettings.CinemateSettings.DefaultCredentials.Role;

            // This line will create a new Role
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        private async Task SeedUsersAsync()
        {   /* This method is responsible for  both creating a user in Asp.net Users table
               and assign a role of administrator
            */
            if (_userManager.Users.Any()) return;

            var credentials = _appSettings.CinemateSettings.DefaultCredentials;


            var newUser = new IdentityUser()
            {
                Email = credentials.Email,
                UserName = credentials.Email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newUser, credentials.Password);
            await _userManager.AddToRoleAsync(newUser, credentials.Role);

        }

        private async Task SeedCollections()
        {
            //if(_dbContext.Collection.Any()) return;

            _dbContext.Add(new Collection()
            {
                Name = _appSettings.CinemateSettings.DefaultCollection.Name,
                Description = _appSettings.CinemateSettings.DefaultCollection.Description
            });

            await _dbContext.SaveChangesAsync();
        }

    }
}
