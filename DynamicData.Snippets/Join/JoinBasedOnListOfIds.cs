using System.Collections.Generic;

namespace DynamicData.Snippets.Join
{
    class JoinBasedOnListOfIds
    {
        public JoinBasedOnListOfIds(IObservableCache<User, int> users, IObservableCache<Role, int> roles)
        {
            //TODO: Add overload to join many which enables joining on an array

            //users.Connect().JoinMany(roles.Connect(),
            //        // select some sort of list with ids 
            //        user => user.Roles,
            //        // select right key
            //        role => role.Id,
            //        // join every list of every user with the right source and produce a list of matching values
            //        (user, roles) => new UserViewModel
            //        {
            //            Name = user.Name,
            //            Roles = roles
            //        });

        }
    }
    
    class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> Roles { get; set; }
    }

    class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    class UserViewModel
    {
        public string Name { get; set; }

        public List<Role> Roles { get; set; }
    }
}
