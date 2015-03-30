using Vjezba.BL.MainSearch.Models;
using Vjezba.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vjezba.BL.MainSearch.Repository
{
    public class MainSearchRepository : IDisposable
    {
        private BizHubEntities ctx = new BizHubEntities();

        public IQueryable<MainSearchReturnModel> AdvancedSearchUsers(string searchName, string[] country, string searchCity, string[] category, int page)
        {
            int pageLength = 10;

            int pageToSkip = (page - 1) * pageLength;

            if (Array.IndexOf(country, "") > -1 && Array.IndexOf(category, "") > -1)
            {
                int totalUsers = (from user in ctx.Profile
                                  where user.Name.Contains(searchName)  && user.City.Contains(searchCity)
                                  select user.ProfileID
                ).Count();

                if (totalUsers < 1)
                {
                    return null;
                }

                var result = (from user in ctx.Profile
                              where user.Name.Contains(searchName) && user.City.Contains(searchCity)
                              orderby user.Name
                              select new MainSearchReturnModel
                              {
                                  ProfileID = user.ProfileID,
                                  Name = user.Name,
                                  Category = user.Category,
                                  Country = user.Country,
                                  City = user.City,
                                  ProfilePicture = user.ProfilePicture,
                                  TotalUsers = totalUsers
                              }
                                  ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else if (Array.IndexOf(country, "") > -1)
            {
                int totalUsers = (from user in ctx.Profile
                                  where user.Name.Contains(searchName) && user.City.Contains(searchCity) && category.Any(val => user.Category.Contains(val))
                                  select user.ProfileID
                ).Count();

                if (totalUsers < 1)
                {
                    return null;
                }

                var result = (from user in ctx.Profile
                              where user.Name.Contains(searchName) && user.City.Contains(searchCity) && category.Any(val => user.Category.Contains(val))
                              orderby user.Name
                              select new MainSearchReturnModel
                              {
                                  ProfileID = user.ProfileID,
                                  Name = user.Name,
                                  Category = user.Category,
                                  Country = user.Country,
                                  City = user.City,
                                  ProfilePicture = user.ProfilePicture,
                                  TotalUsers = totalUsers
                              }
                                  ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else if (Array.IndexOf(category, "") > -1)
            {
                int totalUsers = (from user in ctx.Profile
                                  where user.Name.Contains(searchName) && user.City.Contains(searchCity) && country.Any(val => user.Country.Contains(val))
                                  select user.ProfileID
                ).Count();

                if (totalUsers < 1)
                {
                    return null;
                }

                var result = (from user in ctx.Profile
                              where user.Name.Contains(searchName) && user.City.Contains(searchCity) && country.Any(val => user.Country.Contains(val))
                              orderby user.Name
                              select new MainSearchReturnModel
                              {
                                  ProfileID = user.ProfileID,
                                  Name = user.Name,
                                  Category = user.Category,
                                  Country = user.Country,
                                  City = user.City,
                                  ProfilePicture = user.ProfilePicture,
                                  TotalUsers = totalUsers
                              }
                                  ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }
            else
            {
                int totalUsers = (from user in ctx.Profile
                                  where user.Name.Contains(searchName) && country.Any(val => user.Country.Contains(val)) && user.City.Contains(searchCity) && category.Any(val => user.Category.Contains(val))
                                  select user.ProfileID
                ).Count();

                if (totalUsers < 1)
                {
                    return null;
                }

                var result = (from user in ctx.Profile
                              where user.Name.Contains(searchName) && country.Any(val => user.Country.Contains(val)) && user.City.Contains(searchCity) && category.Any(val => user.Category.Contains(val))
                              orderby user.Name
                              select new MainSearchReturnModel
                              {
                                  ProfileID = user.ProfileID,
                                  Name = user.Name,
                                  Category = user.Category,
                                  Country = user.Country,
                                  City = user.City,
                                  ProfilePicture = user.ProfilePicture,
                                  TotalUsers = totalUsers
                              }
                                  ).Skip(pageToSkip).Take(pageLength);

                return result.AsQueryable();
            }

            

        }

        public void Dispose()
        {
            ctx.Dispose();
        }
    }
}
