using BMW_20250523.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.ViewModel;

public class UserViewModel(User user)
{
    public User User { get; } = user;

    public string FirstName => User.FirstName;
    public string LastName => User.LastName;

    public string FullName => User.FullName;

    public string ProfileImageUrl => User.ProfileImageUrl;
    public string BackgroundImageUrl => User.BackgroundImageUrl;
}
