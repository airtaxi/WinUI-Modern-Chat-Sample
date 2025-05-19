using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model;

public class User
{
    public required string Id { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public string FullName => $"{LastName}{FirstName}";

    public string ProfileImageUrl => $"https://picsum.photos/seed/{Id}/256/256";
    public string BackgroundImageUrl => $"https://picsum.photos/seed/{Id + "123"}/800/400?blur";
}
