using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Pet
{
    public int PetID {  get; set; }
    public int ColourNum { get; set; }
    public int xpCost { get; set; }
    public int coinCost { get; set; }

    public Pet(int petID, int colourNum, int xpCost, int coinCost)
    {
        this.PetID = petID;
        this.ColourNum = colourNum;
        this.xpCost = xpCost;
        this.coinCost = coinCost;
    }
}