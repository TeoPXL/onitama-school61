using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Onitama.Core.UserAggregate;

public class UpdateUser
    {
        private readonly IUserService _userService;

        public UpdateUser(IUserService userService)
        {
            _userService = userService;
        }

        public async Task UpdateUserPropertyAsync(User user, string newPropertyValue)
        {
            // Update the user's property
            user.SomeProperty = newPropertyValue;

            // Use the user service to update the user in the database
            await _userService.UpdateUserAsync(user);
        }
    }