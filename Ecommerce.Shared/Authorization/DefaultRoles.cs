namespace Ecommerce.Shared.Authorization;
public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "019931ab-a038-78cd-b48c-a587b7f3d33f";
        public const string ConcurrencyStamp = "019931ab-a038-78cd-b48c-a58882b3f8fa";
    }
    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "019931ab-a038-78cd-b48c-a589117d2362";
        public const string ConcurrencyStamp = "019931ab-a038-78cd-b48c-a58a7e3e5887";
    }
}

