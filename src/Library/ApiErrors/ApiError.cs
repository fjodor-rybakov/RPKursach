﻿using Microsoft.AspNetCore.Mvc;

namespace ApiErrors
{
    public class Error
    {
        public string Message;
    }
    
    public class ApiError : ControllerBase
    {
        public ActionResult UserNotFount => NotFound(new Error{Message = "Такой пользователь не существует. Проверьте данные для входа"});
        public ActionResult UserAlreadyExist => BadRequest(new Error{Message = "Такой пользователь уже существует"});
        public ActionResult ServerError => StatusCode(500, new Error{Message = "Произошла ошибка на сервере"});
    }
}
