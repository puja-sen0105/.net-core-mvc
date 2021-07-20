using Microsoft.AspNetCore.Mvc;
using OdeToFood.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.ViewComponents
{
    //doesnt respond to http request its more like partial view
    public class RestaurantCountViewComponent
         : ViewComponent
    {
        private readonly IRestaurantData restaurantData;

        public RestaurantCountViewComponent(IRestaurantData restaurantData)
        {
            this.restaurantData = restaurantData;
        }

        public IViewComponentResult Invoke(int zipcode)
        {
            var count = restaurantData.GetCountOfRestaurants();
            return View(count);
        }

    }
}
