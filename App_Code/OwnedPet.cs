using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class OwnedPet
{
    public int ColourNum { get; set; }
    public bool IsEquipped { get; set; }

    public OwnedPet(int colourNum, bool isEquipped)
    {
        ColourNum = colourNum;
        IsEquipped = isEquipped;
    }
}