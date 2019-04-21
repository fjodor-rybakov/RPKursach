using Microsoft.AspNetCore.Mvc;

namespace ApiErrors
{
    public class Error
    {
        public string Message;
    }
    
    public class ApiError : ControllerBase
    {
        public ActionResult UserNotFount => NotFound(new Error{Message = "Такой пользователь не существует"});
    }
}
