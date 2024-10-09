using System.Runtime.CompilerServices;
using Api.controllers;
using API.Services;

namespace API.Controllers
{
    public class UserController(UserService user) : BaseController
    {
        private readonly UserService _user = user;
    }
}
