using Onitama.Core;
using Onitama.Core.UserAggregate;

namespace Onitama.Api.Services.Contracts;
//DO NOT TOUCH THIS FILE!!

public interface ITokenFactory
{
    string CreateToken(User user, IList<string> roleNames);
}