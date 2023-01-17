using System.Collections.Generic;

namespace Project_SignalR_Api.Models
{
    public class Room
    {
        //kayıt atmaya çalışırken aldığımız hatayı engellemek için bu constructor'ı ekliyoruz
        public Room()
        {
            Users = new List<User>();
        }
        public int RoomID { get; set; }
        public string RoomName { get; set; }

        public List<User> Users { get; set; }

        //bir kullanıcı 1 odada bulunabilecek.
    }
}
