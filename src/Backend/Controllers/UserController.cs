using Assets.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    public class UserController : ControllerBase
    {
        [HttpPost]
        public void Registration([FromBody] string value)
        {
            RegistrationUser registrationUser = JsonConvert.DeserializeObject<RegistrationUser>(value);
        }
    }
}