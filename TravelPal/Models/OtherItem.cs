using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Interfaces;

namespace TravelPal.Models;

public class OtherItem : IPackingListItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }

    public OtherItem(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    /// <summary>
    /// Method for returning a interpolated string with info about the item.
    /// </summary>
    /// <returns>The method returns a string.</returns>
    public string GetInfo()
    {
        return $"{Name}\tQuantity: {Quantity}";
    }
}
