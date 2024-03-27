using Onitama.Core.UserAggregate;

namespace Onitama.Core.Tests.Builders;

public class UserBuilder
{
    private readonly User _user = new()
    {
        Id = Guid.NewGuid(),
        Email = Guid.NewGuid().ToString(),
        WarriorName = Guid.NewGuid().ToString(),
        UserName = Guid.NewGuid().ToString(),
        PasswordHash = Guid.NewGuid().ToString()
    };

    public UserBuilder AsCloneOf(User user)
    {
        _user.Id = user.Id;
        _user.Email = user.Email;
        _user.WarriorName = user.WarriorName;
        _user.UserName = user.UserName;
        _user.PasswordHash = user.PasswordHash;
        return this;
    }

    public UserBuilder WithWarriorName(string warriorName)
    {
        _user.WarriorName = warriorName;
        return this;
    }

    public User Build()
    {
        return _user;
    }
}