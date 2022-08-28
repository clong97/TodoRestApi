using System.Web.Http;
using TodoRestApi.Models;
using TodoRestApi.Dto;
using TodoRestApi.Utils;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace TodoRestApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private TodoDbEntities db = new TodoDbEntities();

        [HttpPost]
        public HttpResponseMessage Register([FromBody]RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Username.ToLower() == model.Username.ToLower());
                if (existingUser == null)
                {
                    User user = new User();
                    user.Name = model.Name;
                    user.Username = model.Username;
                    user.Password = Encrypt.EncryptPassword(model.Password);

                    db.Users.Add(user);
                    db.SaveChanges();
                    
                    return Request.CreateResponse(HttpStatusCode.OK, "User created.");
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Username exist.");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }


    }
}
