using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

public class ForbidBanned : AuthorizeAttribute
{
    public ForbidBanned()
    {
        Roles = "User,Admin";
    }
}