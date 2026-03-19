using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarieModele.Enums
{
    [Flags]
    public enum CaracteristiciProdus
    {
        Niciuna = 0,
        DePost = 1,
        FaraZahar = 2,
        FaraGluten = 4,
        FaraLactoza = 8
    }
}
