using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore.Models.Models
{
    public static class DbContextExtend
    {
        public static DbContext ToRead(this DbContext dbContext)
        {
            if (dbContext is ApiDBContent)
            {
                return ((ApiDBContent)dbContext).ToRead();
            }
            else
                throw new Exception();
        }

        public static DbContext ToWrite(this DbContext dbContext)
        {
            if (dbContext is ApiDBContent)
            {
                return ((ApiDBContent)dbContext).ToWrite();
            }
            else
                throw new Exception();
        }

    }
}
