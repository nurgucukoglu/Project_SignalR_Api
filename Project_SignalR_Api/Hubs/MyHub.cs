using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_SignalR_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_SignalR_Api.Hubs
{
    public class MyHub:Hub  //merkez class burası, endpointte yönlendireceğimiz class, istekte bulunduğumuz metotları barındıran ve o metotlar için abonelik sistemi oluşturduğumuz class.
    {
        private readonly Context _context;

        public MyHub(Context context/*, int clientCount*/)
        {
            _context = context;
            //ClientCount = clientCount;
        }

        public static List<string> Names { get; set; } = new List<string>();

        public int ClientCount { get; set; } = 0; //client sayısını yazdırıcaz

        public static int roomCount { get; set; } = 7;// bir oda da maximum 7 kişi olsun.

        //public async Task SendName(string name)  //....üstteki Names.e isim göndermemi sağladı.
        //{
        //    Names.Add(name);
        //    await Clients.All.SendAsync("ReceiveName", name); //bu clientta çalışan tüm portları gösterdik. //burada sadece parametreden gelen değeri aldık=name
            

        //}  //altta bu isimde metot yazdı.
        public async Task GetNames()  //Bu da isimleri getiren bir metot olacak
        {
           
            await Clients.All.SendAsync("ReceiveNames", Names); //burada listedeki her şeyi aldık= Names
        }


        public async override Task OnConnectedAsync() //clientcount metodu
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount); //on metoduyla diğer tarafta çağırıyoruz bu isimleri (ReceiveClientCount), parametre olarak propu gönderdim ClientCount

        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

        }

        //odadaki kişi sayısını olması gereken kapasite ile kontrol edecek
        public async Task SendName(string name)
        {
            if (Names.Count >= roomCount)
            {
                //odadaki kişi sayısı yani names'ler sayısı eğer roomCount büyük ise 
                await Clients.Caller.SendAsync("Error", $"Bu oda en fazla {roomCount} kişi kadar üye alabilir.");
            }
            else
            {
                Names.Add(name);//sorun yoksa o kişiyi de bağlatacak yani listenin içerisine ekleyecek.
                await Clients.All.SendAsync("ReceiveName", name);
            }

        }

      
        public async Task SendNameByGroup(string name, string roomName)
        {
            var room = _context.Rooms.Where(x => x.RoomName == roomName).FirstOrDefault();
            //gönderilen ismin hangi odada bulunduğunu görmek odalara bölmek için bu komut
            //satırını yazıyoruz.
            if (room != null)
            {
                room.Users.Add(new User
                {
                    Name = name
                });
            }
            else
            {
                var newRoom = new Room
                {
                    RoomName = roomName  // oda yoksa odayı oluştur.
                };
                newRoom.Users.Add(new User { Name = name });//yeni kullanıcıyı odaya eklesin.
                _context.Rooms.Add(newRoom);

            }
            await _context.SaveChangesAsync();
            await Clients.Group(roomName).SendAsync("ReceiveMessageByGroup", name, room.RoomID);
            //seçili odaya o kişiyi ekleyecek

        }


        
        public async Task GetNamesByGroup()  //grubun içindeki kişileri listeleyecek
        {
            var rooms = _context.Rooms.Include(x => x.Users).Select(y => new  //2 tabloyu bağladık include
            {
                roomId = y.RoomID,
                Users = y.Users.ToList()
            });
            await Clients.All.SendAsync("ReceiveNamesByGroup", rooms);
        }


        //kişileri odalara yani gruplara kaydedecek metotlar:
        //gruba ekle
        //Buradaki ekleme çıkarma signal r'daki ekleme çıkarma
        public async Task AddToGroup(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        //gruptan çıkar
        public async Task RemoveToGroup(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        }


    }
}
