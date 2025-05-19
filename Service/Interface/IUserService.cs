using BMW_20250523.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service.Interface;

public interface IUserService
{
    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddUser
    public User AddUser(string firstName, string lastName);

    // Read: GetUser, GetAllUsers, GetUsersByIds
    public User GetUser(string userId); 
    public List<User> GetAllUsers();
    public List<User> GetUsersByIds(List<string> userIds);

    // Update: ReplaceUser
    public void ReplaceUser(string userId, User newUser);

    // Delete: DeleteUser
    public void DeleteUser(string userId);

    public List<User> SearchForUsers(string searchText);
    public List<User> GetRandomUsers(int count);
    public List<User> GenerateSampleUsers(int numberOfUsers);
}
