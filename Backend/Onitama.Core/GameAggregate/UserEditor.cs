using Microsoft.AspNetCore.Identity;
using Onitama.Core.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onitama.Core.GameAggregate
{
    public class UserEditor
    {
        private UserManager<User> _userManager;
        public UserEditor(UserManager<User> userManager) 
        { 
            _userManager = userManager;
        }

        public void UpdateUser(User user)
        {
            _userManager.UpdateAsync(user);
        }
    }
}
