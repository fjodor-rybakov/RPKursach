using Microsoft.AspNetCore.Mvc;

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
        
        public ActionResult OrderNotFound => NotFound(new Error{Message = "Заказ не найден"});

        public ActionResult ProductNotFound => NotFound(new Error{Message = "Продкукт не найден"});
        public ActionResult IncorrectProductCount => BadRequest(new Error {Message = "Неверное кол-во продукта"});

        public ActionResult IncorrectCountImageFile => BadRequest(new Error {Message = "Неверное кол-во файлов"});
        public ActionResult IncorrectContentTypeFile => BadRequest(new Error {Message = "Неверный формат файла"});
        
        public ActionResult ServerError => StatusCode(500, new Error{Message = "Произошла ошибка на сервере"});
        public object InvalidToken => StatusCode(401, new Error {Message = "Токен истёк или не валиден"}).Value;
        public object AccessDenied => StatusCode(403, new Error {Message = "Недостаточно прав"}).Value;
    }
}
