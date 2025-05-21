using BMW_20250523.Model;
using BMW_20250523.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service;

public class UserService : IUserService
{
    private readonly List<User> _users = [];

    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddUser
    public User AddUser(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("First name and last name cannot be null or empty.");

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            FirstName = firstName,
            LastName = lastName,
        };

        do
        {
            var existingUser = _users.FirstOrDefault(x => x.Id == newUser.Id);
            if (existingUser == null) break;

            newUser.Id = Guid.NewGuid().ToString("N")[..8];
        } while (true);

        _users.Add(newUser);

        return newUser;
    }

    // Read: GetUser, GetAllUsers, GetUsersByIds
    public User GetUser(string userId) => _users.FirstOrDefault(x => x.Id == userId)
        ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

    public List<User> GetAllUsers() => [.. _users];

    public List<User> GetUsersByIds(List<string> userIds)
    {
        if (userIds == null || userIds.Count == 0) return [.. _users];
        return [.. _users.Where(x => userIds.Contains(x.Id))];
    }

    // Update: ReplaceUser
    public void ReplaceUser(string userId, User newUser)
    {
        var index = _users.FindIndex(x => x.Id == userId);
        if (index != -1) _users[index] = newUser;
        else throw new KeyNotFoundException($"User with ID {userId} not found.");
    }

    // Delete: DeleteUser
    public void DeleteUser(string userId)
    {
        var index = _users.FindIndex(x => x.Id == userId);
        if (index != -1) _users.RemoveAt(index);
        else throw new KeyNotFoundException($"User with ID {userId} not found.");
    }

    public List<User> GenerateSampleUsers(int numberOfUsers)
    {
        var users = new List<User>();

        string[] firstNames = { "철수", "영희", "민수", "지민", "서연", "하준", "예린", "도윤", "수빈", "지우", "유진", "민재", "서준", "지안", "하율", "예은", "도현", "수연", "지후", "유나", "민서", "서하", "지유", "하린", "예지", "도연", "수아", "지민", "유빈", "민규", "서영", "지성", "하은", "예솔", "도윤", "수현", "지혜", "유리", "민찬", "서진", "지수", "하람", "예나", "도윤", "수빈", "지영", "유정", "민호", "서율", "지은", "하영", "예슬", "도현", "수진", "지혜", "유림" };
        string[] lastNames = { "김", "이", "박", "최", "정", "강", "조", "윤", "임", "오", "서", "한", "신", "황", "안", "전", "고", "문", "양", "손", "배", "남궁", "황보", "유", "구", "차", "탁", "변", "엄", "도", "추", "하", "여", "진", "기", "명", "방", "설", "변", "옥", "가" };

        for (int i = 1; i <= numberOfUsers; i++)
        {
            users.Add(AddUser(firstNames[Random.Shared.Next(firstNames.Length)],
                lastNames[Random.Shared.Next(lastNames.Length)]));
        }

        return users;
    }

    public List<User> SearchForUsers(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return _users;
        else return [.. _users.Where(u => u.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase))];
    }

    public List<User> GetRandomUsers(int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        return [.. _users.OrderBy(_ => Random.Shared.Next()).Take(count)];
    }
}
