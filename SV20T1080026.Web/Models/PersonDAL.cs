namespace SV20T1080026.Web.Models
{
    public class PersonDAL
    {
        public List<Person> List()
        {
            List<Person> list = new List<Person>();

            list.Add(new Person
            {
                PersonId = 1,
                Name = "Chuong Cute",
                Address = "56/93 An Duong Vuong",
                Email = "chuongdoan2102@gmail.com"
            });

            list.Add(new Person
            {
                PersonId = 2,
                Name = "Nhat gay",
                Address = "15 Nguyen Huu Canh",
                Email = "nhatit2101@gmail.com"
            });

            list.Add(new Person
            {
                PersonId = 3,
                Name = "Tu beso",
                Address = "29 Tuong Vy",
                Email = "tumuvodich@gmail.com"
            });

            return list;
        }
    }
}
