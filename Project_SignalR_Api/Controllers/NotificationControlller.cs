using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project_SignalR_Api.Hubs;
using System.Threading.Tasks;

namespace Project_SignalR_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationControlller : ControllerBase  //odalara ayırma, bu controllera istek atıcaz ve o odada kaç kişi var onu görücez.
    {
       
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationControlller(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        [HttpGet("{roomCount}")] // bu apiye istekte bulunulacak ve istekte bulunduktan sonra odada kaç kişinin olacağını set edecek
        public async Task<IActionResult> SetRoomCount(int roomCount)
        {
            //MyHub.roomCount= roomCount;

           // MyHub.roomCount = roomCount;//böylece apiden gönderdiğim roomcounta göre çalışacak
            //bir değer göndermediğim zaman MyHub içindeki default 7'yi alacak
            await _hubContext.Clients.All.SendAsync("Notify", $"Bu oda en fazla {roomCount} kişi olabilir."); //notify diğer tarafta Indexte çağırıcaz.
            

            return Ok();
        }

    }
}
